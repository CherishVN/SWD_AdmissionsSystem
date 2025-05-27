using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmissionInfoSystem.Models
{
    public class AdmissionCriteria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinimumScore { get; set; }
        
        [Required]
        public int AdmissionMethodId { get; set; }
        
        // Navigation property
        [ForeignKey("AdmissionMethodId")]
        public virtual AdmissionMethod AdmissionMethod { get; set; } = null!;
    }
} 