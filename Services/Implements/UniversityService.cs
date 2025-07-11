using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Repositories;

namespace AdmissionInfoSystem.Services
{
    public class UniversityService : IUniversityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UniversityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<University> CreateUniversityAsync(University university)
        {
            await _unitOfWork.Universities.AddAsync(university);
            await _unitOfWork.SaveChangesAsync();
            return university;
        }

        public async Task DeleteUniversityAsync(int id)
        {
            await _unitOfWork.Universities.RemoveAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<University>> GetAllUniversitiesAsync()
        {
            return await _unitOfWork.Universities.GetAllAsync();
        }

        public async Task<University> GetUniversityByIdAsync(int id)
        {
            return await _unitOfWork.Universities.GetByIdAsync(id);
        }

        public async Task<IEnumerable<University>> GetUniversitiesWithDetailsAsync()
        {
            return await _unitOfWork.Universities.GetUniversitiesWithDetailsAsync();
        }

        public async Task<University> GetUniversityWithDetailsAsync(int id)
        {
            return await _unitOfWork.Universities.GetUniversityWithDetailsAsync(id);
        }

        public async Task<University> UpdateUniversityAsync(University university)
        {
            await _unitOfWork.Universities.UpdateAsync(university);
            await _unitOfWork.SaveChangesAsync();
            return university;
        }
    }
} 