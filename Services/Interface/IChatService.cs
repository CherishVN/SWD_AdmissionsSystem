using AdmissionInfoSystem.DTOs;

namespace AdmissionInfoSystem.Services.Interface
{
    public interface IChatService
    {
        Task<ChatResponseDTO> SendMessageAsync(int userId, SendMessageDTO request);
        Task<List<ChatSessionDTO>> GetUserChatHistoryAsync(int userId);
        Task<ChatSessionDTO?> GetChatSessionAsync(int sessionId, int userId);
        Task<bool> DeleteChatSessionAsync(int sessionId, int userId);
        Task<string> GenerateAIResponseAsync(string userMessage, List<ChatMessageDTO> chatHistory);
        Task<bool> UpdateSessionTitleAsync(int sessionId, int userId, string title);
    }
} 