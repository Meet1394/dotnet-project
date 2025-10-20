using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalCloudDrive.Models
{
    public class FolderModel
    {
        [Key]
        public int FolderId { get; set; }
        
        [Required]
        [StringLength(255)]
        public string FolderName { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        public int? ParentFolderId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedOn { get; set; }
        
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        
        [ForeignKey("ParentFolderId")]
        public virtual FolderModel ParentFolder { get; set; }
        
        public virtual ICollection<FolderModel> SubFolders { get; set; }
        public virtual ICollection<FileModel> Files { get; set; }
    }
}