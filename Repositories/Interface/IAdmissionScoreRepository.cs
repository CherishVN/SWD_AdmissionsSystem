using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Repositories.Interface
{
    public interface IAdmissionScoreRepository : IRepository<AdmissionScore>
    {
        Task<IEnumerable<AdmissionScore>> GetByMajorIdAsync(int majorId);
        Task<IEnumerable<AdmissionScore>> GetByYearAsync(int year);
        Task<IEnumerable<AdmissionScore>> GetByMajorAndYearAsync(int majorId, int year);
        Task<AdmissionScore?> GetByMajorYearAndMethodAsync(int majorId, int year, int? admissionMethodId);
        Task<AdmissionScore?> GetByMajorYearAndMethodAsyncNoTracking(int majorId, int year, int? admissionMethodId);
        Task<(IEnumerable<AdmissionScore> Data, int TotalCount)> GetPagedAsync(int page, int pageSize);
        Task<(IEnumerable<AdmissionScore> Data, int TotalCount)> GetPagedByYearAsync(int year, int page, int pageSize);
        Task<(IEnumerable<AdmissionScore> Data, int TotalCount)> GetPagedByMajorIdAsync(int majorId, int page, int pageSize);
        Task<(IEnumerable<AdmissionScore> Data, int TotalCount)> GetPagedByUniversityIdAsync(int universityId, int page, int pageSize);
        Task<IEnumerable<AdmissionScore>> GetByUniversityIdAsync(int universityId);
    }
} 