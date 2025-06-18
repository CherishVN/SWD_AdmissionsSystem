using AdmissionInfoSystem.Models;
using System.Security.Claims;

namespace AdmissionInfoSystem.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        ClaimsPrincipal? ValidateToken(string token);
    }
} 