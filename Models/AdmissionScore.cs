using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmissionInfoSystem.Models
{
    public class AdmissionScore
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public int MajorId { get; set; }
        
        [Required]
        public int Year { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Score { get; set; }
        
        public int? AdmissionMethodId { get; set; }
        
        public string? Note { get; set; }
        
        public string? SubjectCombination { get; set; }
        
        // Navigation properties
        [ForeignKey("MajorId")]
        public virtual Major? Major { get; set; }
        
        [ForeignKey("AdmissionMethodId")]
        public virtual AdmissionMethod? AdmissionMethod { get; set; }
    }
} 