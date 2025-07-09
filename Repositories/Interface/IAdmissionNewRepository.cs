using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Repositories
{
    public interface IAdmissionNewRepository : IRepository<AdmissionNew>
    {
        Task<IEnumerable<AdmissionNew>> GetAdmissionNewsByUniversityAsync(int universityId);
        Task<IEnumerable<AdmissionNew>> GetLatestAdmissionNewsAsync(int count);
        Task<(IEnumerable<AdmissionNew> items, int totalCount)> GetPagedAdmissionNewsAsync(int page, int pageSize);
    }
} 