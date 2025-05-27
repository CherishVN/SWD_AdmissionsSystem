namespace AdmissionInfoSystem.DTOs
{
    public class UniversityDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Introduction { get; set; } = string.Empty;
        public string OfficialWebsite { get; set; } = string.Empty;
        public string AdmissionWebsite { get; set; } = string.Empty;
        public int? Ranking { get; set; }
        public string RankingCriteria { get; set; } = string.Empty;
    }
} 