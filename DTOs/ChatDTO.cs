namespace AdmissionInfoSystem.DTOs
{
    public class SendMessageDTO
    {
        public string Message { get; set; } = string.Empty;
        public int? SessionId { get; set; } // Null nếu là session mới
    }

    public class ChatMessageDTO
    {
        public int Id { get; set; }
        public string Sender { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }

    public class ChatSessionDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime StartedAt { get; set; }
        public List<ChatMessageDTO> Messages { get; set; } = new List<ChatMessageDTO>();
    }

    public class ChatResponseDTO
    {
        public int SessionId { get; set; }
        public string BotResponse { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class UpdateSessionTitleDTO
    {
        public string Title { get; set; } = string.Empty;
    }
} 