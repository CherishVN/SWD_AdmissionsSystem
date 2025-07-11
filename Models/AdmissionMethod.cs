using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmissionInfoSystem.Models
{
    public class AdmissionMethod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public string Criteria { get; set; } = string.Empty;
        
        public int? Year { get; set; }
        
        [Required]
        public int UniversityId { get; set; }
        
        // Navigation properties
        [ForeignKey("UniversityId")]
        public virtual University? University { get; set; } 
        
        public virtual ICollection<AdmissionCriteria> AdmissionCriterias { get; set; } = new List<AdmissionCriteria>();
    }
} 