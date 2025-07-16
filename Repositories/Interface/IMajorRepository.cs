using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Repositories
{
    public interface IMajorRepository : IRepository<Major>
    {
        Task<IEnumerable<Major>> GetByUniversityIdAsync(int universityId);
        Task<(IEnumerable<Major> Data, int TotalCount)> GetPagedAsync(int page, int pageSize);
        Task<(IEnumerable<Major> Data, int TotalCount)> GetPagedByUniversityIdAsync(int universityId, int page, int pageSize);
    }
} 