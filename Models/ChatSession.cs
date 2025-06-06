using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmissionInfoSystem.Models
{
    public class ChatSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        
        public string? Title { get; set; }
        
        // Navigation property
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
        
        // Navigation property for messages
        public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    }
} 