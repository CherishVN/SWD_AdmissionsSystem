using Microsoft.AspNetCore.Mvc;
using AdmissionInfoSystem.DTOs;
using AdmissionInfoSystem.Services.Interface;
using AdmissionInfoSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace AdmissionInfoSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                    code = "EMAIL_NOT_VERIFIED",
                    email = request.EmailOrUsername
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for user: {EmailOrUsername}", request.EmailOrUsername);
                return StatusCode(500, new { message = "Lỗi server" });
            }
        }

        [HttpPost("google-sync")]
        public async Task<ActionResult<AuthResponseDTO>> GoogleSync([FromBody] GoogleSyncDTO request)
        {
            try
            {
                var response = await _authService.GoogleSyncAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google sync error for email: {Email}", request.Email);
                return StatusCode(500, new { message = "Đồng bộ tài khoản thất bại" });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> Register([FromBody] RegisterDTO request)
        {
            try
            {
                var response = await _authService.RegisterAsync(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Register error for email: {Email}", request.Email);
                return StatusCode(500, new { message = "Đăng ký thất bại" });
            }
        }

        [HttpPut("verify-email")]
        public async Task<ActionResult<AuthResponseDTO>> VerifyEmail([FromBody] VerifyEmailDTO request)
        {
            try
            {
                var response = await _authService.VerifyEmailAsync(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Verify email error for user: {FirebaseUid}, email: {Email}", request.FirebaseUid, request.Email);
                return StatusCode(500, new { message = "Lỗi server" });
            }
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            
            return Ok(new { message = "Đăng xuất thành công" });
        }

        [HttpPost("check-username")]
        public async Task<IActionResult> CheckUsernameAvailability([FromBody] CheckAvailabilityDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username))
                {
                    return BadRequest(new { message = "Username không được để trống" });
                }
                
                var isAvailable = await _authService.CheckUsernameAvailabilityAsync(request.Username);
                
                return Ok(new { 
                    available = isAvailable,
                    message = isAvailable ? "Username có thể sử dụng" : "Username đã được sử dụng"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Check username availability error for: {Username}", request.Username);
                return StatusCode(500, new { message = "Lỗi server" });
            }
        }

        [HttpGet("check-availability")]
        public async Task<IActionResult> CheckEmailAvailability([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new { message = "Email không được để trống" });
                }
                
                var userInfo = await _authService.GetUserInfoForPasswordResetAsync(email);
                
                if (userInfo == null)
                {
                    return Ok(new { 
                        exists = false,
                        message = "Email chưa được đăng ký"
                    });
                }
                
                return Ok(new { 
                    exists = true,
                    provider = userInfo.Provider,
                    hasPassword = !string.IsNullOrEmpty(userInfo.PasswordHash),
                    message = "Email đã tồn tại"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Check email availability error for: {Email}", email);
                return StatusCode(500, new { message = "Lỗi server" });
            }
        }

        [HttpPut("update-password-after-reset")]
        public async Task<IActionResult> UpdatePasswordAfterReset([FromBody] UpdatePasswordAfterResetDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.NewPassword))
                {
                    return BadRequest(new { message = "Email và mật khẩu mới không được để trống" });
                }
                
                var result = await _authService.UpdatePasswordAfterResetAsync(request.Email, request.NewPassword);
                
                if (result)
                {
                    return Ok(new { message = "Cập nhật mật khẩu thành công" });
                }
                else
                {
                    return BadRequest(new { message = "Không thể cập nhật mật khẩu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update password after reset error for email: {Email}", request.Email);
                return StatusCode(500, new { message = "Lỗi server" });
            }
        }
    }
} 