using System.ComponentModel.DataAnnotations;

namespace AdmissionInfoSystem.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MinLength(5, ErrorMessage = "Mật khẩu phải có ít nhất 5 ký tự")]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        [Required]
        // Loại người dùng (user: người dùng thông thường, university: người dùng thuộc trường đại học)
        public string UserType { get; set; } = string.Empty;
        
        // Chỉ cần nhập khi UserType là "university", và phải là ID trường hợp lệ
        // Với người dùng thông thường, UniversityId sẽ được đặt là null
        public int? UniversityId { get; set; }
    }
} 