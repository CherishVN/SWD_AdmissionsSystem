using System.ComponentModel.DataAnnotations;

namespace AdmissionInfoSystem.DTOs
{
    public class GoogleSyncDTO
    {
        [Required]
        public string FirebaseUid { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public string? DisplayName { get; set; }
        
        public string? PhotoURL { get; set; }
        
        public bool EmailVerified { get; set; }
    }
} 