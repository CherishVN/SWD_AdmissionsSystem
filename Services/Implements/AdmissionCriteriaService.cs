using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Repositories;

namespace AdmissionInfoSystem.Services
{
    public class AdmissionCriteriaService : IAdmissionCriteriaService
    {
        private readonly IAdmissionCriteriaRepository _criteriaRepository;

        public AdmissionCriteriaService(IAdmissionCriteriaRepository criteriaRepository)
        {
            _criteriaRepository = criteriaRepository;
        }

        public async Task<AdmissionCriteria> CreateAsync(AdmissionCriteria criteria)
        {
            await _criteriaRepository.AddAsync(criteria);
            return criteria;
        }

        public async Task DeleteAsync(int id)
        {
            var criteria = await _criteriaRepository.GetByIdAsync(id);
            if (criteria != null)
            {
                await _criteriaRepository.RemoveAsync(criteria);
            }
        }

        public async Task<IEnumerable<AdmissionCriteria>> GetAllAsync()
        {
            return await _criteriaRepository.GetAllAsync();
        }

        public async Task<IEnumerable<AdmissionCriteria>> GetByAdmissionMethodIdAsync(int admissionMethodId)
        {
            return await _criteriaRepository.GetByAdmissionMethodIdAsync(admissionMethodId);
        }

        public async Task<AdmissionCriteria> GetByIdAsync(int id)
        {
            return await _criteriaRepository.GetByIdAsync(id);
        }

        public async Task<AdmissionCriteria> UpdateAsync(AdmissionCriteria criteria)
        {
            await _criteriaRepository.UpdateAsync(criteria);
            return criteria;
        }
    }
} 