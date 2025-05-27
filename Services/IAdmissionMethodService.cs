using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Services
{
    public interface IAdmissionMethodService
    {
        Task<IEnumerable<AdmissionMethod>> GetAllAsync();
        Task<AdmissionMethod> GetByIdAsync(int id);
        Task<IEnumerable<AdmissionMethod>> GetByUniversityIdAsync(int universityId);
        Task<AdmissionMethod> CreateAsync(AdmissionMethod method);
        Task<AdmissionMethod> UpdateAsync(AdmissionMethod method);
        Task DeleteAsync(int id);
    }
} 