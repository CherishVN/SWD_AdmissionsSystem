using AdmissionInfoSystem.DTOs;
using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Services;
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
        [Authorize]
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
        [Authorize]
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

        // PUT: api/Users/5 - Chỉ cho phép update DisplayName, Role, PhotoURL
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUser(int id, UpdateUserDTO updateUserDto)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Chỉ update những field được phép
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

            await _userService.UpdateUserAsync(user);

            return NoContent();
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
        [Authorize]
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