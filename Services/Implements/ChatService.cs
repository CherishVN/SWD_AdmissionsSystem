using AdmissionInfoSystem.DTOs;
using AdmissionInfoSystem.Repositories.Interface;
using AdmissionInfoSystem.Services.Interface;

namespace AdmissionInfoSystem.Services.Implements
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IAIService _aiService;

        public ChatService(IChatRepository chatRepository, IAIService aiService)
        {
            _chatRepository = chatRepository;
            _aiService = aiService;
        }

        public async Task<ChatResponseDTO> SendMessageAsync(int userId, SendMessageDTO request)
        {
            // Tạo hoặc lấy session
            var session = request.SessionId.HasValue 
                ? await _chatRepository.GetSessionByIdAsync(request.SessionId.Value)
                : await _chatRepository.CreateSessionAsync(userId);

            if (session == null)
                throw new ArgumentException("Không tìm thấy phiên chat");

            // Kiểm tra quyền truy cập
            if (session.UserId != userId)
                throw new UnauthorizedAccessException("Bạn không có quyền truy cập phiên chat này");

            // Lưu tin nhắn của người dùng
            await _chatRepository.AddMessageAsync(session.Id, "User", request.Message);

            // Lấy lịch sử chat
            var chatHistory = await GetChatHistoryForAI(session.Id);

            // Tạo phản hồi AI
            var aiResponse = await GenerateAIResponseAsync(request.Message, chatHistory);

            // Lưu phản hồi của AI
            var botMessage = await _chatRepository.AddMessageAsync(session.Id, "AI", aiResponse);

            // Tự động tạo title cho session mới
            if (string.IsNullOrEmpty(session.Title) || session.Title == "Cuộc trò chuyện mới")
            {
                var title = GenerateSessionTitle(request.Message);
                await _chatRepository.UpdateSessionTitleAsync(session.Id, title);
            }

            return new ChatResponseDTO
            {
                SessionId = session.Id,
                BotResponse = aiResponse,
                Timestamp = botMessage.SentAt
            };
        }

        public async Task<List<ChatSessionDTO>> GetUserChatHistoryAsync(int userId)
        {
            var sessions = await _chatRepository.GetUserSessionsAsync(userId);
            
            return sessions.Select(s => new ChatSessionDTO
            {
                Id = s.Id,
                Title = s.Title,
                StartedAt = s.StartedAt,
                Messages = s.ChatMessages.Select(m => new ChatMessageDTO
                {
                    Id = m.Id,
                    Sender = m.Sender == "AI" ? "Assistant" : m.Sender, // Map AI back to Assistant for display
                    Message = m.Message,
                    SentAt = m.SentAt
                }).OrderBy(m => m.SentAt).ToList()
            }).ToList();
        }

        public async Task<ChatSessionDTO?> GetChatSessionAsync(int sessionId, int userId)
        {
            var session = await _chatRepository.GetSessionByIdAsync(sessionId);
            
            if (session == null || session.UserId != userId)
                return null;

            return new ChatSessionDTO
            {
                Id = session.Id,
                Title = session.Title,
                StartedAt = session.StartedAt,
                Messages = session.ChatMessages.Select(m => new ChatMessageDTO
                {
                    Id = m.Id,
                    Sender = m.Sender == "AI" ? "Assistant" : m.Sender, // Map AI back to Assistant for display
                    Message = m.Message,
                    SentAt = m.SentAt
                }).OrderBy(m => m.SentAt).ToList()
            };
        }

        public async Task<bool> DeleteChatSessionAsync(int sessionId, int userId)
        {
            var session = await _chatRepository.GetSessionByIdAsync(sessionId);
            
            if (session == null || session.UserId != userId)
                return false;

            return await _chatRepository.DeleteSessionAsync(sessionId);
        }

        public async Task<string> GenerateAIResponseAsync(string userMessage, List<ChatMessageDTO> chatHistory)
        {
            // Lấy ngữ cảnh từ cơ sở dữ liệu
            var contextData = await _aiService.GetAdmissionContextAsync(userMessage);
            
            // Tạo phản hồi AI
            return await _aiService.GenerateResponseAsync(userMessage, chatHistory, contextData);
        }

        private async Task<List<ChatMessageDTO>> GetChatHistoryForAI(int sessionId)
        {
            var messages = await _chatRepository.GetSessionMessagesAsync(sessionId);
            
            return messages.Select(m => new ChatMessageDTO
            {
                Id = m.Id,
                Sender = m.Sender == "AI" ? "Assistant" : m.Sender, // Map AI back to Assistant for display
                Message = m.Message,
                SentAt = m.SentAt
            }).ToList();
        }

        private string GenerateSessionTitle(string firstMessage)
        {
            // Tạo title ngắn gọn từ tin nhắn đầu tiên
            var title = firstMessage.Length > 50 
                ? firstMessage.Substring(0, 47) + "..." 
                : firstMessage;
            
            return title;
        }
    }
} 