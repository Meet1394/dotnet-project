using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalCloudDrive.Models
{
    public class SharedFileModel
    {
        [Key]
        public int ShareId { get; set; }
        
        [Required]
        public int FileId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string ShareToken { get; set; }
        
        [StringLength(255)]
        public string Password { get; set; }
        
        public DateTime? ExpiryDate { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public int DownloadCount { get; set; } = 0;
        
        [ForeignKey("FileId")]
        public virtual FileModel File { get; set; }
    }
}