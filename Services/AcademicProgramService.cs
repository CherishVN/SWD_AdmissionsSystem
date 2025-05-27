using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Repositories;

namespace AdmissionInfoSystem.Services
{
    public class AcademicProgramService : IAcademicProgramService
    {
        private readonly IAcademicProgramRepository _programRepository;

        public AcademicProgramService(IAcademicProgramRepository programRepository)
        {
            _programRepository = programRepository;
        }

        public async Task<AcademicProgram> CreateAsync(AcademicProgram program)
        {
            await _programRepository.AddAsync(program);
            return program;
        }

        public async Task DeleteAsync(int id)
        {
            var program = await _programRepository.GetByIdAsync(id);
            if (program != null)
            {
                await _programRepository.RemoveAsync(program);
            }
        }

        public async Task<IEnumerable<AcademicProgram>> GetAllAsync()
        {
            return await _programRepository.GetAllAsync();
        }

        public async Task<AcademicProgram> GetByIdAsync(int id)
        {
            return await _programRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<AcademicProgram>> GetByUniversityIdAsync(int universityId)
        {
            return await _programRepository.GetByUniversityIdAsync(universityId);
        }

        public async Task<AcademicProgram> UpdateAsync(AcademicProgram program)
        {
            await _programRepository.UpdateAsync(program);
            return program;
        }
    }
} 