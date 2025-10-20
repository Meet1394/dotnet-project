using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalCloudDrive.Models
{
    public class FileModel
    {
        [Key]
        public int FileId { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [Required]
        [StringLength(255)]
        public string FileName { get; set; }
        
        [Required]
        public string FilePath { get; set; }
        
        [Required]
        [StringLength(50)]
        public string FileType { get; set; }
        
        public long FileSize { get; set; }
        public DateTime UploadedOn { get; set; } = DateTime.Now;
        public DateTime? LastModified { get; set; }
        public int? ParentFolderId { get; set; }
        public int Version { get; set; } = 1;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedOn { get; set; }
        
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        
        [ForeignKey("ParentFolderId")]
        public virtual FolderModel ParentFolder { get; set; }
    }
}