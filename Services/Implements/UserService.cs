using AdmissionInfoSystem.DTOs;
using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Repositories;
using AdmissionInfoSystem.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace AdmissionInfoSystem.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public UserService(IUnitOfWork unitOfWork, IConfiguration configuration, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _unitOfWork.Users.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _unitOfWork.Users.GetByIdAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _unitOfWork.Users.GetUserByUsernameAsync(username);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _unitOfWork.Users.GetUserByEmailAsync(email);
        }

        public async Task<AuthResponseDTO> RegisterUserAsync(RegisterDTO registerDto)
        {
            try
            {
                // Kiểm tra email đã tồn tại chưa
                var existingUserByEmail = await _unitOfWork.Users.GetUserByEmailAsync(registerDto.Email);
                if (existingUserByEmail != null)
                {
                    return new AuthResponseDTO
                    {
                        User = new UserDTO(),
                        Token = string.Empty
                    };
                }

                // Kiểm tra username đã tồn tại chưa (nếu có)
                if (!string.IsNullOrEmpty(registerDto.Username))
                {
                    var existingUserByUsername = await _unitOfWork.Users.GetUserByUsernameAsync(registerDto.Username);
                    if (existingUserByUsername != null)
                    {
                        return new AuthResponseDTO
                        {
                            User = new UserDTO(),
                            Token = string.Empty
                        };
                    }
                }

                // Xử lý UniversityId dựa trên Role
                int? universityId = null;
                if (registerDto.Role.ToLower() == "university")
                {
                    // Nếu là tài khoản đại học, bắt buộc phải có UniversityId hợp lệ
                    if (!registerDto.UniversityId.HasValue)
                    {
                        return new AuthResponseDTO
                        {
                            User = new UserDTO(),
                            Token = string.Empty
                        };
                    }

                    var university = await _unitOfWork.Universities.GetByIdAsync(registerDto.UniversityId.Value);
                    if (university == null)
                    {
                        return new AuthResponseDTO
                        {
                            User = new UserDTO(),
                            Token = string.Empty
                        };
                    }

                    universityId = registerDto.UniversityId;
                }

                // Mã hóa mật khẩu
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                // Tạo user mới
                var user = new User
                {
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    DisplayName = registerDto.DisplayName,
                    PasswordHash = hashedPassword,
                    Role = registerDto.Role,
                    Provider = "email",
                    UniversityId = universityId,
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                // Tạo token
                var token = GenerateJwtToken(user);

                return new AuthResponseDTO
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
            }
            catch (Exception ex)
            {
                return new AuthResponseDTO
                {
                    User = new UserDTO(),
                    Token = string.Empty
                };
            }
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto)
        {
            try
            {
                var user = await _unitOfWork.Users.AuthenticateAsync(loginDto.EmailOrUsername, loginDto.Password);
                if (user == null)
                {
                    return new AuthResponseDTO
                    {
                        User = new UserDTO(),
                        Token = string.Empty
                    };
                }

                // Cập nhật last login
                user.LastLoginAt = DateTime.UtcNow;
                
                // Chỉ cập nhật LastLoginAt, không update toàn bộ entity
                _context.Entry(user).Property(u => u.LastLoginAt).IsModified = true;
                await _unitOfWork.SaveChangesAsync();

                // Tạo token
                var token = GenerateJwtToken(user);

                return new AuthResponseDTO
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
            }
            catch (Exception ex)
            {
                return new AuthResponseDTO
                {
                    User = new UserDTO(),
                    Token = string.Empty
                };
            }
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return user;
        }

        public async Task DeleteUserAsync(int id)
        {
            await _unitOfWork.Users.RemoveAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "ThisIsADefaultSecretKeyForJWTAuthentication12345");
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", user.Id.ToString()),
                    new Claim("email", user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("displayName", user.DisplayName ?? ""),
                    new Claim("provider", user.Provider ?? "email")
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"] ?? "AdmissionInfoSystem",
                Audience = _configuration["Jwt:Audience"] ?? "AdmissionInfoSystemClient"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
} 