using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Services
{
    public interface IMajorService
    {
        Task<IEnumerable<Major>> GetAllAsync();
        Task<Major> GetByIdAsync(int id);
        Task<IEnumerable<Major>> GetByUniversityIdAsync(int universityId);
        Task<Major> CreateAsync(Major major);
        Task<Major> UpdateAsync(Major major);
        Task DeleteAsync(int id);
    }
} 