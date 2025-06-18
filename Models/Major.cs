using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmissionInfoSystem.Models
{
    public class Major
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Code { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? AdmissionScore { get; set; }
        
        public int? Year { get; set; }
        
        [Required]
        public int UniversityId { get; set; }
        
        public int? ProgramId { get; set; }
        
        // Navigation properties
        [ForeignKey("UniversityId")]
        public virtual University University { get; set; } = null!;
        
        [ForeignKey("ProgramId")]
        public virtual AcademicProgram? Program { get; set; }
    }
} 