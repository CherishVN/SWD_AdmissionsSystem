using AdmissionInfoSystem.DTOs;

namespace AdmissionInfoSystem.Services.Interface
{
    public interface IAIService
    {
        Task<string> GenerateResponseAsync(string userMessage, List<ChatMessageDTO> chatHistory, string contextData);
        Task<string> GetAdmissionContextAsync(string query);
    }
} 