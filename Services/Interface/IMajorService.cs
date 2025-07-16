using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.DTOs;

namespace AdmissionInfoSystem.Services
{
    public interface IMajorService
    {
        Task<IEnumerable<Major>> GetAllAsync();
        Task<Major> GetByIdAsync(int id);
        Task<IEnumerable<Major>> GetByUniversityIdAsync(int universityId);
        Task<PagedMajorDTO> GetPagedAsync(int page, int pageSize);
        Task<PagedMajorDTO> GetPagedByUniversityIdAsync(int universityId, int page, int pageSize);
        Task<Major> CreateAsync(Major major);
        Task<Major> UpdateAsync(Major major);
        Task DeleteAsync(int id);
    }
} 