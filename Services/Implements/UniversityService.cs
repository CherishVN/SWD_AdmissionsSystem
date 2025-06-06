using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Repositories;

namespace AdmissionInfoSystem.Services
{
    public class UniversityService : IUniversityService
    {
        private readonly IUniversityRepository _universityRepository;

        public UniversityService(IUniversityRepository universityRepository)
        {
            _universityRepository = universityRepository;
        }

        public async Task<University> CreateUniversityAsync(University university)
        {
            await _universityRepository.AddAsync(university);
            return university;
        }

        public async Task DeleteUniversityAsync(int id)
        {
            await _universityRepository.RemoveAsync(id);
        }

        public async Task<IEnumerable<University>> GetAllUniversitiesAsync()
        {
            return await _universityRepository.GetAllAsync();
        }

        public async Task<University> GetUniversityByIdAsync(int id)
        {
            return await _universityRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<University>> GetUniversitiesWithDetailsAsync()
        {
            return await _universityRepository.GetUniversitiesWithDetailsAsync();
        }

        public async Task<University> GetUniversityWithDetailsAsync(int id)
        {
            return await _universityRepository.GetUniversityWithDetailsAsync(id);
        }

        public async Task<University> UpdateUniversityAsync(University university)
        {
            await _universityRepository.UpdateAsync(university);
            return university;
        }
    }
} 