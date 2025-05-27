using AdmissionInfoSystem.Data;
using AdmissionInfoSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AdmissionInfoSystem.Repositories
{
    public class MajorRepository : Repository<Major>, IMajorRepository
    {
        private readonly ApplicationDbContext _context;

        public MajorRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Major>> GetByUniversityIdAsync(int universityId)
        {
            return await _context.Majors
                .Where(m => m.UniversityId == universityId)
                .ToListAsync();
        }
    }
} 