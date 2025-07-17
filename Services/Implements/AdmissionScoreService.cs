using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Repositories;
using AdmissionInfoSystem.Services.Interface;
using AdmissionInfoSystem.DTOs;

namespace AdmissionInfoSystem.Services.Implements
{
    public class AdmissionScoreService : IAdmissionScoreService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdmissionScoreService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AdmissionScore>> GetAllAsync()
        {
            return await _unitOfWork.AdmissionScores.GetAllAsync();
        }

        public async Task<AdmissionScore?> GetByIdAsync(int id)
        {
            return await _unitOfWork.AdmissionScores.GetByIdAsync(id);
        }

        public async Task<IEnumerable<AdmissionScore>> GetByMajorIdAsync(int majorId)
        {
            return await _unitOfWork.AdmissionScores.GetByMajorIdAsync(majorId);
        }

        public async Task<IEnumerable<AdmissionScore>> GetByYearAsync(int year)
        {
            return await _unitOfWork.AdmissionScores.GetByYearAsync(year);
        }

        public async Task<IEnumerable<AdmissionScore>> GetByMajorAndYearAsync(int majorId, int year)
        {
            return await _unitOfWork.AdmissionScores.GetByMajorAndYearAsync(majorId, year);
        }

        public async Task<IEnumerable<AdmissionScore>> GetByUniversityIdAsync(int universityId)
        {
            return await _unitOfWork.AdmissionScores.GetByUniversityIdAsync(universityId);
        }

        public async Task<AdmissionScore?> GetByMajorYearAndMethodAsync(int majorId, int year, int? admissionMethodId)
        {
            return await _unitOfWork.AdmissionScores.GetByMajorYearAndMethodAsync(majorId, year, admissionMethodId);
        }

        public async Task<PagedAdmissionScoreDTO> GetPagedAsync(int page, int pageSize)
        {
            var (data, totalCount) = await _unitOfWork.AdmissionScores.GetPagedAsync(page, pageSize);
            
            var scoreDtos = data.Select(s => new AdmissionScoreDTO
            {
                Id = s.Id,
                MajorId = s.MajorId,
                Year = s.Year,
                Score = s.Score,
                AdmissionMethodId = s.AdmissionMethodId,
                Note = s.Note,
                SubjectCombination = s.SubjectCombination,
                MajorName = s.Major?.Name,
                UniversityName = s.Major?.University?.Name,
                AdmissionMethodName = s.AdmissionMethod?.Name
            });

            return new PagedAdmissionScoreDTO
            {
                Data = scoreDtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<PagedAdmissionScoreDTO> GetPagedByYearAsync(int year, int page, int pageSize)
        {
            var (data, totalCount) = await _unitOfWork.AdmissionScores.GetPagedByYearAsync(year, page, pageSize);
            
            var scoreDtos = data.Select(s => new AdmissionScoreDTO
            {
                Id = s.Id,
                MajorId = s.MajorId,
                Year = s.Year,
                Score = s.Score,
                AdmissionMethodId = s.AdmissionMethodId,
                Note = s.Note,
                SubjectCombination = s.SubjectCombination,
                MajorName = s.Major?.Name,
                UniversityName = s.Major?.University?.Name,
                AdmissionMethodName = s.AdmissionMethod?.Name
            });

            return new PagedAdmissionScoreDTO
            {
                Data = scoreDtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<PagedAdmissionScoreDTO> GetPagedByMajorIdAsync(int majorId, int page, int pageSize)
        {
            var (data, totalCount) = await _unitOfWork.AdmissionScores.GetPagedByMajorIdAsync(majorId, page, pageSize);
            
            var scoreDtos = data.Select(s => new AdmissionScoreDTO
            {
                Id = s.Id,
                MajorId = s.MajorId,
                Year = s.Year,
                Score = s.Score,
                AdmissionMethodId = s.AdmissionMethodId,
                Note = s.Note,
                SubjectCombination = s.SubjectCombination,
                MajorName = s.Major?.Name,
                UniversityName = s.Major?.University?.Name,
                AdmissionMethodName = s.AdmissionMethod?.Name
            });

            return new PagedAdmissionScoreDTO
            {
                Data = scoreDtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<PagedAdmissionScoreDTO> GetPagedByUniversityIdAsync(int universityId, int page, int pageSize)
        {
            var (data, totalCount) = await _unitOfWork.AdmissionScores.GetPagedByUniversityIdAsync(universityId, page, pageSize);
            
            var scoreDtos = data.Select(s => new AdmissionScoreDTO
            {
                Id = s.Id,
                MajorId = s.MajorId,
                Year = s.Year,
                Score = s.Score,
                AdmissionMethodId = s.AdmissionMethodId,
                Note = s.Note,
                SubjectCombination = s.SubjectCombination,
                MajorName = s.Major?.Name,
                UniversityName = s.Major?.University?.Name,
                AdmissionMethodName = s.AdmissionMethod?.Name
            });

            return new PagedAdmissionScoreDTO
            {
                Data = scoreDtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<AdmissionScore> CreateAsync(AdmissionScore admissionScore)
        {
            // Kiểm tra xem Major có tồn tại không
            var major = await _unitOfWork.Majors.GetByIdAsync(admissionScore.MajorId);
            if (major == null)
            {
                throw new ArgumentException("Major không tồn tại");
            }

            // Kiểm tra xem điểm chuẩn đã tồn tại cho ngành, năm và phương thức tuyển sinh này chưa
            var existingScore = await _unitOfWork.AdmissionScores.GetByMajorYearAndMethodAsync(
                admissionScore.MajorId, 
                admissionScore.Year, 
                admissionScore.AdmissionMethodId
            );

            if (existingScore != null)
            {
                throw new InvalidOperationException("Điểm chuẩn cho ngành này trong năm và phương thức tuyển sinh đã tồn tại");
            }

            await _unitOfWork.AdmissionScores.AddAsync(admissionScore);
            await _unitOfWork.SaveChangesAsync();

            return admissionScore;
        }

        public async Task<AdmissionScore> UpdateAsync(AdmissionScore admissionScore)
        {
            var existingScore = await _unitOfWork.AdmissionScores.GetByIdAsync(admissionScore.Id);
            if (existingScore == null)
            {
                throw new ArgumentException("Điểm chuẩn không tồn tại");
            }

            // Kiểm tra xem Major có tồn tại không
            var major = await _unitOfWork.Majors.GetByIdAsync(admissionScore.MajorId);
            if (major == null)
            {
                throw new ArgumentException("Major không tồn tại");
            }

            // Kiểm tra trùng lặp (trừ chính nó)
            var duplicateScore = await _unitOfWork.AdmissionScores.GetByMajorYearAndMethodAsync(
                admissionScore.MajorId, 
                admissionScore.Year, 
                admissionScore.AdmissionMethodId
            );

            if (duplicateScore != null && duplicateScore.Id != admissionScore.Id)
            {
                throw new InvalidOperationException("Điểm chuẩn cho ngành này trong năm và phương thức tuyển sinh đã tồn tại");
            }

            await _unitOfWork.AdmissionScores.UpdateAsync(admissionScore);
            await _unitOfWork.SaveChangesAsync();

            return admissionScore;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var admissionScore = await _unitOfWork.AdmissionScores.GetByIdAsync(id);
            if (admissionScore == null)
            {
                return false;
            }

            await _unitOfWork.AdmissionScores.RemoveAsync(admissionScore);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var admissionScore = await _unitOfWork.AdmissionScores.GetByIdAsync(id);
            return admissionScore != null;
        }
    }
} 