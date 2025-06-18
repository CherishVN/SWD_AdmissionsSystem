using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Repositories;
using AdmissionInfoSystem.Services.Interface;

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

        public async Task<AdmissionScore?> GetByMajorYearAndMethodAsync(int majorId, int year, int? admissionMethodId)
        {
            return await _unitOfWork.AdmissionScores.GetByMajorYearAndMethodAsync(majorId, year, admissionMethodId);
        }

        public async Task<AdmissionScore> CreateAsync(AdmissionScore admissionScore)
        {
            // Kiểm tra xem Major có tồn tại không
            var major = await _unitOfWork.Majors.GetByIdAsync(admissionScore.MajorId);
            if (major == null)
            {
                throw new ArgumentException("Major không tồn tại");
            }

            // Kiểm tra xem AdmissionMethod có tồn tại không (nếu có)
            if (admissionScore.AdmissionMethodId.HasValue)
            {
                var admissionMethod = await _unitOfWork.AdmissionMethods.GetByIdAsync(admissionScore.AdmissionMethodId.Value);
                if (admissionMethod == null)
                {
                    throw new ArgumentException("Admission Method không tồn tại");
                }
            }

            // Kiểm tra xem đã có điểm cho major, year và method này chưa
            var existingScore = await _unitOfWork.AdmissionScores
                .GetByMajorYearAndMethodAsync(admissionScore.MajorId, admissionScore.Year, admissionScore.AdmissionMethodId);
            
            if (existingScore != null)
            {
                throw new InvalidOperationException("Điểm chuẩn cho ngành, năm và phương thức tuyển sinh này đã tồn tại");
            }

            await _unitOfWork.AdmissionScores.AddAsync(admissionScore);
            await _unitOfWork.SaveChangesAsync();
            
            return await _unitOfWork.AdmissionScores.GetByIdAsync(admissionScore.Id) ?? admissionScore;
        }

        public async Task<AdmissionScore> UpdateAsync(AdmissionScore admissionScore)
        {
            var existingScore = await _unitOfWork.AdmissionScores.GetByIdAsync(admissionScore.Id);
            if (existingScore == null)
            {
                throw new ArgumentException("Admission Score không tồn tại");
            }

            // Kiểm tra xem Major có tồn tại không
            var major = await _unitOfWork.Majors.GetByIdAsync(admissionScore.MajorId);
            if (major == null)
            {
                throw new ArgumentException("Major không tồn tại");
            }

            // Kiểm tra xem AdmissionMethod có tồn tại không (nếu có)
            if (admissionScore.AdmissionMethodId.HasValue)
            {
                var admissionMethod = await _unitOfWork.AdmissionMethods.GetByIdAsync(admissionScore.AdmissionMethodId.Value);
                if (admissionMethod == null)
                {
                    throw new ArgumentException("Admission Method không tồn tại");
                }
            }

            // Kiểm tra xem có bản ghi trùng lặp khác không
            var duplicateScore = await _unitOfWork.AdmissionScores
                .GetByMajorYearAndMethodAsync(admissionScore.MajorId, admissionScore.Year, admissionScore.AdmissionMethodId);
            
            if (duplicateScore != null && duplicateScore.Id != admissionScore.Id)
            {
                throw new InvalidOperationException("Điểm chuẩn cho ngành, năm và phương thức tuyển sinh này đã tồn tại");
            }

            existingScore.MajorId = admissionScore.MajorId;
            existingScore.Year = admissionScore.Year;
            existingScore.Score = admissionScore.Score;
            existingScore.AdmissionMethodId = admissionScore.AdmissionMethodId;
            existingScore.Note = admissionScore.Note;
            existingScore.SubjectCombination = admissionScore.SubjectCombination;

            await _unitOfWork.AdmissionScores.UpdateAsync(existingScore);
            await _unitOfWork.SaveChangesAsync();
            
            return await _unitOfWork.AdmissionScores.GetByIdAsync(existingScore.Id) ?? existingScore;
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