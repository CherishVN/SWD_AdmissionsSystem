using AdmissionInfoSystem.Data;
using AdmissionInfoSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AdmissionInfoSystem.Repositories
{
    public class AdmissionMethodRepository : Repository<AdmissionMethod>, IAdmissionMethodRepository
    {
        private readonly ApplicationDbContext _db;

        public AdmissionMethodRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<AdmissionMethod>> GetAdmissionMethodsByUniversityAsync(int universityId)
        {
            return await _db.AdmissionMethods
                .Include(am => am.University)
                .Include(am => am.AdmissionCriterias)
                .Where(am => am.UniversityId == universityId)
                .ToListAsync();
        }

        public async Task<AdmissionMethod> GetAdmissionMethodWithCriteriasAsync(int id)
        {
            return await _db.AdmissionMethods
                .Include(am => am.University)
                .Include(am => am.AdmissionCriterias)
                .FirstOrDefaultAsync(am => am.Id == id);
        }
    }
} 