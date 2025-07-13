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
    }
} 