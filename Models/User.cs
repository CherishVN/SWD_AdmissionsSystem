using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmissionInfoSystem.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [StringLength(50)]
        public string? Username { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? DisplayName { get; set; }
        
        [StringLength(500)]
        public string? PhotoURL { get; set; }
        
        [StringLength(100)]
        public string? PasswordHash { get; set; }
        
        [StringLength(50)]
        public string? FirebaseUid { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "student";
        
        [StringLength(20)]
        public string Provider { get; set; } = "email";
        
        public bool EmailVerified { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        
        public DateTime? LastLoginAt { get; set; }
        
        public int? UniversityId { get; set; }
        
        [ForeignKey("UniversityId")]
        public virtual University? University { get; set; }
        
        public virtual ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
    }
} 