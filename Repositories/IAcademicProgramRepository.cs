using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Repositories
{
    public interface IAcademicProgramRepository : IRepository<AcademicProgram>
    {
        Task<IEnumerable<AcademicProgram>> GetByUniversityIdAsync(int universityId);
    }
} 