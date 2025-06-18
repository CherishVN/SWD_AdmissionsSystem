using System.ComponentModel.DataAnnotations;

namespace AdmissionInfoSystem.DTOs
{
    public class RegisterDTO
    {
        public string? Username { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        public string DisplayName { get; set; } = string.Empty;
        
        [Required]
        public string Role { get; set; } = "student"; // student, university
        
        // For university accounts
        public int? UniversityId { get; set; }
    }
} 