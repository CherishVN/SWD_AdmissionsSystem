using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using AdmissionInfoSystem.Data;
using AdmissionInfoSystem.DTOs;
using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Services;

namespace AdmissionInfoSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            ApplicationDbContext context, 
            IJwtService jwtService,
            ILogger<AuthController> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO request)
        {
            try
            {
                // Find user by email or username
                var user = await _context.Users
                    .Include(u => u.University)
                    .FirstOrDefaultAsync(u => 
                        u.Email == request.EmailOrUsername || 
                        u.Username == request.EmailOrUsername);

                if (user == null || user.PasswordHash == null || 
                    !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return Unauthorized(new { message = "Email hoặc mật khẩu không chính xác" });
                }

                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Generate token
                var token = _jwtService.GenerateToken(user);

                var response = new AuthResponseDTO
                {
                    User = new UserDTO
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        DisplayName = user.DisplayName,
                        Role = user.Role,
                        PhotoURL = user.PhotoURL,
                        Provider = user.Provider,
                        UniversityId = user.UniversityId,
                        EmailVerified = user.EmailVerified,
                        CreatedAt = user.CreatedAt,
                        LastLoginAt = user.LastLoginAt
                    },
                    Token = token
                };

                return Ok(response);
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
                // Find existing user by email or firebaseUid
                var user = await _context.Users
                    .Include(u => u.University)
                    .FirstOrDefaultAsync(u => 
                        u.Email == request.Email || 
                        u.FirebaseUid == request.FirebaseUid);

                if (user == null)
                {
                    // Create new user from Google account
                    user = new User
                    {
                        Email = request.Email,
                        DisplayName = request.DisplayName,
                        PhotoURL = request.PhotoURL,
                        FirebaseUid = request.FirebaseUid,
                        Provider = "google",
                        Role = "student", // default role
                        EmailVerified = request.EmailVerified,
                        CreatedAt = DateTime.UtcNow,
                        LastLoginAt = DateTime.UtcNow
                    };

                    _context.Users.Add(user);
                }
                else
                {
                    // Update existing user
                    user.FirebaseUid = request.FirebaseUid;
                    user.DisplayName = request.DisplayName;
                    user.PhotoURL = request.PhotoURL;
                    user.EmailVerified = request.EmailVerified;
                    user.LastLoginAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                // Generate token
                var token = _jwtService.GenerateToken(user);

                var response = new AuthResponseDTO
                {
                    User = new UserDTO
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        DisplayName = user.DisplayName,
                        Role = user.Role,
                        PhotoURL = user.PhotoURL,
                        Provider = user.Provider,
                        UniversityId = user.UniversityId,
                        EmailVerified = user.EmailVerified,
                        CreatedAt = user.CreatedAt,
                        LastLoginAt = user.LastLoginAt
                    },
                    Token = token
                };

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
                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                {
                    return BadRequest(new { message = "Email đã được sử dụng" });
                }

                // Check if username already exists (if provided)
                if (!string.IsNullOrEmpty(request.Username) && 
                    await _context.Users.AnyAsync(u => u.Username == request.Username))
                {
                    return BadRequest(new { message = "Tên đăng nhập đã được sử dụng" });
                }

                // Hash password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create user
                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    DisplayName = request.DisplayName,
                    PasswordHash = passwordHash,
                    Role = request.Role,
                    Provider = "email",
                    UniversityId = request.UniversityId,
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Generate token
                var token = _jwtService.GenerateToken(user);

                var response = new AuthResponseDTO
                {
                    User = new UserDTO
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        DisplayName = user.DisplayName,
                        Role = user.Role,
                        PhotoURL = user.PhotoURL,
                        Provider = user.Provider,
                        UniversityId = user.UniversityId,
                        EmailVerified = user.EmailVerified,
                        CreatedAt = user.CreatedAt,
                        LastLoginAt = user.LastLoginAt
                    },
                    Token = token
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Register error for email: {Email}", request.Email);
                return StatusCode(500, new { message = "Đăng ký thất bại" });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // In JWT, logout is handled client-side by removing the token
            // You can implement token blacklisting here if needed
            
            return Ok(new { message = "Đăng xuất thành công" });
        }
    }
} 