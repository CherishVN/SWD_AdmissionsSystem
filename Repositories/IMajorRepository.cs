using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Repositories
{
    public interface IMajorRepository : IRepository<Major>
    {
        Task<IEnumerable<Major>> GetByUniversityIdAsync(int universityId);
    }
} 