using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmissionInfoSystem.Models
{
    public class ChatMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public int ChatSessionId { get; set; }
        
        [Required]
        public string Sender { get; set; } = string.Empty;
        
        [Required]
        public string Message { get; set; } = string.Empty;
        
        [Required]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        [ForeignKey("ChatSessionId")]
        public virtual ChatSession ChatSession { get; set; } = null!;
    }
} 