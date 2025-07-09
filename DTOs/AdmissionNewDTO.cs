namespace AdmissionInfoSystem.DTOs
{
    public class AdmissionNewDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
        public int? Year { get; set; }
        public int UniversityId { get; set; }
        public string UniversityName { get; set; } = string.Empty;
    }
    
    public class AdmissionNewCreateDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
        public int? Year { get; set; }
        public int UniversityId { get; set; }
    }
    
    public class AdmissionNewUpdateDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
        public int? Year { get; set; }
        public int UniversityId { get; set; }
    }

    // DTO cho danh sách với nội dung rút gọn (phân trang)
    public class AdmissionNewSummaryDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty; // Nội dung rút gọn
        public DateTime PublishDate { get; set; }
        public int? Year { get; set; }
        public int UniversityId { get; set; }
        public string UniversityName { get; set; } = string.Empty;
    }

    // DTO cho phân trang
    public class PagedAdmissionNewsDTO
    {
        public List<AdmissionNewSummaryDTO> Items { get; set; } = new List<AdmissionNewSummaryDTO>();
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
} 