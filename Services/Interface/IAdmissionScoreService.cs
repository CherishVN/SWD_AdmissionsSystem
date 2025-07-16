using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.DTOs;

namespace AdmissionInfoSystem.Services.Interface
{
    public interface IAdmissionScoreService
    {
        Task<IEnumerable<AdmissionScore>> GetAllAsync();
        Task<AdmissionScore?> GetByIdAsync(int id);
        Task<IEnumerable<AdmissionScore>> GetByMajorIdAsync(int majorId);
        Task<IEnumerable<AdmissionScore>> GetByYearAsync(int year);
        Task<IEnumerable<AdmissionScore>> GetByMajorAndYearAsync(int majorId, int year);
        Task<AdmissionScore?> GetByMajorYearAndMethodAsync(int majorId, int year, int? admissionMethodId);
        Task<PagedAdmissionScoreDTO> GetPagedAsync(int page, int pageSize);
        Task<PagedAdmissionScoreDTO> GetPagedByYearAsync(int year, int page, int pageSize);
        Task<PagedAdmissionScoreDTO> GetPagedByMajorIdAsync(int majorId, int page, int pageSize);
        Task<PagedAdmissionScoreDTO> GetPagedByUniversityIdAsync(int universityId, int page, int pageSize);
        Task<AdmissionScore> CreateAsync(AdmissionScore admissionScore);
        Task<AdmissionScore> UpdateAsync(AdmissionScore admissionScore);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
} 