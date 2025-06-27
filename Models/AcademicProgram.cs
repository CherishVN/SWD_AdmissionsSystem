using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AdmissionInfoSystem.Models
{
    public class AcademicProgram
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Tuition { get; set; }
        
        [Required]
        public string TuitionUnit { get; set; } = string.Empty;
        
        public int? Year { get; set; }
        
        [Required]
        public int UniversityId { get; set; }
        
        // Navigation properties
        [ForeignKey("UniversityId")]
        [JsonIgnore]
        public virtual University University { get; set; } = null!;
        
        [JsonIgnore]
        public virtual ICollection<Major> Majors { get; set; } = new List<Major>();
    }
} 