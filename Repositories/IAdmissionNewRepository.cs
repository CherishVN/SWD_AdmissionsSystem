using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Repositories
{
    public interface IAdmissionNewRepository : IRepository<AdmissionNew>
    {
        Task<IEnumerable<AdmissionNew>> GetAdmissionNewsByUniversityAsync(int universityId);
        Task<IEnumerable<AdmissionNew>> GetLatestAdmissionNewsAsync(int count);
    }
} 