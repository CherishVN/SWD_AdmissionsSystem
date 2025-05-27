using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Services
{
    public interface IAdmissionCriteriaService
    {
        Task<IEnumerable<AdmissionCriteria>> GetAllAsync();
        Task<AdmissionCriteria> GetByIdAsync(int id);
        Task<IEnumerable<AdmissionCriteria>> GetByAdmissionMethodIdAsync(int admissionMethodId);
        Task<AdmissionCriteria> CreateAsync(AdmissionCriteria criteria);
        Task<AdmissionCriteria> UpdateAsync(AdmissionCriteria criteria);
        Task DeleteAsync(int id);
    }
} 