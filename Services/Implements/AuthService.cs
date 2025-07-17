using AdmissionInfoSystem.DTOs;
using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Services.Interface;
using AdmissionInfoSystem.Repositories.Interface;
using AdmissionInfoSystem.Repositories;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace AdmissionInfoSystem.Services.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto)
        {
            try
            {
                var user = await GetUserByEmailOrUsernameAsync(loginDto.EmailOrUsername);

                if (user == null || user.PasswordHash == null ||
                    !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    throw new UnauthorizedAccessException("Email hoặc mật khẩu không chính xác");
                }

                if (!user.EmailVerified)
                {
                    throw new InvalidOperationException("Email chưa được xác minh");
                }

                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                // Generate token
                var token = _jwtService.GenerateToken(user);

                return new AuthResponseDTO
                {
                    User = MapToUserDTO(user),
                    Token = token
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for user: {EmailOrUsername}", loginDto.EmailOrUsername);
                throw;
            }
        }

        public async Task<AuthResponseDTO> GoogleSyncAsync(GoogleSyncDTO googleSyncDto)
        {
            try
            {
                // Find existing user by email or firebaseUid
                var user = await GetUserByEmailAsync(googleSyncDto.Email) ??
                          await GetUserByFirebaseUidAsync(googleSyncDto.FirebaseUid);

                if (user == null)
                {
                    // Create new user from Google account
                    user = new User
                    {
                        Email = googleSyncDto.Email,
                        DisplayName = googleSyncDto.DisplayName,
                        PhotoURL = googleSyncDto.PhotoURL,
                        FirebaseUid = googleSyncDto.FirebaseUid,
                        Provider = "google",
                        Role = "student", // default role
                        EmailVerified = googleSyncDto.EmailVerified,
                        CreatedAt = DateTime.UtcNow,
                        LastLoginAt = DateTime.UtcNow
                    };

                    await _unitOfWork.Users.AddAsync(user);
                }
                else
                {
                    // Update existing user
                    user.FirebaseUid = googleSyncDto.FirebaseUid;
                    user.DisplayName = googleSyncDto.DisplayName;
                    user.PhotoURL = googleSyncDto.PhotoURL;
                    user.EmailVerified = googleSyncDto.EmailVerified;
                    user.LastLoginAt = DateTime.UtcNow;

                    await _unitOfWork.Users.UpdateAsync(user);
                }

                await _unitOfWork.SaveChangesAsync();

                // Generate token
                var token = _jwtService.GenerateToken(user);

                return new AuthResponseDTO
                {
                    User = MapToUserDTO(user),
                    Token = token
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google sync error for email: {Email}", googleSyncDto.Email);
                throw new InvalidOperationException("Đồng bộ tài khoản thất bại");
            }
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto)
        {
            try
            {
                if (string.IsNullOrEmpty(registerDto.Password) && string.IsNullOrEmpty(registerDto.FirebaseUid))
                {
                    throw new ArgumentException("Mật khẩu hoặc Firebase UID là bắt buộc");
                }

                // Check if email already exists
                if (await GetUserByEmailAsync(registerDto.Email) != null)
                {
                    throw new InvalidOperationException("Email đã được sử dụng");
                }

                // Check if username already exists (if provided)
                if (!string.IsNullOrEmpty(registerDto.Username) &&
                    !await CheckUsernameAvailabilityAsync(registerDto.Username))
                {
                    throw new InvalidOperationException("Tên đăng nhập đã được sử dụng");
                }

                // Hash password
                string? passwordHash = null;
                string provider = "email";

                if (!string.IsNullOrEmpty(registerDto.Password))
                {
                    passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
                }
                else if (!string.IsNullOrEmpty(registerDto.FirebaseUid))
                {
                    provider = "firebase";
                }

                // Create user
                var user = new User
                {
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    DisplayName = registerDto.DisplayName,
                    PasswordHash = passwordHash,
                    Role = registerDto.Role,
                    Provider = provider,
                    FirebaseUid = registerDto.FirebaseUid,
                    EmailVerified = registerDto.EmailVerified,
                    UniversityId = registerDto.UniversityId,
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                // Generate token
                var token = _jwtService.GenerateToken(user);

                return new AuthResponseDTO
                {
                    User = MapToUserDTO(user),
                    Token = token
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Register error for email: {Email}", registerDto.Email);
                throw;
            }
        }

        public async Task<bool> CheckUsernameAvailabilityAsync(string username)
        {
            var user = await _unitOfWork.Users.GetUserByUsernameAsync(username);
            return user == null;
        }

        public async Task<User?> GetUserByEmailOrUsernameAsync(string emailOrUsername)
        {
            // Try to get by email first
            var user = await GetUserByEmailAsync(emailOrUsername);
            if (user != null) return user;

            // If not found, try by username
            return await _unitOfWork.Users.GetUserByUsernameAsync(emailOrUsername);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _unitOfWork.Users.GetUserByEmailAsync(email);
        }

        public async Task<User?> GetUserByFirebaseUidAsync(string firebaseUid)
        {
            return await _unitOfWork.Users.GetUserByFirebaseUidAsync(firebaseUid);
        }

        public async Task<AuthResponseDTO> VerifyEmailAsync(VerifyEmailDTO verifyEmailDto)
        {
            try
            {
                // Find user by FirebaseUid or Email
                var user = await _unitOfWork.Users.GetUserByFirebaseUidAsync(verifyEmailDto.FirebaseUid);
                if (user == null)
                {
                    user = await _unitOfWork.Users.GetUserByEmailAsync(verifyEmailDto.Email);
                }

                if (user == null)
                {
                    throw new ArgumentException("Không tìm thấy tài khoản");
                }

                if (user.EmailVerified)
                {
                    throw new InvalidOperationException("Email đã được xác thực trước đó");
                }

                // Update user email verification status
                user.EmailVerified = true;
                user.LastLoginAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                // Generate token
                var token = _jwtService.GenerateToken(user);

                return new AuthResponseDTO
                {
                    User = MapToUserDTO(user),
                    Token = token
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Verify email error for user: {FirebaseUid}, email: {Email}", 
                    verifyEmailDto.FirebaseUid, verifyEmailDto.Email);
                throw;
            }
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            try
            {
                var user = await _unitOfWork.Users.GetUserByEmailAsync(email);
                return user != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Check email exists error for email: {Email}", email);
                throw;
            }
        }

        public async Task<User?> GetUserInfoForPasswordResetAsync(string email)
        {
            try
            {
                var user = await _unitOfWork.Users.GetUserByEmailAsync(email);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get user info for password reset error for email: {Email}", email);
                throw;
            }
        }

        public async Task<bool> UpdatePasswordAfterResetAsync(string email, string newPassword)
        {
            try
            {
                var user = await _unitOfWork.Users.GetUserByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("User not found for password reset: {Email}", email);
                    return false;
                }

                // Hash the new password
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                
                // Update password and provider
                user.PasswordHash = hashedPassword;
                user.Provider = "email"; // Set provider to email since they now have a password

                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Password updated successfully for user: {Email}", email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update password after reset error for email: {Email}", email);
                throw;
            }
        }

        private UserDTO MapToUserDTO(User user)
        {
            return new UserDTO
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
        }
    }
} 