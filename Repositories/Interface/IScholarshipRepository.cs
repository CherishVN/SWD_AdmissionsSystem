using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Repositories
{
    public interface IScholarshipRepository : IRepository<Scholarship>
    {
        Task<IEnumerable<Scholarship>> GetByUniversityIdAsync(int universityId);
    }
} 