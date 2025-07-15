using AdmissionInfoSystem.DTOs;
using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Services;
using AdmissionInfoSystem.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdmissionInfoSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        [AdminAuthorize]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            var userDtos = users.Select(u => new UserDTO
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                DisplayName = u.DisplayName,
                Role = u.Role,
                PhotoURL = u.PhotoURL,
                Provider = u.Provider,
                UniversityId = u.UniversityId,
                EmailVerified = u.EmailVerified,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt
            });

            return Ok(userDtos);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [AdminAuthorize]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDTO
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
            };

            return userDto;
        }

        // PUT: api/Users/5 - User thường chỉnh sửa thông tin cá nhân
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUser(int id, UpdateUserProfileDTO updateUserDto)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy user" });
            }

            // Lấy thông tin user hiện tại từ JWT token
            var currentUserIdClaim = HttpContext.User.FindFirst("userId")?.Value;
            
            if (string.IsNullOrEmpty(currentUserIdClaim) || !int.TryParse(currentUserIdClaim, out int currentUserId))
            {
                return Unauthorized(new { message = "Token không hợp lệ" });
            }

            // User chỉ có thể chỉnh sửa thông tin của chính mình
            if (currentUserId != id)
            {
                return Forbid("Bạn chỉ có thể chỉnh sửa thông tin của chính mình");
            }

            // User thường chỉ được chỉnh sửa DisplayName và PhotoURL
            if (!string.IsNullOrEmpty(updateUserDto.DisplayName))
            {
                user.DisplayName = updateUserDto.DisplayName;
            }

            if (!string.IsNullOrEmpty(updateUserDto.PhotoURL))
            {
                user.PhotoURL = updateUserDto.PhotoURL;
            }

            try
            {
                await _userService.UpdateUserAsync(user);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật thông tin user", error = ex.Message });
            }
        }

        // PUT: api/Users/5/admin - Admin chỉnh sửa tất cả thông tin user
        [HttpPut("{id}/admin")]
        [AdminAuthorize]
        public async Task<IActionResult> PutUserByAdmin(int id, UpdateUserDTO updateUserDto)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy user" });
            }

            // Admin có thể chỉnh sửa tất cả các field
            if (!string.IsNullOrEmpty(updateUserDto.DisplayName))
            {
                user.DisplayName = updateUserDto.DisplayName;
            }

            if (!string.IsNullOrEmpty(updateUserDto.Role))
            {
                user.Role = updateUserDto.Role;
            }

            if (!string.IsNullOrEmpty(updateUserDto.PhotoURL))
            {
                user.PhotoURL = updateUserDto.PhotoURL;
            }

            if (updateUserDto.EmailVerified.HasValue)
            {
                user.EmailVerified = updateUserDto.EmailVerified.Value;
            }

            try
            {
                await _userService.UpdateUserAsync(user);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật user", error = ex.Message });
            }
        }

        // PUT: api/Users/5/change-password - Đổi mật khẩu riêng biệt
        [HttpPut("{id}/change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(int id, ChangePasswordDTO changePasswordDto)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Kiểm tra mật khẩu hiện tại
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                return BadRequest(new { message = "Mật khẩu hiện tại không đúng" });
            }

            // Cập nhật mật khẩu mới
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            await _userService.UpdateUserAsync(user);

            return Ok(new { message = "Đổi mật khẩu thành công" });
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [AdminAuthorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userService.DeleteUserAsync(id);

            return NoContent();
        }
    }
} 