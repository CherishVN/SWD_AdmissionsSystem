using AdmissionInfoSystem.DTOs;
using AdmissionInfoSystem.Services.Interface;
using AdmissionInfoSystem.Repositories.Interface;
using AdmissionInfoSystem.Repositories;
using System.Text;
using System.Text.Json;

namespace AdmissionInfoSystem.Services.Implements
{
    public class AIService : IAIService
    {
        private readonly IUniversityRepository _universityRepository;
        private readonly IMajorRepository _majorRepository;
        private readonly IAdmissionScoreRepository _admissionScoreRepository;
        private readonly IAdmissionNewRepository _admissionNewRepository;
        private readonly IScholarshipRepository _scholarshipRepository;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AIService(
            IUniversityRepository universityRepository,
            IMajorRepository majorRepository,
            IAdmissionScoreRepository admissionScoreRepository,
            IAdmissionNewRepository admissionNewRepository,
            IScholarshipRepository scholarshipRepository,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _universityRepository = universityRepository;
            _majorRepository = majorRepository;
            _admissionScoreRepository = admissionScoreRepository;
            _admissionNewRepository = admissionNewRepository;
            _scholarshipRepository = scholarshipRepository;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GenerateResponseAsync(string userMessage, List<ChatMessageDTO> chatHistory, string contextData)
        {
            // Tạo prompt cho AI
            var systemPrompt = @"Bạn là một trợ lý AI thông minh chuyên về tuyển sinh đại học tại Việt Nam. 
Nhiệm vụ của bạn là:
1. Trả lời các câu hỏi về tuyển sinh, điểm chuẩn, ngành học, trường đại học
2. So sánh các trường đại học và ngành học
3. Tư vấn lựa chọn trường và ngành phù hợp
4. Cung cấp thông tin về học bổng
5. Giải đáp thắc mắc về quy trình tuyển sinh

Hãy trả lời một cách thân thiện, chi tiết và chính xác. Sử dụng dữ liệu được cung cấp để đưa ra câu trả lời.";

            var conversationHistory = new StringBuilder();
            foreach (var msg in chatHistory.TakeLast(10)) // Chỉ lấy 10 tin nhắn gần nhất
            {
                conversationHistory.AppendLine($"{msg.Sender}: {msg.Message}");
            }

            var prompt = $@"{systemPrompt}

Dữ liệu ngữ cảnh:
{contextData}

Lịch sử cuộc trò chuyện:
{conversationHistory}

Người dùng: {userMessage}
Trợ lý AI:";

            // Gọi AI API (ví dụ: Gemini hoặc OpenAI)
            return await CallGeminiAPI(prompt);
        }

        public async Task<string> GetAdmissionContextAsync(string query)
        {
            var context = new StringBuilder();

            try
            {
                // Lấy thông tin trường đại học
                var universities = await _universityRepository.GetAllAsync();
                var relevantUniversities = universities.Where(u => 
                    u.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    u.Introduction?.Contains(query, StringComparison.OrdinalIgnoreCase) == true ||
                    u.ShortName.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .Take(5);

                if (relevantUniversities.Any())
                {
                    context.AppendLine("THÔNG TIN TRƯỜNG ĐẠI HỌC:");
                    foreach (var uni in relevantUniversities)
                    {
                        context.AppendLine($"- {uni.Name} ({uni.ShortName})");
                        context.AppendLine($"  Giới thiệu: {uni.Introduction}");
                        context.AppendLine($"  Website chính thức: {uni.OfficialWebsite}");
                        context.AppendLine($"  Website tuyển sinh: {uni.AdmissionWebsite}");
                        context.AppendLine($"  Loại: {uni.Type}");
                        if (uni.Ranking.HasValue)
                            context.AppendLine($"  Xếp hạng: {uni.Ranking} ({uni.RankingCriteria})");
                        if (!string.IsNullOrEmpty(uni.Locations))
                            context.AppendLine($"  Địa điểm: {uni.Locations}");
                    }
                    context.AppendLine();
                }

                // Lấy thông tin ngành học
                var majors = await _majorRepository.GetAllAsync();
                var relevantMajors = majors.Where(m =>
                    m.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    m.Description?.Contains(query, StringComparison.OrdinalIgnoreCase) == true)
                    .Take(10);

                if (relevantMajors.Any())
                {
                    context.AppendLine("THÔNG TIN NGÀNH HỌC:");
                    foreach (var major in relevantMajors)
                    {
                        context.AppendLine($"- {major.Name} (Mã: {major.Code})");
                        if (!string.IsNullOrEmpty(major.Description))
                            context.AppendLine($"  Mô tả: {major.Description}");
                    }
                    context.AppendLine();
                }

                // Lấy thông tin điểm chuẩn
                var admissionScores = await _admissionScoreRepository.GetAllAsync();
                var relevantScores = admissionScores.Where(s =>
                    s.Major?.University?.Name.Contains(query, StringComparison.OrdinalIgnoreCase) == true ||
                    s.Major?.Name.Contains(query, StringComparison.OrdinalIgnoreCase) == true)
                    .Take(10);

                if (relevantScores.Any())
                {
                    context.AppendLine("THÔNG TIN ĐIỂM CHUẨN:");
                    foreach (var score in relevantScores)
                    {
                        context.AppendLine($"- {score.Major?.University?.Name} - {score.Major?.Name}: {score.Score} điểm (Năm {score.Year})");
                    }
                    context.AppendLine();
                }

                // Lấy tin tức tuyển sinh
                var admissionNews = await _admissionNewRepository.GetAllAsync();
                var relevantNews = admissionNews.Where(n =>
                    n.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    n.Content?.Contains(query, StringComparison.OrdinalIgnoreCase) == true)
                    .OrderByDescending(n => n.PublishDate)
                    .Take(5);

                if (relevantNews.Any())
                {
                    context.AppendLine("TIN TỨC TUYỂN SINH:");
                    foreach (var news in relevantNews)
                    {
                        context.AppendLine($"- {news.Title}");
                        context.AppendLine($"  Nội dung: {news.Content.Substring(0, Math.Min(200, news.Content.Length))}...");
                        context.AppendLine($"  Ngày xuất bản: {news.PublishDate:dd/MM/yyyy}");
                        if (news.Year.HasValue)
                            context.AppendLine($"  Năm: {news.Year}");
                    }
                    context.AppendLine();
                }

                // Lấy thông tin học bổng
                var scholarships = await _scholarshipRepository.GetAllAsync();
                var relevantScholarships = scholarships.Where(s =>
                    s.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    s.Description?.Contains(query, StringComparison.OrdinalIgnoreCase) == true ||
                    s.Criteria?.Contains(query, StringComparison.OrdinalIgnoreCase) == true)
                    .Take(5);

                if (relevantScholarships.Any())
                {
                    context.AppendLine("THÔNG TIN HỌC BỔNG:");
                    foreach (var scholarship in relevantScholarships)
                    {
                        context.AppendLine($"- {scholarship.Name}");
                        context.AppendLine($"  Mô tả: {scholarship.Description}");
                        if (scholarship.Value.HasValue)
                            context.AppendLine($"  Giá trị: {scholarship.Value:N0} {scholarship.ValueType}");
                        else
                            context.AppendLine($"  Loại giá trị: {scholarship.ValueType}");
                        context.AppendLine($"  Tiêu chí: {scholarship.Criteria}");
                        if (scholarship.Year.HasValue)
                            context.AppendLine($"  Năm: {scholarship.Year}");
                    }
                }

            }
            catch (Exception ex)
            {
                context.AppendLine($"Lỗi khi lấy dữ liệu: {ex.Message}");
            }

            return context.ToString();
        }

        private async Task<string> CallGeminiAPI(string prompt)
        {
            try
            {
                var apiKey = _configuration["Gemini:ApiKey"] ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY");
                if (string.IsNullOrEmpty(apiKey))
                {
                    return "Xin lỗi, tôi chưa thể kết nối với dịch vụ AI. Vui lòng liên hệ quản trị viên để cấu hình API key.";
                }

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        topK = 40,
                        topP = 0.95,
                        maxOutputTokens = 1024
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(
                    $"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent?key={apiKey}",
                    content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var responseObj = JsonSerializer.Deserialize<JsonElement>(responseJson);
                    
                    if (responseObj.TryGetProperty("candidates", out var candidates) &&
                        candidates.GetArrayLength() > 0)
                    {
                        var firstCandidate = candidates[0];
                        if (firstCandidate.TryGetProperty("content", out var content_prop) &&
                            content_prop.TryGetProperty("parts", out var parts) &&
                            parts.GetArrayLength() > 0)
                        {
                            var firstPart = parts[0];
                            if (firstPart.TryGetProperty("text", out var text))
                            {
                                return text.GetString() ?? "Xin lỗi, tôi không thể tạo phản hồi lúc này.";
                            }
                        }
                    }
                }

                return "Xin lỗi, tôi gặp sự cố khi xử lý câu hỏi của bạn. Vui lòng thử lại sau.";
            }
            catch (Exception ex)
            {
                return $"Xin lỗi, tôi gặp lỗi: {ex.Message}. Vui lòng thử lại sau.";
            }
        }
    }
} 