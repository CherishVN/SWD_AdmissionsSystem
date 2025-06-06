using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Services
{
    public interface IAdmissionNewService
    {
        Task<IEnumerable<AdmissionNew>> GetAllAdmissionNewsAsync();
        Task<AdmissionNew> GetAdmissionNewByIdAsync(int id);
        Task<IEnumerable<AdmissionNew>> GetAdmissionNewsByUniversityAsync(int universityId);
        Task<IEnumerable<AdmissionNew>> GetLatestAdmissionNewsAsync(int count);
        Task<AdmissionNew> CreateAdmissionNewAsync(AdmissionNew admissionNew);
        Task<AdmissionNew> UpdateAdmissionNewAsync(AdmissionNew admissionNew);
        Task DeleteAdmissionNewAsync(int id);
    }
} 