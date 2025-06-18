using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmissionInfoSystem.Models
{
    public class University
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string ShortName { get; set; } = string.Empty;
        
        [Required]
        public string Introduction { get; set; } = string.Empty;
        
        [Required]
        public string OfficialWebsite { get; set; } = string.Empty;
        
        [Required]
        public string AdmissionWebsite { get; set; } = string.Empty;
        
        public int? Ranking { get; set; }
        
        [Required]
        public string RankingCriteria { get; set; } = string.Empty;
        
        // Thêm thuộc tính Locations để lưu trữ các thẻ vị trí
        public string? Locations { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Type { get; set; } = "Công lập";
        
        // Navigation properties
        public virtual ICollection<AdmissionNew> AdmissionNews { get; set; } = new List<AdmissionNew>();
        public virtual ICollection<AdmissionMethod> AdmissionMethods { get; set; } = new List<AdmissionMethod>();
        public virtual ICollection<AcademicProgram> Programs { get; set; } = new List<AcademicProgram>();
        public virtual ICollection<Major> Majors { get; set; } = new List<Major>();
        public virtual ICollection<Scholarship> Scholarships { get; set; } = new List<Scholarship>();
    }
} 