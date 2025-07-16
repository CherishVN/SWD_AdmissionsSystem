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
                .Include(m => m.University)
                .Where(m => m.UniversityId == universityId)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Major> Data, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            var totalCount = await _context.Majors.CountAsync();
            
            var data = await _context.Majors
                .Include(m => m.University)
                .Include(m => m.Program)
                .OrderBy(m => m.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        public async Task<(IEnumerable<Major> Data, int TotalCount)> GetPagedByUniversityIdAsync(int universityId, int page, int pageSize)
        {
            var totalCount = await _context.Majors.Where(m => m.UniversityId == universityId).CountAsync();
            
            var data = await _context.Majors
                .Include(m => m.University)
                .Include(m => m.Program)
                .Where(m => m.UniversityId == universityId)
                .OrderBy(m => m.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        public new async Task<IEnumerable<Major>> GetAllAsync()
        {
            return await _context.Majors
                .Include(m => m.University)
                .ToListAsync();
        }

        public new async Task<Major?> GetByIdAsync(int id)
        {
            return await _context.Majors
                .Include(m => m.University)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
} 