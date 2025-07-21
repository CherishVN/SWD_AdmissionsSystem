using System.ComponentModel.DataAnnotations;

namespace AdmissionInfoSystem.DTOs
{
    public class UniversityDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Introduction { get; set; } = string.Empty;
        public string OfficialWebsite { get; set; } = string.Empty;
        public string AdmissionWebsite { get; set; } = string.Empty;
        public int? Ranking { get; set; }
        public string RankingCriteria { get; set; } = string.Empty;
        public string? Locations { get; set; }
        public string? Logo { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Type { get; set; } = "Công lập";
        public bool IsVerified { get; set; } = false;
    }

    public class CreateUniversityDTO
    {
        [Required(ErrorMessage = "Tên trường đại học là bắt buộc")]
        [MaxLength(200, ErrorMessage = "Tên trường không được quá 200 ký tự")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Tên viết tắt là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Tên viết tắt không được quá 50 ký tự")]
        public string ShortName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Giới thiệu là bắt buộc")]
        public string Introduction { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Website chính thức là bắt buộc")]
        [Url(ErrorMessage = "Website chính thức phải là URL hợp lệ")]
        public string OfficialWebsite { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Website tuyển sinh là bắt buộc")]
        [Url(ErrorMessage = "Website tuyển sinh phải là URL hợp lệ")]
        public string AdmissionWebsite { get; set; } = string.Empty;
        
        [Range(1, int.MaxValue, ErrorMessage = "Thứ hạng phải là số dương")]
        public int? Ranking { get; set; }
        
        [Required(ErrorMessage = "Tiêu chí xếp hạng là bắt buộc")]
        public string RankingCriteria { get; set; } = string.Empty;
        
        public string? Locations { get; set; }
        
        [Url(ErrorMessage = "Logo phải là URL hợp lệ")]
        [MaxLength(500, ErrorMessage = "URL Logo không được quá 500 ký tự")]
        public string? Logo { get; set; }
        
        [Required(ErrorMessage = "Loại trường là bắt buộc")]
        [RegularExpression("^(Công lập|Tư thục)$", ErrorMessage = "Loại trường phải là 'Công lập' hoặc 'Tư thục'")]
        public string Type { get; set; } = "Công lập";
    }

    public class UpdateUniversityDTO
    {
        [Required(ErrorMessage = "Id là bắt buộc")]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Tên trường đại học là bắt buộc")]
        [MaxLength(200, ErrorMessage = "Tên trường không được quá 200 ký tự")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Tên viết tắt là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Tên viết tắt không được quá 50 ký tự")]
        public string ShortName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Giới thiệu là bắt buộc")]
        public string Introduction { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Website chính thức là bắt buộc")]
        [Url(ErrorMessage = "Website chính thức phải là URL hợp lệ")]
        public string OfficialWebsite { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Website tuyển sinh là bắt buộc")]
        [Url(ErrorMessage = "Website tuyển sinh phải là URL hợp lệ")]
        public string AdmissionWebsite { get; set; } = string.Empty;
        
        [Range(1, int.MaxValue, ErrorMessage = "Thứ hạng phải là số dương")]
        public int? Ranking { get; set; }
        
        [Required(ErrorMessage = "Tiêu chí xếp hạng là bắt buộc")]
        public string RankingCriteria { get; set; } = string.Empty;
        
        public string? Locations { get; set; }
        
        [Url(ErrorMessage = "Logo phải là URL hợp lệ")]
        [MaxLength(500, ErrorMessage = "URL Logo không được quá 500 ký tự")]
        public string? Logo { get; set; }
        
        [Required(ErrorMessage = "Loại trường là bắt buộc")]
        [RegularExpression("^(Công lập|Tư thục)$", ErrorMessage = "Loại trường phải là 'Công lập' hoặc 'Tư thục'")]
        public string Type { get; set; } = "Công lập";
    }
} 