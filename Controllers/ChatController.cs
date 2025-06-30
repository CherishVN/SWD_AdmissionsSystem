using AdmissionInfoSystem.DTOs;
using AdmissionInfoSystem.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdmissionInfoSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Gửi tin nhắn và nhận phản hồi từ AI
        /// </summary>
        [HttpPost("send")]
        public async Task<ActionResult<ChatResponseDTO>> SendMessage([FromBody] SendMessageDTO request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var response = await _chatService.SendMessageAsync(userId, request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi xử lý tin nhắn", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy lịch sử chat của người dùng
        /// </summary>
        [HttpGet("history")]
        public async Task<ActionResult<List<ChatSessionDTO>>> GetChatHistory()
        {
            try
            {
                var userId = GetCurrentUserId();
                var sessions = await _chatService.GetUserChatHistoryAsync(userId);
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy lịch sử chat", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy chi tiết một phiên chat
        /// </summary>
        [HttpGet("session/{sessionId}")]
        public async Task<ActionResult<ChatSessionDTO>> GetChatSession(int sessionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var session = await _chatService.GetChatSessionAsync(sessionId, userId);
                
                if (session == null)
                    return NotFound(new { message = "Không tìm thấy phiên chat" });

                return Ok(session);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy thông tin phiên chat", error = ex.Message });
            }
        }

        /// <summary>
        /// Xóa một phiên chat
        /// </summary>
        [HttpDelete("session/{sessionId}")]
        public async Task<ActionResult> DeleteChatSession(int sessionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _chatService.DeleteChatSessionAsync(sessionId, userId);
                
                if (!success)
                    return NotFound(new { message = "Không tìm thấy phiên chat hoặc bạn không có quyền xóa" });

                return Ok(new { message = "Đã xóa phiên chat thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi xóa phiên chat", error = ex.Message });
            }
        }

        /// <summary>
        /// Tạo phiên chat mới
        /// </summary>
        [HttpPost("new-session")]
        public async Task<ActionResult<ChatSessionDTO>> CreateNewSession([FromBody] string? title = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                // Gửi tin nhắn đầu tiên để tạo session
                var initialMessage = new SendMessageDTO
                {
                    Message = "Xin chào! Tôi có thể giúp gì cho bạn về tuyển sinh?",
                    SessionId = null
                };

                var response = await _chatService.SendMessageAsync(userId, initialMessage);
                var session = await _chatService.GetChatSessionAsync(response.SessionId, userId);
                
                return Ok(session);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi tạo phiên chat mới", error = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Không thể xác định người dùng");
            }
            return userId;
        }
    }
} 