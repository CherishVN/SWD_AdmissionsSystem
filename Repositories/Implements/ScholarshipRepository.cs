using AdmissionInfoSystem.Data;
using AdmissionInfoSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AdmissionInfoSystem.Repositories
{
    public class ScholarshipRepository : Repository<Scholarship>, IScholarshipRepository
    {
        private readonly ApplicationDbContext _context;

        public ScholarshipRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Scholarship>> GetByUniversityIdAsync(int universityId)
        {
            return await _context.Scholarships
                .Where(s => s.UniversityId == universityId)
                .ToListAsync();
        }
    }
} 