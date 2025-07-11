using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmissionInfoSystem.Models
{
    public class AdmissionNew
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        public DateTime PublishDate { get; set; }
        
        public int? Year { get; set; }
        
        [Required]
        public int UniversityId { get; set; }
        
        // Navigation property
        [ForeignKey("UniversityId")]
        public virtual University?University { get; set; } 
    }
} 