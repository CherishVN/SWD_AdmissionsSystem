using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Repositories;

namespace AdmissionInfoSystem.Services
{
    public class AdmissionNewService : IAdmissionNewService
    {
        private readonly IAdmissionNewRepository _admissionNewRepository;

        public AdmissionNewService(IAdmissionNewRepository admissionNewRepository)
        {
            _admissionNewRepository = admissionNewRepository;
        }

        public async Task<AdmissionNew> CreateAdmissionNewAsync(AdmissionNew admissionNew)
        {
            await _admissionNewRepository.AddAsync(admissionNew);
            return admissionNew;
        }

        public async Task DeleteAdmissionNewAsync(int id)
        {
            await _admissionNewRepository.RemoveAsync(id);
        }

        public async Task<IEnumerable<AdmissionNew>> GetAllAdmissionNewsAsync()
        {
            return await _admissionNewRepository.GetAllAsync();
        }

        public async Task<AdmissionNew> GetAdmissionNewByIdAsync(int id)
        {
            return await _admissionNewRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<AdmissionNew>> GetAdmissionNewsByUniversityAsync(int universityId)
        {
            return await _admissionNewRepository.GetAdmissionNewsByUniversityAsync(universityId);
        }

        public async Task<IEnumerable<AdmissionNew>> GetLatestAdmissionNewsAsync(int count)
        {
            return await _admissionNewRepository.GetLatestAdmissionNewsAsync(count);
        }

        public async Task<AdmissionNew> UpdateAdmissionNewAsync(AdmissionNew admissionNew)
        {
            await _admissionNewRepository.UpdateAsync(admissionNew);
            return admissionNew;
        }
    }
} 