using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Repositories;
using AdmissionInfoSystem.DTOs;
using AdmissionInfoSystem.Services.Interface;

namespace AdmissionInfoSystem.Services
{
    public class MajorService : IMajorService
    {
        private readonly IMajorRepository _majorRepository;
        private readonly IAdmissionScoreService _admissionScoreService;

        public MajorService(IMajorRepository majorRepository, IAdmissionScoreService admissionScoreService)
        {
            _majorRepository = majorRepository;
            _admissionScoreService = admissionScoreService;
        }

        public async Task<Major> CreateAsync(Major major)
        {
            await _majorRepository.AddAsync(major);
            return major;
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var major = await _majorRepository.GetByIdAsync(id);
                if (major == null)
                {
                    return; // Major not found, nothing to delete
                }

                // Check if there are related AdmissionScores
                var relatedScores = await _admissionScoreService.GetByMajorIdAsync(id);
                if (relatedScores.Any())
                {
                    // Delete all related AdmissionScores first
                    foreach (var score in relatedScores)
                    {
                        await _admissionScoreService.DeleteAsync(score.Id);
                    }
                }

                // Now delete the major
                await _majorRepository.RemoveAsync(major);
            }
            catch (Exception ex)
            {
                // Log the error and rethrow with more context
                throw new InvalidOperationException($"Không thể xóa ngành học. Lỗi: {ex.Message}", ex);
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