using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmissionInfoSystem.Models
{
    public class Scholarship
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Value { get; set; }
        
        [Required]
        public string ValueType { get; set; } = string.Empty;
        
        [Required]
        public string Criteria { get; set; } = string.Empty;
        
        public int? Year { get; set; }
        
        [Required]
        public int UniversityId { get; set; }
        
        // Navigation property
        [ForeignKey("UniversityId")]
        public virtual University? University { get; set; } 
    }
} 