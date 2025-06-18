using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Repositories.Interface
{
    public interface IAdmissionScoreRepository : IRepository<AdmissionScore>
    {
        Task<IEnumerable<AdmissionScore>> GetByMajorIdAsync(int majorId);
        Task<IEnumerable<AdmissionScore>> GetByYearAsync(int year);
        Task<IEnumerable<AdmissionScore>> GetByMajorAndYearAsync(int majorId, int year);
        Task<AdmissionScore?> GetByMajorYearAndMethodAsync(int majorId, int year, int? admissionMethodId);
    }
} 