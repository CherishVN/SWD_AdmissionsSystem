using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Repositories
{
    public interface IAdmissionMethodRepository : IRepository<AdmissionMethod>
    {
        Task<IEnumerable<AdmissionMethod>> GetAdmissionMethodsByUniversityAsync(int universityId);
        Task<AdmissionMethod> GetAdmissionMethodWithCriteriasAsync(int id);
    }
} 