using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmissionInfoSystem.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string Username { get; set; } = string.Empty;
        
        public string? Email { get; set; }
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        public string UserType { get; set; } = string.Empty;
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Foreign key
        public int? UniversityId { get; set; }
        
        // Navigation property
        [ForeignKey("UniversityId")]
        public virtual University? University { get; set; }
        
        // Navigation property for ChatSessions
        public virtual ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
    }
} 