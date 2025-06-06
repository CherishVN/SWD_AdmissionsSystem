using AdmissionInfoSystem.Data;
using AdmissionInfoSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AdmissionInfoSystem.Repositories
{
    public class UniversityRepository : Repository<University>, IUniversityRepository
    {
        private readonly ApplicationDbContext _db;

        public UniversityRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<University>> GetUniversitiesWithDetailsAsync()
        {
            return await _db.Universities
                .Include(u => u.AdmissionNews)
                .Include(u => u.AdmissionMethods)
                .Include(u => u.Programs)
                .Include(u => u.Majors)
                .Include(u => u.Scholarships)
                .ToListAsync();
        }

        public async Task<University> GetUniversityWithDetailsAsync(int id)
        {
            return await _db.Universities
                .Include(u => u.AdmissionNews)
                .Include(u => u.AdmissionMethods)
                .ThenInclude(am => am.AdmissionCriterias)
                .Include(u => u.Programs)
                .Include(u => u.Majors)
                .Include(u => u.Scholarships)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
} 