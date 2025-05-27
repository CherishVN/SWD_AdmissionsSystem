using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Repositories
{
    public interface IAdmissionCriteriaRepository : IRepository<AdmissionCriteria>
    {
        Task<IEnumerable<AdmissionCriteria>> GetByAdmissionMethodIdAsync(int admissionMethodId);
    }
} 