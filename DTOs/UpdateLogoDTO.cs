using System.ComponentModel.DataAnnotations;

namespace AdmissionInfoSystem.DTOs
{
    public class UpdateLogoDTO
    {
        [Url(ErrorMessage = "Logo phải là một URL hợp lệ")]
        [MaxLength(500, ErrorMessage = "URL Logo không được vượt quá 500 ký tự")]
        public string? Logo { get; set; }
    }
} 