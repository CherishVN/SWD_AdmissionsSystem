using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Repositories.Interface
{
    public interface IChatRepository
    {
        Task<ChatSession> CreateSessionAsync(int userId, string? title = null);
        Task<ChatSession?> GetSessionByIdAsync(int sessionId);
        Task<List<ChatSession>> GetUserSessionsAsync(int userId);
        Task<ChatMessage> AddMessageAsync(int sessionId, string sender, string message);
        Task<List<ChatMessage>> GetSessionMessagesAsync(int sessionId);
        Task<bool> UpdateSessionTitleAsync(int sessionId, string title);
        Task<bool> DeleteSessionAsync(int sessionId);
    }
} 