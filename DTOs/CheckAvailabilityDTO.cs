using System.ComponentModel.DataAnnotations;

namespace AdmissionInfoSystem.DTOs
{
    public class CheckAvailabilityDTO
    {
        [Required]
        public string? Username { get; set; }
    }
} 