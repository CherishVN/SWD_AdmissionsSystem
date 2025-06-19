using System.ComponentModel.DataAnnotations;

namespace AdmissionInfoSystem.DTOs
{
    public class UpdateUserDTO
    {
        [StringLength(100, ErrorMessage = "Tên hiển thị không được vượt quá 100 ký tự")]
        public string? DisplayName { get; set; }

        [StringLength(50, ErrorMessage = "Role không được vượt quá 50 ký tự")]
        public string? Role { get; set; }

        [Url(ErrorMessage = "PhotoURL phải là một URL hợp lệ")]
        [StringLength(500, ErrorMessage = "PhotoURL không được vượt quá 500 ký tự")]
        public string? PhotoURL { get; set; }
    }
} 