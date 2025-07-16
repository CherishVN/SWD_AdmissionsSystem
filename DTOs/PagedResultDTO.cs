namespace AdmissionInfoSystem.DTOs
{
    public class PagedResultDTO<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }

    public class PagedMajorDTO : PagedResultDTO<MajorDTO>
    {
    }

    public class PagedAdmissionScoreDTO : PagedResultDTO<AdmissionScoreDTO>
    {
    }

    public class MajorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? AdmissionScore { get; set; }
        public int? Year { get; set; }
        public int UniversityId { get; set; }
        public int? ProgramId { get; set; }
        public string? UniversityName { get; set; }
        public string? ProgramName { get; set; }
    }
} 