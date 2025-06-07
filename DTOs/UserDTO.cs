using System.ComponentModel.DataAnnotations;

namespace AdmissionInfoSystem.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [EmailAddress]
        public string? Email { get; set; }
        
        public string UserType { get; set; } = string.Empty;
        
        public int? UniversityId { get; set; }
    }
} 