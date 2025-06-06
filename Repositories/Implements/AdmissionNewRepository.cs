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
    }
} 