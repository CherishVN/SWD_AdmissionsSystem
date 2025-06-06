using AdmissionInfoSystem.Data;
using AdmissionInfoSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AdmissionInfoSystem.Repositories
{
    public class AcademicProgramRepository : Repository<AcademicProgram>, IAcademicProgramRepository
    {
        private readonly ApplicationDbContext _context;

        public AcademicProgramRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AcademicProgram>> GetByUniversityIdAsync(int universityId)
        {
            return await _context.Programs
                .Where(p => p.UniversityId == universityId)
                .ToListAsync();
        }
    }
} 