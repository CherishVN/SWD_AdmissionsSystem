using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.DTOs;

namespace AdmissionInfoSystem.Services
{
    public interface IAdmissionNewService
    {
        Task<IEnumerable<AdmissionNew>> GetAllAdmissionNewsAsync();
        Task<AdmissionNew> GetAdmissionNewByIdAsync(int id);
        Task<IEnumerable<AdmissionNew>> GetAdmissionNewsByUniversityAsync(int universityId);
        Task<IEnumerable<AdmissionNew>> GetLatestAdmissionNewsAsync(int count);
        Task<PagedAdmissionNewsDTO> GetPagedAdmissionNewsAsync(int page, int pageSize);
        Task<AdmissionNew> CreateAdmissionNewAsync(AdmissionNew admissionNew);
        Task<AdmissionNew> UpdateAdmissionNewAsync(AdmissionNew admissionNew);
        Task DeleteAdmissionNewAsync(int id);
    }
} 