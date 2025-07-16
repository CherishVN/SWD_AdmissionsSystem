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

        public async Task<(IEnumerable<AdmissionScore> Data, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            var totalCount = await _context.AdmissionScores.CountAsync();
            
            var data = await _context.AdmissionScores
                .Include(ads => ads.Major)
                .ThenInclude(m => m.University)
                .Include(ads => ads.AdmissionMethod)
                .OrderByDescending(ads => ads.Year)
                .ThenBy(ads => ads.Major.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        public async Task<(IEnumerable<AdmissionScore> Data, int TotalCount)> GetPagedByYearAsync(int year, int page, int pageSize)
        {
            var totalCount = await _context.AdmissionScores.Where(ads => ads.Year == year).CountAsync();
            
            var data = await _context.AdmissionScores
                .Include(ads => ads.Major)
                .ThenInclude(m => m.University)
                .Include(ads => ads.AdmissionMethod)
                .Where(ads => ads.Year == year)
                .OrderBy(ads => ads.Major.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        public async Task<(IEnumerable<AdmissionScore> Data, int TotalCount)> GetPagedByMajorIdAsync(int majorId, int page, int pageSize)
        {
            var totalCount = await _context.AdmissionScores.Where(ads => ads.MajorId == majorId).CountAsync();
            
            var data = await _context.AdmissionScores
                .Include(ads => ads.Major)
                .ThenInclude(m => m.University)
                .Include(ads => ads.AdmissionMethod)
                .Where(ads => ads.MajorId == majorId)
                .OrderByDescending(ads => ads.Year)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        public async Task<(IEnumerable<AdmissionScore> Data, int TotalCount)> GetPagedByUniversityIdAsync(int universityId, int page, int pageSize)
        {
            var totalCount = await _context.AdmissionScores
                .Where(ads => ads.Major.UniversityId == universityId)
                .CountAsync();
            
            var data = await _context.AdmissionScores
                .Include(ads => ads.Major)
                .ThenInclude(m => m.University)
                .Include(ads => ads.AdmissionMethod)
                .Where(ads => ads.Major.UniversityId == universityId)
                .OrderByDescending(ads => ads.Year)
                .ThenBy(ads => ads.Major.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
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