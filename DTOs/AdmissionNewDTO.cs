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
} 