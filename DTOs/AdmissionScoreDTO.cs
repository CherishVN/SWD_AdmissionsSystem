using System.ComponentModel.DataAnnotations;

namespace AdmissionInfoSystem.DTOs
{
    public class AdmissionScoreDTO
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "MajorId là bắt buộc")]
        public int MajorId { get; set; }
        
        [Required(ErrorMessage = "Year là bắt buộc")]
        [Range(2000, 3000, ErrorMessage = "Year phải từ 2000 đến 3000")]
        public int Year { get; set; }
        
        [Required(ErrorMessage = "Score là bắt buộc")]
        [Range(0, 30, ErrorMessage = "Score phải từ 0 đến 30")]
        public decimal Score { get; set; }
        
        public int? AdmissionMethodId { get; set; }
        
        [MaxLength(1000, ErrorMessage = "Note không được quá 1000 ký tự")]
        public string? Note { get; set; }
        
        [MaxLength(200, ErrorMessage = "SubjectCombination không được quá 200 ký tự")]
        public string? SubjectCombination { get; set; }
        
        // Navigation properties cho response
        public string? MajorName { get; set; }
        public string? UniversityName { get; set; }
        public string? AdmissionMethodName { get; set; }
    }

    public class CreateAdmissionScoreDTO
    {
        [Required(ErrorMessage = "MajorId là bắt buộc")]
        public int MajorId { get; set; }
        
        [Required(ErrorMessage = "Year là bắt buộc")]
        [Range(2000, 3000, ErrorMessage = "Year phải từ 2000 đến 3000")]
        public int Year { get; set; }
        
        [Required(ErrorMessage = "Score là bắt buộc")]
        [Range(0, 30, ErrorMessage = "Score phải từ 0 đến 30")]
        public decimal Score { get; set; }
        
        public int? AdmissionMethodId { get; set; }
        
        [MaxLength(1000, ErrorMessage = "Note không được quá 1000 ký tự")]
        public string? Note { get; set; }
        
        [MaxLength(200, ErrorMessage = "SubjectCombination không được quá 200 ký tự")]
        public string? SubjectCombination { get; set; }
    }

    public class UpdateAdmissionScoreDTO
    {
        [Required(ErrorMessage = "Id là bắt buộc")]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "MajorId là bắt buộc")]
        public int MajorId { get; set; }
        
        [Required(ErrorMessage = "Year là bắt buộc")]
        [Range(2000, 3000, ErrorMessage = "Year phải từ 2000 đến 3000")]
        public int Year { get; set; }
        
        [Required(ErrorMessage = "Score là bắt buộc")]
        [Range(0, 30, ErrorMessage = "Score phải từ 0 đến 30")]
        public decimal Score { get; set; }
        
        public int? AdmissionMethodId { get; set; }
        
        [MaxLength(1000, ErrorMessage = "Note không được quá 1000 ký tự")]
        public string? Note { get; set; }
        
        [MaxLength(200, ErrorMessage = "SubjectCombination không được quá 200 ký tự")]
        public string? SubjectCombination { get; set; }
    }
} 