using AdmissionInfoSystem.DTOs;
using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<AuthResponseDTO> RegisterUserAsync(RegisterDTO registerDto);
        Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto);
        Task<User> UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
    }
} 