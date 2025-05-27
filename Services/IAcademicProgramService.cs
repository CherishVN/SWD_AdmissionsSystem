using AdmissionInfoSystem.Models;

namespace AdmissionInfoSystem.Services
{
    public interface IAcademicProgramService
    {
        Task<IEnumerable<AcademicProgram>> GetAllAsync();
        Task<AcademicProgram> GetByIdAsync(int id);
        Task<IEnumerable<AcademicProgram>> GetByUniversityIdAsync(int universityId);
        Task<AcademicProgram> CreateAsync(AcademicProgram program);
        Task<AcademicProgram> UpdateAsync(AcademicProgram program);
        Task DeleteAsync(int id);
    }
} 