using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PersonalCloudDrive.Data;
using PersonalCloudDrive.Models;
using Microsoft.EntityFrameworkCore;

namespace PersonalCloudDrive.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            // Get user statistics
            var totalFiles = await _context.Files
                .Where(f => f.UserId == userId && !f.IsDeleted)
                .CountAsync();

            var totalFolders = await _context.Folders
                .Where(f => f.UserId == userId && !f.IsDeleted)
                .CountAsync();

            var recentFiles = await _context.Files
                .Where(f => f.UserId == userId && !f.IsDeleted)
                .OrderByDescending(f => f.UploadedOn)
                .Take(10)
                .ToListAsync();

            // Ensure StorageUsed is in sync with actual stored files in the database.
            // This prevents showing a non-zero default when no files exist.
            var dbStorageUsed = await _context.Files
                .Where(f => f.UserId == userId && !f.IsDeleted)
                .Select(f => (long?)f.FileSize)
                .SumAsync() ?? 0L;

            if (user != null && user.StorageUsed != dbStorageUsed)
            {
                user.StorageUsed = dbStorageUsed;
                // update the user record to persist corrected storage
                await _userManager.UpdateAsync(user);
            }

            ViewBag.User = user;
            ViewBag.TotalFiles = totalFiles;
            ViewBag.TotalFolders = totalFolders;
            ViewBag.RecentFiles = recentFiles;
            ViewBag.StorageUsedPercentage = (user != null && user.StorageLimit > 0)
                ? (user.StorageUsed * 100.0 / user.StorageLimit)
                : 0.0;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest(new { message = "No files provided" });
            }

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            var uploadPathConfig = "wwwroot/uploads"; // fallback
            try
            {
                var userUploadDir = Path.Combine(Directory.GetCurrentDirectory(), uploadPathConfig, userId);
                Directory.CreateDirectory(userUploadDir);

                foreach (var file in files)
                {
                    if (file.Length <= 0) continue;
                    var safeFileName = Path.GetFileName(file.FileName);
                    var uniqueName = $"{Guid.NewGuid()}_{safeFileName}";
                    var filePath = Path.Combine(userUploadDir, uniqueName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var fileModel = new FileModel
                    {
                        UserId = userId,
                        FileName = safeFileName,
                        FilePath = $"/uploads/{userId}/{uniqueName}",
                        FileType = Path.GetExtension(safeFileName)?.TrimStart('.') ?? "",
                        FileSize = file.Length,
                        UploadedOn = DateTime.Now
                    };

                    _context.Files.Add(fileModel);
                    user.StorageUsed += file.Length;
                }

                await _context.SaveChangesAsync();
                await _userManager.UpdateAsync(user);

                // Recalculate totals to return to client
                var totalFiles = await _context.Files
                    .Where(f => f.UserId == userId && !f.IsDeleted)
                    .CountAsync();

                var totalFolders = await _context.Folders
                    .Where(f => f.UserId == userId && !f.IsDeleted)
                    .CountAsync();

                var storageUsedPercentage = (user.StorageUsed * 100.0 / user.StorageLimit);

                return Json(new
                {
                    success = true,
                    totalFiles,
                    totalFolders,
                    storageUsedPercentage,
                    storageUsed = user.StorageUsed,
                    storageLimit = user.StorageLimit
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var userId = _userManager.GetUserId(User);
            var file = await _context.Files.FirstOrDefaultAsync(f => f.FileId == id && f.UserId == userId && !f.IsDeleted);
            if (file == null)
                return NotFound();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (!System.IO.File.Exists(filePath))
                return NotFound();
            var contentType = "application/octet-stream";
            return PhysicalFile(filePath, contentType, file.FileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var userId = _userManager.GetUserId(User);
            var file = await _context.Files.FirstOrDefaultAsync(f => f.FileId == id && f.UserId == userId && !f.IsDeleted);
            if (file == null)
                return Json(new { success = false, message = "File not found" });
            file.IsDeleted = true;
            file.DeletedOn = DateTime.Now;

            // Subtract file size from user's storage used (ensure non-negative)
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.StorageUsed = Math.Max(0, user.StorageUsed - file.FileSize);
                await _userManager.UpdateAsync(user);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}