using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> AuthenticateAsync(string usernameOrEmail, string password);
    }
} 