using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Repositories;
using AdmissionInfoSystem.DTOs;

namespace AdmissionInfoSystem.Services
{
    public class MajorService : IMajorService
    {
        private readonly IMajorRepository _majorRepository;

        public MajorService(IMajorRepository majorRepository)
        {
            _majorRepository = majorRepository;
        }

        public async Task<Major> CreateAsync(Major major)
        {
            await _majorRepository.AddAsync(major);
            return major;
        }

        public async Task DeleteAsync(int id)
        {
            var major = await _majorRepository.GetByIdAsync(id);
            if (major != null)
            {
                await _majorRepository.RemoveAsync(major);
            }
        }

        public async Task<IEnumerable<Major>> GetAllAsync()
        {
            return await _majorRepository.GetAllAsync();
        }

        public async Task<Major> GetByIdAsync(int id)
        {
            return await _majorRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Major>> GetByUniversityIdAsync(int universityId)
        {
            return await _majorRepository.GetByUniversityIdAsync(universityId);
        }

        public async Task<PagedMajorDTO> GetPagedAsync(int page, int pageSize)
        {
            var (data, totalCount) = await _majorRepository.GetPagedAsync(page, pageSize);
            
            var majorDtos = data.Select(m => new MajorDTO
            {
                Id = m.Id,
                Name = m.Name,
                Code = m.Code,
                Description = m.Description,
                AdmissionScore = m.AdmissionScore,
                Year = m.Year,
                UniversityId = m.UniversityId,
                ProgramId = m.ProgramId,
                UniversityName = m.University?.Name,
                ProgramName = m.Program?.Name
            });

            return new PagedMajorDTO
            {
                Data = majorDtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<PagedMajorDTO> GetPagedByUniversityIdAsync(int universityId, int page, int pageSize)
        {
            var (data, totalCount) = await _majorRepository.GetPagedByUniversityIdAsync(universityId, page, pageSize);
            
            var majorDtos = data.Select(m => new MajorDTO
            {
                Id = m.Id,
                Name = m.Name,
                Code = m.Code,
                Description = m.Description,
                AdmissionScore = m.AdmissionScore,
                Year = m.Year,
                UniversityId = m.UniversityId,
                ProgramId = m.ProgramId,
                UniversityName = m.University?.Name,
                ProgramName = m.Program?.Name
            });

            return new PagedMajorDTO
            {
                Data = majorDtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<Major> UpdateAsync(Major major)
        {
            await _majorRepository.UpdateAsync(major);
            return major;
        }
    }
} 