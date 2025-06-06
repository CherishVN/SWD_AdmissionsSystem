using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Services
{
    public interface IUniversityService
    {
        Task<IEnumerable<University>> GetAllUniversitiesAsync();
        Task<University> GetUniversityByIdAsync(int id);
        Task<University> GetUniversityWithDetailsAsync(int id);
        Task<IEnumerable<University>> GetUniversitiesWithDetailsAsync();
        Task<University> CreateUniversityAsync(University university);
        Task<University> UpdateUniversityAsync(University university);
        Task DeleteUniversityAsync(int id);
    }
} 