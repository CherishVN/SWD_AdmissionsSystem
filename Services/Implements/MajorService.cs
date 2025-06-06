using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Repositories;

namespace AdmissionInfoSystem.Services
{
    public class MajorService : IMajorService
    {
        private readonly IMajorRepository _majorRepository;

        public MajorService(IMajorRepository majorRepository)
        {
            _majorRepository = majorRepository;
        }

        public async Task<Major> CreateAsync(Major major)
        {
            await _majorRepository.AddAsync(major);
            return major;
        }

        public async Task DeleteAsync(int id)
        {
            var major = await _majorRepository.GetByIdAsync(id);
            if (major != null)
            {
                await _majorRepository.RemoveAsync(major);
            }
        }

        public async Task<IEnumerable<Major>> GetAllAsync()
        {
            return await _majorRepository.GetAllAsync();
        }

        public async Task<Major> GetByIdAsync(int id)
        {
            return await _majorRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Major>> GetByUniversityIdAsync(int universityId)
        {
            return await _majorRepository.GetByUniversityIdAsync(universityId);
        }

        public async Task<Major> UpdateAsync(Major major)
        {
            await _majorRepository.UpdateAsync(major);
            return major;
        }
    }
} 