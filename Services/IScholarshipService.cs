using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Services
{
    public interface IScholarshipService
    {
        Task<IEnumerable<Scholarship>> GetAllAsync();
        Task<Scholarship> GetByIdAsync(int id);
        Task<IEnumerable<Scholarship>> GetByUniversityIdAsync(int universityId);
        Task<Scholarship> CreateAsync(Scholarship scholarship);
        Task<Scholarship> UpdateAsync(Scholarship scholarship);
        Task DeleteAsync(int id);
    }
} 