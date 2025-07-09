using AdmissionInfoSystem.Data;
using AdmissionInfoSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AdmissionInfoSystem.Repositories
{
    public class AdmissionNewRepository : Repository<AdmissionNew>, IAdmissionNewRepository
    {
        private readonly ApplicationDbContext _db;

        public AdmissionNewRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public new async Task<IEnumerable<AdmissionNew>> GetAllAsync()
        {
            return await _db.AdmissionNews
                .Include(an => an.University)
                .OrderByDescending(an => an.PublishDate)
                .ToListAsync();
        }

        public new async Task<AdmissionNew> GetByIdAsync(int id)
        {
            return await _db.AdmissionNews
                .Include(an => an.University)
                .FirstOrDefaultAsync(an => an.Id == id);
        }

        public async Task<IEnumerable<AdmissionNew>> GetAdmissionNewsByUniversityAsync(int universityId)
        {
            return await _db.AdmissionNews
                .Include(an => an.University)
                .Where(an => an.UniversityId == universityId)
                .OrderByDescending(an => an.PublishDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<AdmissionNew>> GetLatestAdmissionNewsAsync(int count)
        {
            return await _db.AdmissionNews
                .Include(an => an.University)
                .OrderByDescending(an => an.PublishDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<(IEnumerable<AdmissionNew> items, int totalCount)> GetPagedAdmissionNewsAsync(int page, int pageSize)
        {
            var query = _db.AdmissionNews
                .Include(an => an.University)
                .OrderByDescending(an => an.PublishDate);

            var totalCount = await query.CountAsync();
            
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
} 