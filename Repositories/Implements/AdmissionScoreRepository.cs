using AdmissionInfoSystem.Data;
using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace AdmissionInfoSystem.Repositories.Implements
{
    public class AdmissionScoreRepository : Repository<AdmissionScore>, IAdmissionScoreRepository
    {
        private readonly ApplicationDbContext _context;

        public AdmissionScoreRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdmissionScore>> GetByMajorIdAsync(int majorId)
        {
            return await _context.AdmissionScores
                .Include(ads => ads.Major)
                .ThenInclude(m => m.University)
                .Include(ads => ads.AdmissionMethod)
                .Where(ads => ads.MajorId == majorId)
                .OrderByDescending(ads => ads.Year)
                .ToListAsync();
        }

        public async Task<IEnumerable<AdmissionScore>> GetByYearAsync(int year)
        {
            return await _context.AdmissionScores
                .Include(ads => ads.Major)
                .ThenInclude(m => m.University)
                .Include(ads => ads.AdmissionMethod)
                .Where(ads => ads.Year == year)
                .OrderBy(ads => ads.Major.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<AdmissionScore>> GetByMajorAndYearAsync(int majorId, int year)
        {
            return await _context.AdmissionScores
                .Include(ads => ads.Major)
                .ThenInclude(m => m.University)
                .Include(ads => ads.AdmissionMethod)
                .Where(ads => ads.MajorId == majorId && ads.Year == year)
                .ToListAsync();
        }

        public async Task<AdmissionScore?> GetByMajorYearAndMethodAsync(int majorId, int year, int? admissionMethodId)
        {
            return await _context.AdmissionScores
                .Include(ads => ads.Major)
                .ThenInclude(m => m.University)
                .Include(ads => ads.AdmissionMethod)
                .FirstOrDefaultAsync(ads => ads.MajorId == majorId && 
                                          ads.Year == year && 
                                          ads.AdmissionMethodId == admissionMethodId);
        }

        public new async Task<IEnumerable<AdmissionScore>> GetAllAsync()
        {
            return await _context.AdmissionScores
                .Include(ads => ads.Major)
                .ThenInclude(m => m.University)
                .Include(ads => ads.AdmissionMethod)
                .OrderByDescending(ads => ads.Year)
                .ThenBy(ads => ads.Major.Name)
                .ToListAsync();
        }

        public new async Task<AdmissionScore?> GetByIdAsync(int id)
        {
            return await _context.AdmissionScores
                .Include(ads => ads.Major)
                .ThenInclude(m => m.University)
                .Include(ads => ads.AdmissionMethod)
                .FirstOrDefaultAsync(ads => ads.Id == id);
        }
    }
} 