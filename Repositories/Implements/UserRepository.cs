using AdmissionInfoSystem.Data;
using AdmissionInfoSystem.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace AdmissionInfoSystem.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _db;

        public UserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _db.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _db.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return null;

            try
            {
                // Sử dụng BCrypt để kiểm tra mật khẩu
                bool verified = BCrypt.Net.BCrypt.Verify(password, user.Password);
                if (verified)
                    return user;
            }
            catch (SaltParseException)
            {
                // Nếu mật khẩu không phải định dạng BCrypt (từ dữ liệu cũ), so sánh trực tiếp
                if (user.Password == password)
                {
                    // Cập nhật mật khẩu sang định dạng BCrypt cho lần đăng nhập sau
                    user.Password = BCrypt.Net.BCrypt.HashPassword(password);
                    await _db.SaveChangesAsync();
                    return user;
                }
            }

            return null;
        }
    }
} 