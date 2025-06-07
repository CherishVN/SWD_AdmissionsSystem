using AdmissionInfoSystem.DTOs;
using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Repositories;
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
        private const string UNIVERSITY_USER_TYPE = "university";

        public UserService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _unitOfWork.Users.GetAllAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _unitOfWork.Users.GetByIdAsync(id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _unitOfWork.Users.GetUserByUsernameAsync(username);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _unitOfWork.Users.GetUserByEmailAsync(email);
        }

        public async Task<AuthResponseDTO> RegisterUserAsync(RegisterDTO registerDto)
        {
            try
            {
                // Kiểm tra username đã tồn tại chưa
                var existingUserByUsername = await _unitOfWork.Users.GetUserByUsernameAsync(registerDto.Username);
                if (existingUserByUsername != null)
                {
                    return new AuthResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Tên đăng nhập đã tồn tại"
                    };
                }

                // Kiểm tra email đã tồn tại chưa
                var existingUserByEmail = await _unitOfWork.Users.GetUserByEmailAsync(registerDto.Email);
                if (existingUserByEmail != null)
                {
                    return new AuthResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Email đã tồn tại"
                    };
                }

                // Xử lý UniversityId dựa trên UserType
                int? universityId = null;
                if (registerDto.UserType.ToLower() == UNIVERSITY_USER_TYPE)
                {
                    // Nếu là tài khoản đại học, bắt buộc phải có UniversityId hợp lệ
                    if (!registerDto.UniversityId.HasValue)
                    {
                        return new AuthResponseDTO
                        {
                            IsSuccess = false,
                            Message = "Tài khoản đại học cần cung cấp ID trường"
                        };
                    }

                    var university = await _unitOfWork.Universities.GetByIdAsync(registerDto.UniversityId.Value);
                    if (university == null)
                    {
                        return new AuthResponseDTO
                        {
                            IsSuccess = false,
                            Message = $"Trường đại học với ID {registerDto.UniversityId.Value} không tồn tại"
                        };
                    }

                    universityId = registerDto.UniversityId;
                }
                else
                {
                    // Nếu là tài khoản người dùng bình thường, UniversityId phải là null
                    universityId = null;
                }

                // Mã hóa mật khẩu
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                // Tạo user mới
                var user = new User
                {
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    Password = hashedPassword, // Sử dụng mật khẩu đã mã hóa
                    UserType = registerDto.UserType,
                    UniversityId = universityId,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                // Tạo token
                var token = GenerateJwtToken(user);

                return new AuthResponseDTO
                {
                    IsSuccess = true,
                    Message = "Đăng ký thành công",
                    Token = token,
                    User = new UserDTO
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        UserType = user.UserType,
                        UniversityId = user.UniversityId
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi đăng ký: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto)
        {
            try
            {
                var user = await _unitOfWork.Users.AuthenticateAsync(loginDto.Username, loginDto.Password);
                if (user == null)
                {
                    return new AuthResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Tên đăng nhập hoặc mật khẩu không đúng"
                    };
                }

                // Tạo token
                var token = GenerateJwtToken(user);

                return new AuthResponseDTO
                {
                    IsSuccess = true,
                    Message = "Đăng nhập thành công",
                    Token = token,
                    User = new UserDTO
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        UserType = user.UserType,
                        UniversityId = user.UniversityId
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi đăng nhập: {ex.Message}"
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
            var jwtKey = _configuration["Jwt:Key"] ?? "ThisIsADefaultSecretKeyForJWTAuthentication12345";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim("id", user.Id.ToString()),
                new Claim("userType", user.UserType),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var issuer = _configuration["Jwt:Issuer"] ?? "AdmissionInfoSystem";
            var audience = _configuration["Jwt:Audience"] ?? "AdmissionInfoSystemClient";

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
} 