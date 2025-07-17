using AdmissionInfoSystem.DTOs;
using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Services.Interface
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto);
        Task<AuthResponseDTO> GoogleSyncAsync(GoogleSyncDTO googleSyncDto);
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto);
        Task<bool> CheckUsernameAvailabilityAsync(string username);
        Task<User?> GetUserByEmailOrUsernameAsync(string emailOrUsername);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByFirebaseUidAsync(string firebaseUid);
        Task<AuthResponseDTO> VerifyEmailAsync(VerifyEmailDTO verifyEmailDto);
        Task<bool> CheckEmailExistsAsync(string email);
        Task<User?> GetUserInfoForPasswordResetAsync(string email);
        Task<bool> UpdatePasswordAfterResetAsync(string email, string newPassword);
    }
} 