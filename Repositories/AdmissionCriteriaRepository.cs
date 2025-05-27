using AdmissionInfoSystem.Data;
using AdmissionInfoSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AdmissionInfoSystem.Repositories
{
    public class AdmissionCriteriaRepository : Repository<AdmissionCriteria>, IAdmissionCriteriaRepository
    {
        private readonly ApplicationDbContext _context;

        public AdmissionCriteriaRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdmissionCriteria>> GetByAdmissionMethodIdAsync(int admissionMethodId)
        {
            return await _context.AdmissionCriterias
                .Where(ac => ac.AdmissionMethodId == admissionMethodId)
                .ToListAsync();
        }
    }
} 