using Microsoft.AspNetCore.Mvc;
using PersonalCloudDrive.Models;
using System.Diagnostics;

namespace PersonalCloudDrive.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // If user is authenticated, redirect to dashboard
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}