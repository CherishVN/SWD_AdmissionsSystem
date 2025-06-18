using AdmissionInfoSystem.Data;
using AdmissionInfoSystem.Repositories.Implements;
using AdmissionInfoSystem.Repositories.Interface;

namespace AdmissionInfoSystem.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        
        public IUserRepository Users { get; private set; }
        public IUniversityRepository Universities { get; private set; }
        public IAdmissionNewRepository AdmissionNews { get; private set; }
        public IAdmissionMethodRepository AdmissionMethods { get; private set; }
        public IAdmissionCriteriaRepository AdmissionCriterias { get; private set; }
        public IAcademicProgramRepository AcademicPrograms { get; private set; }
        public IMajorRepository Majors { get; private set; }
        public IScholarshipRepository Scholarships { get; private set; }
        public IAdmissionScoreRepository AdmissionScores { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Users = new UserRepository(db);
            Universities = new UniversityRepository(db);
            AdmissionNews = new AdmissionNewRepository(db);
            AdmissionMethods = new AdmissionMethodRepository(db);
            AdmissionCriterias = new AdmissionCriteriaRepository(db);
            AcademicPrograms = new AcademicProgramRepository(db);
            Majors = new MajorRepository(db);
            Scholarships = new ScholarshipRepository(db);
            AdmissionScores = new AdmissionScoreRepository(db);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
} 