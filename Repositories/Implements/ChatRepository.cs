using AdmissionInfoSystem.Data;
using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace AdmissionInfoSystem.Repositories.Implements
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ChatSession> CreateSessionAsync(int userId, string? title = null)
        {
            var session = new ChatSession
            {
                UserId = userId,
                Title = title ?? "Cuộc trò chuyện mới",
                StartedAt = DateTime.UtcNow
            };

            _context.ChatSessions.Add(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<ChatSession?> GetSessionByIdAsync(int sessionId)
        {
            return await _context.ChatSessions
                .Include(s => s.ChatMessages)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == sessionId);
        }

        public async Task<List<ChatSession>> GetUserSessionsAsync(int userId)
        {
            return await _context.ChatSessions
                .Where(s => s.UserId == userId)
                .Include(s => s.ChatMessages)
                .OrderByDescending(s => s.StartedAt)
                .ToListAsync();
        }

        public async Task<ChatMessage> AddMessageAsync(int sessionId, string sender, string message)
        {
            // Map sender names to match database constraint
            var dbSender = sender switch
            {
                "User" => "User",
                "Assistant" => "AI",
                _ => "User" // Default fallback
            };

            var chatMessage = new ChatMessage
            {
                ChatSessionId = sessionId,
                Sender = dbSender,
                Message = message,
                SentAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();
            return chatMessage;
        }

        public async Task<List<ChatMessage>> GetSessionMessagesAsync(int sessionId)
        {
            return await _context.ChatMessages
                .Where(m => m.ChatSessionId == sessionId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateSessionTitleAsync(int sessionId, string title)
        {
            var session = await _context.ChatSessions.FindAsync(sessionId);
            if (session == null) return false;

            session.Title = title;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSessionAsync(int sessionId)
        {
            var session = await _context.ChatSessions
                .Include(s => s.ChatMessages)
                .FirstOrDefaultAsync(s => s.Id == sessionId);
            
            if (session == null) return false;

            _context.ChatMessages.RemoveRange(session.ChatMessages);
            _context.ChatSessions.Remove(session);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 