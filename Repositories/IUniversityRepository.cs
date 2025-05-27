using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Repositories
{
    public interface IUniversityRepository : IRepository<University>
    {
        Task<IEnumerable<University>> GetUniversitiesWithDetailsAsync();
        Task<University> GetUniversityWithDetailsAsync(int id);
    }
} 