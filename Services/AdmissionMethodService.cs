using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Repositories;

namespace AdmissionInfoSystem.Services
{
    public class AdmissionMethodService : IAdmissionMethodService
    {
        private readonly IAdmissionMethodRepository _methodRepository;

        public AdmissionMethodService(IAdmissionMethodRepository methodRepository)
        {
            _methodRepository = methodRepository;
        }

        public async Task<AdmissionMethod> CreateAsync(AdmissionMethod method)
        {
            await _methodRepository.AddAsync(method);
            return method;
        }

        public async Task DeleteAsync(int id)
        {
            var method = await _methodRepository.GetByIdAsync(id);
            if (method != null)
            {
                await _methodRepository.RemoveAsync(method);
            }
        }

        public async Task<IEnumerable<AdmissionMethod>> GetAllAsync()
        {
            return await _methodRepository.GetAllAsync();
        }

        public async Task<AdmissionMethod> GetByIdAsync(int id)
        {
            return await _methodRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<AdmissionMethod>> GetByUniversityIdAsync(int universityId)
        {
            return await _methodRepository.GetAdmissionMethodsByUniversityAsync(universityId);
        }

        public async Task<AdmissionMethod> UpdateAsync(AdmissionMethod method)
        {
            await _methodRepository.UpdateAsync(method);
            return method;
        }
    }
} 