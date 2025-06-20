using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using AdmissionInfoSystem.Services;
using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Data;
using AdmissionInfoSystem.Attributes;
using Microsoft.EntityFrameworkCore;

namespace AdmissionInfoSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AdminAuthorize]
    public class UtilityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UtilityController> _logger;

        public UtilityController(ApplicationDbContext context, ILogger<UtilityController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("hash-password")]
        public IActionResult HashPassword([FromBody] string password)
        {
            try
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                return Ok(new { 
                    originalPassword = password,
                    hashedPassword = hashedPassword,
                    note = "Sử dụng hash này để update database"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("verify-password")]
        public IActionResult VerifyPassword([FromBody] VerifyPasswordRequest request)
        {
            try
            {
                var isValid = BCrypt.Net.BCrypt.Verify(request.Password, request.Hash);
                return Ok(new { 
                    password = request.Password,
                    hash = request.Hash,
                    isValid = isValid
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("debug-user/{emailOrUsername}")]
        public async Task<IActionResult> DebugUser(string emailOrUsername)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == emailOrUsername || u.Username == emailOrUsername);

                if (user == null)
                {
                    return NotFound(new { message = "User không tồn tại" });
                }

                return Ok(new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    displayName = user.DisplayName,
                    role = user.Role,
                    provider = user.Provider,
                    emailVerified = user.EmailVerified,
                    hasPasswordHash = !string.IsNullOrEmpty(user.PasswordHash),
                    passwordHashLength = user.PasswordHash?.Length ?? 0,
                    createdAt = user.CreatedAt,
                    lastLoginAt = user.LastLoginAt
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("test-login")]
        public async Task<IActionResult> TestLogin([FromBody] TestLoginRequest request)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.EmailOrUsername || u.Username == request.EmailOrUsername);

                if (user == null)
                {
                    return Ok(new { 
                        success = false, 
                        message = "User không tồn tại",
                        emailOrUsername = request.EmailOrUsername
                    });
                }

                if (string.IsNullOrEmpty(user.PasswordHash))
                {
                    return Ok(new { 
                        success = false, 
                        message = "User không có PasswordHash",
                        userId = user.Id,
                        username = user.Username,
                        email = user.Email
                    });
                }

                var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

                return Ok(new { 
                    success = isPasswordValid, 
                    message = isPasswordValid ? "Password đúng" : "Password sai",
                    userId = user.Id,
                    username = user.Username,
                    email = user.Email,
                    role = user.Role,
                    passwordHashExists = !string.IsNullOrEmpty(user.PasswordHash)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpPost("fix-user-password")]
        public async Task<IActionResult> FixUserPassword([FromBody] FixPasswordRequest request)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null)
                {
                    return NotFound(new { message = "User không tồn tại" });
                }

                // Hash password mới
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                
                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = "Đã cập nhật password thành công",
                    userId = user.Id,
                    email = user.Email,
                    newPasswordHash = user.PasswordHash
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class VerifyPasswordRequest
    {
        public string Password { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;
    }

    public class TestLoginRequest
    {
        public string EmailOrUsername { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class FixPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
} 