using Microsoft.AspNetCore.Identity;

namespace PersonalCloudDrive.Models
{
    public class ApplicationUser : IdentityUser
    {
        public long StorageLimit { get; set; } = 1073741824; // 1 GB
        public long StorageUsed { get; set; } = 0;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        
        public virtual ICollection<FileModel> Files { get; set; }
        public virtual ICollection<FolderModel> Folders { get; set; }
    }
}