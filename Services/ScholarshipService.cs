using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Repositories;

namespace AdmissionInfoSystem.Services
{
    public class ScholarshipService : IScholarshipService
    {
        private readonly IScholarshipRepository _scholarshipRepository;

        public ScholarshipService(IScholarshipRepository scholarshipRepository)
        {
            _scholarshipRepository = scholarshipRepository;
        }

        public async Task<Scholarship> CreateAsync(Scholarship scholarship)
        {
            await _scholarshipRepository.AddAsync(scholarship);
            return scholarship;
        }

        public async Task DeleteAsync(int id)
        {
            var scholarship = await _scholarshipRepository.GetByIdAsync(id);
            if (scholarship != null)
            {
                await _scholarshipRepository.RemoveAsync(scholarship);
            }
        }

        public async Task<IEnumerable<Scholarship>> GetAllAsync()
        {
            return await _scholarshipRepository.GetAllAsync();
        }

        public async Task<Scholarship> GetByIdAsync(int id)
        {
            return await _scholarshipRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Scholarship>> GetByUniversityIdAsync(int universityId)
        {
            return await _scholarshipRepository.GetByUniversityIdAsync(universityId);
        }

        public async Task<Scholarship> UpdateAsync(Scholarship scholarship)
        {
            await _scholarshipRepository.UpdateAsync(scholarship);
            return scholarship;
        }
    }
} 