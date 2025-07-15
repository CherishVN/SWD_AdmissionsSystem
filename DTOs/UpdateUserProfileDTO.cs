using System.ComponentModel.DataAnnotations;

namespace AdmissionInfoSystem.DTOs
{
    public class UpdateUserProfileDTO
    {
        [StringLength(100, ErrorMessage = "Tên hiển thị không được vượt quá 100 ký tự")]
        public string? DisplayName { get; set; }

        [Url(ErrorMessage = "PhotoURL phải là một URL hợp lệ")]
        [StringLength(500, ErrorMessage = "PhotoURL không được vượt quá 500 ký tự")]
        public string? PhotoURL { get; set; }
    }
} 