using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByFirebaseUidAsync(string firebaseUid);
        Task<User?> AuthenticateAsync(string usernameOrEmail, string password);
    }
} 