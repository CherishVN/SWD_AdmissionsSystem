using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.DTOs;
using AdmissionInfoSystem.Repositories;

namespace AdmissionInfoSystem.Services
{
    public class AdmissionNewService : IAdmissionNewService
    {
        private readonly IAdmissionNewRepository _admissionNewRepository;

        public AdmissionNewService(IAdmissionNewRepository admissionNewRepository)
        {
            _admissionNewRepository = admissionNewRepository;
        }

        public async Task<AdmissionNew> CreateAdmissionNewAsync(AdmissionNew admissionNew)
        {
            await _admissionNewRepository.AddAsync(admissionNew);
            return admissionNew;
        }

        public async Task DeleteAdmissionNewAsync(int id)
        {
            await _admissionNewRepository.RemoveAsync(id);
        }

        public async Task<IEnumerable<AdmissionNew>> GetAllAdmissionNewsAsync()
        {
            return await _admissionNewRepository.GetAllAsync();
        }

        public async Task<AdmissionNew> GetAdmissionNewByIdAsync(int id)
        {
            return await _admissionNewRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<AdmissionNew>> GetAdmissionNewsByUniversityAsync(int universityId)
        {
            return await _admissionNewRepository.GetAdmissionNewsByUniversityAsync(universityId);
        }

        public async Task<IEnumerable<AdmissionNew>> GetLatestAdmissionNewsAsync(int count)
        {
            return await _admissionNewRepository.GetLatestAdmissionNewsAsync(count);
        }

        public async Task<PagedAdmissionNewsDTO> GetPagedAdmissionNewsAsync(int page, int pageSize)
        {
            var (items, totalCount) = await _admissionNewRepository.GetPagedAdmissionNewsAsync(page, pageSize);
            
            var summaryItems = items.Select(an => new AdmissionNewSummaryDTO
            {
                Id = an.Id,
                Title = an.Title,
                Summary = GetSummary(an.Content, 200), // Lấy 200 ký tự đầu
                PublishDate = an.PublishDate,
                Year = an.Year,
                UniversityId = an.UniversityId,
                UniversityName = an.University?.Name ?? string.Empty
            }).ToList();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new PagedAdmissionNewsDTO
            {
                Items = summaryItems,
                TotalItems = totalCount,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasNextPage = page < totalPages,
                HasPreviousPage = page > 1
            };
        }

        private string GetSummary(string content, int maxLength)
        {
            if (string.IsNullOrEmpty(content))
                return string.Empty;

            if (content.Length <= maxLength)
                return content;

            // Cắt tại dấu cách gần nhất để tránh cắt giữa từ
            var summary = content.Substring(0, maxLength);
            var lastSpaceIndex = summary.LastIndexOf(' ');
            
            if (lastSpaceIndex > 0)
                summary = summary.Substring(0, lastSpaceIndex);

            return summary + "...";
        }

        public async Task<AdmissionNew> UpdateAdmissionNewAsync(AdmissionNew admissionNew)
        {
            await _admissionNewRepository.UpdateAsync(admissionNew);
            return admissionNew;
        }
    }
} 