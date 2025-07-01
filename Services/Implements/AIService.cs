using AdmissionInfoSystem.DTOs;
using AdmissionInfoSystem.Services.Interface;
using AdmissionInfoSystem.Repositories.Interface;
using AdmissionInfoSystem.Repositories;
using AdmissionInfoSystem.Models;
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
        private readonly IAcademicProgramRepository _academicProgramRepository;
        private readonly IAdmissionMethodRepository _admissionMethodRepository;
        private readonly IAdmissionCriteriaRepository _admissionCriteriaRepository;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AIService(
            IUniversityRepository universityRepository,
            IMajorRepository majorRepository,
            IAdmissionScoreRepository admissionScoreRepository,
            IAdmissionNewRepository admissionNewRepository,
            IScholarshipRepository scholarshipRepository,
            IAcademicProgramRepository academicProgramRepository,
            IAdmissionMethodRepository admissionMethodRepository,
            IAdmissionCriteriaRepository admissionCriteriaRepository,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _universityRepository = universityRepository;
            _majorRepository = majorRepository;
            _admissionScoreRepository = admissionScoreRepository;
            _admissionNewRepository = admissionNewRepository;
            _scholarshipRepository = scholarshipRepository;
            _academicProgramRepository = academicProgramRepository;
            _admissionMethodRepository = admissionMethodRepository;
            _admissionCriteriaRepository = admissionCriteriaRepository;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GenerateResponseAsync(string userMessage, List<ChatMessageDTO> chatHistory, string contextData)
        {
            // Tạo prompt cho AI
            var systemPrompt = @"Bạn là một trợ lý AI chuyên về tuyển sinh đại học tại Việt Nam. 

QUY TẮC TUYỆT ĐỐI:
- BẮT BUỘC phải sử dụng dữ liệu được cung cấp trong phần 'Dữ liệu ngữ cảnh' để trả lời
- NGHIÊM CẤM nói 'không có thông tin', 'xin lỗi', 'tôi không thể' khi có dữ liệu trong ngữ cảnh
- LUÔN bắt đầu câu trả lời bằng thông tin tích cực: 'Dựa trên dữ liệu...', 'Theo thông tin...', 'Từ dữ liệu...'
- Nếu có bất kỳ thông tin nào trong dữ liệu ngữ cảnh, hãy sử dụng ngay lập tức
- Trả lời trực tiếp, không giải thích tại sao có hoặc không có thông tin

CÁCH TRẢ LỜI:
- Luôn bắt đầu bằng: 'Dựa trên dữ liệu tuyển sinh...' hoặc 'Theo thông tin từ hệ thống...'
- Đưa ra thông tin cụ thể ngay lập tức
- Không bao giờ nói 'xin lỗi' hay 'không có thông tin' khi đã có dữ liệu
- Nếu không tìm thấy trường cụ thể, hãy trả lời về các trường tương tự có trong dữ liệu
- Luôn tận dụng tối đa dữ liệu có sẵn

VÍ DỤ TRẢ LỜI TỐT:
❌ 'Tôi xin lỗi, dữ liệu không có...' 
✅ 'Dựa trên dữ liệu tuyển sinh, Đại học FPT có học phí...'

❌ 'Không có thông tin về...'
✅ 'Theo thông tin từ hệ thống, các trường có trong database bao gồm...'

Nhiệm vụ:
1. Trả lời câu hỏi về số lượng ngành, tên ngành, điểm chuẩn, học phí
2. So sánh trường đại học và ngành học  
3. Tư vấn dựa trên dữ liệu có sẵn
4. Cung cấp thông tin học bổng, phương thức tuyển sinh
5. Giải đáp về quy trình tuyển sinh";

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
            
            // Debug logging
            Console.WriteLine($"DEBUG: GetAdmissionContextAsync called with query: '{query}'");

            try
            {
                // Lấy thông tin trường đại học
                var universities = await _universityRepository.GetAllAsync();
                Console.WriteLine($"DEBUG: Found {universities?.Count() ?? 0} universities in database");
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
                Console.WriteLine($"DEBUG: Found {majors?.Count() ?? 0} majors in database");
                
                // Debug: Kiểm tra dữ liệu University có được load không
                if (majors != null && majors.Any())
                {
                    var majorWithUni = majors.FirstOrDefault(m => m.University != null);
                    if (majorWithUni != null)
                    {
                        Console.WriteLine($"DEBUG: Sample major with University: '{majorWithUni.Name}' -> University: '{majorWithUni.University.Name}'");
                    }
                    else
                    {
                        Console.WriteLine($"DEBUG: ERROR - No majors have University data loaded!");
                    }
                }
                
                // Debug: Kiểm tra cụ thể ngành của trường ID 37
                if (majors != null)
                {
                    var majorsOfUni37 = majors.Where(m => m.UniversityId == 37).ToList();
                    Console.WriteLine($"DEBUG: Found {majorsOfUni37.Count} majors for University ID 37");
                    if (majorsOfUni37.Any())
                    {
                        var firstMajor = majorsOfUni37.First();
                        Console.WriteLine($"DEBUG: First major of Uni 37: '{firstMajor.Name}' - University loaded: {firstMajor.University?.Name ?? "NULL"}");
                    }
                }
                
                // Tách keywords từ query để tìm kiếm linh hoạt hơn
                var queryKeywords = query.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Where(word => word.Length > 2) // Bỏ qua từ quá ngắn
                    .ToList();
                                        Console.WriteLine($"DEBUG: Query keywords: [{string.Join(", ", queryKeywords)}]");
                        
                        // Debug: Kiểm tra từ khóa đặc biệt - danh sách đầy đủ từ database
                        var specialKeywordsList = new[] { 
                            // Tên trường viết tắt
                            "fpt", "hcmut", "hust", "dtu", "ctu", "hu", "udn", "tnu", "vinhu", "ptit", "ptithcm", 
                            "neu", "vnu", "uit", "uet", "hutech", "buv", "dut", "hsb",
                            
                            // Từ khóa đặc trưng của tên trường
                            "bách", "khoa", "duy", "tân", "vinh", "huế", "ngân", "hàng", "hậu", "cần", 
                            "âm", "nhạc", "báo", "chí", "biên", "phòng", "thanh", "thiếu", "niên", "cảnh", "sát",
                            "an", "ninh", "ngoại", "giao", "nông", "nghiệp", "quân", "y", "tài", "chính",
                            "kỹ", "thuật", "mật", "mã", "hàng", "không", "hành", "chính", "tòa", "án",
                            "phụ", "nữ", "chính", "sách", "phát", "triển", "công", "đoàn", "quốc", "tế",
                            
                            // Địa danh
                            "hà", "nội", "hcm", "cần", "thơ", "đà", "nẵng", "quy", "nhơn", "thái", "nguyên", 
                            "hải", "phòng", "nha", "trang", "vũng", "tàu", "nghệ", "an", "đồng", "nai",
                            "ecopark", "văn", "giang", "hưng", "yên", "long", "thành", "quảng", "trị"
                        };
                        var querySpecialKeywordsList = queryKeywords.Where(k => specialKeywordsList.Contains(k, StringComparer.OrdinalIgnoreCase)).ToList();
                        Console.WriteLine($"DEBUG: Special keywords found: [{string.Join(", ", querySpecialKeywordsList)}]");
                
                // Xử lý các loại query khác nhau
                var relevantMajors = new List<Major>();
                
                if (majors != null)
                {
                    // Nếu hỏi về ngành cụ thể (VD: "trường có ngành công nghệ thông tin")
                    if (query.Contains("trường có ngành", StringComparison.OrdinalIgnoreCase) ||
                        query.Contains("trường nào có", StringComparison.OrdinalIgnoreCase))
                    {
                        // Tìm tất cả ngành khớp với tên ngành được hỏi
                        var majorKeywords = queryKeywords.Where(k => 
                            !new[] { "trường", "có", "ngành", "nào", "là", "gì", "thế", "vậy" }.Contains(k, StringComparer.OrdinalIgnoreCase)).ToList();
                        
                        relevantMajors = majors.Where(m => 
                            majorKeywords.Any(keyword => m.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                            .ToList();
                    }
                    // Nếu hỏi về số lượng ngành của trường cụ thể (VD: "đại học FPT có bao nhiều ngành")
                    else if (query.Contains("có bao nhiều ngành", StringComparison.OrdinalIgnoreCase) ||
                             query.Contains("có mấy ngành", StringComparison.OrdinalIgnoreCase))
                    {
                        // Lấy tên trường từ query
                        var universityKeywords = queryKeywords.Where(k => 
                            !new[] { "có", "bao", "nhiều", "ngành", "mấy", "là", "gì" }.Contains(k, StringComparer.OrdinalIgnoreCase)).ToList();
                        
                        Console.WriteLine($"DEBUG: University keywords for search: [{string.Join(", ", universityKeywords)}]");
                        
                        // Tìm trường dựa trên từ khóa - ưu tiên tìm chính xác trước
                        var universitiesFound = new List<University>();
                        
                        if (universities != null)
                        {
                            // Ưu tiên 1: Tìm theo ShortName chính xác (VD: FPT)
                            var exactShortNameMatch = universities.Where(u =>
                                universityKeywords.Any(k => string.Equals(u.ShortName, k, StringComparison.OrdinalIgnoreCase)))
                                .ToList();
                            
                            // Ưu tiên 2: Tìm theo tên chứa từ khóa đặc biệt
                            var specialKeywords = new[] { "bách", "khoa", "fpt", "huflit", "hutech", "tdtu", "ptit", "hust" };
                            var specialKeywordMatch = universities.Where(u =>
                                universityKeywords.Any(k => specialKeywords.Contains(k, StringComparer.OrdinalIgnoreCase) &&
                                    (u.Name.Contains(k, StringComparison.OrdinalIgnoreCase) || 
                                     u.ShortName.Contains(k, StringComparison.OrdinalIgnoreCase))))
                                .ToList();
                            
                            // Ưu tiên 3: Tìm theo số từ khóa khớp (linh hoạt hơn)
                            var keywordMatch = universities.Where(u => 
                            {
                                var matchCount = universityKeywords.Count(keyword => 
                                    u.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                                    u.ShortName.Contains(keyword, StringComparison.OrdinalIgnoreCase));
                                // Nếu có ít từ khóa thì chỉ cần 1 match, nhiều từ khóa thì cần ít nhất 2
                                var requiredMatches = universityKeywords.Count <= 3 ? 1 : 2;
                                return matchCount >= requiredMatches;
                            }).ToList();
                            
                            // Kết hợp theo thứ tự ưu tiên
                            universitiesFound.AddRange(exactShortNameMatch);
                            universitiesFound.AddRange(specialKeywordMatch.Where(u => !universitiesFound.Contains(u)));
                            universitiesFound.AddRange(keywordMatch.Where(u => !universitiesFound.Contains(u)));
                            
                            // Giới hạn kết quả
                            universitiesFound = universitiesFound.Take(5).ToList();
                        }
                        
                        Console.WriteLine($"DEBUG: Exact universities found: [{string.Join(", ", universitiesFound?.Select(u => u.Name) ?? new List<string>())}]");
                        
                        if (universitiesFound?.Any() == true)
                        {
                            // Lấy ngành của các trường tìm được
                            var universityIds = universitiesFound.Select(u => u.Id).ToList();
                            relevantMajors = majors.Where(m => universityIds.Contains(m.UniversityId)).ToList();
                        }
                        else
                        {
                            // Nếu không tìm thấy trường chính xác, tìm theo từ khóa
                            relevantMajors = majors.Where(m =>
                                universityKeywords.Any(keyword =>
                                    m.University?.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true ||
                                    m.University?.ShortName.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true))
                                .ToList();
                        }
                    }
                    else
                    {
                        // Logic tìm kiếm tổng quát - ưu tiên trường đại học
                        var foundMajors = new List<Major>();
                        
                        // Ưu tiên 0: Tìm chính xác theo tên trường đầy đủ hoặc từ khóa chính
                        var exactUniversityMajors = majors.Where(m => 
                        {
                            if (m.University?.Name == null) return false;
                            
                            // Kiểm tra từng từ quan trọng (bỏ từ chung)
                            var importantWords = queryKeywords.Where(k => 
                                !new[] { "đại", "học", "trường", "có", "là", "phí", "gì", "thế", "nào", "bao", "nhiêu", "ngành", "điểm", "chuẩn" }.Contains(k, StringComparer.OrdinalIgnoreCase)).ToList();
                            
                            if (!importantWords.Any()) return false;
                            
                            // Debug: Chỉ in cho một số trường quan tâm
                            if (m.University.Name.Contains("Ngân", StringComparison.OrdinalIgnoreCase) || 
                                m.University.Name.Contains("Hậu", StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine($"DEBUG: Important words for '{m.University.Name}': [{string.Join(", ", importantWords)}]");
                            }
                            
                            // Kiểm tra xem có ít nhất 70% từ quan trọng khớp không (linh hoạt hơn)
                            var universityWords = m.University.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            var matchedWords = importantWords.Where(keyword => 
                                universityWords.Any(word => word.Contains(keyword, StringComparison.OrdinalIgnoreCase))).ToList();
                            
                            var matchRatio = (double)matchedWords.Count / importantWords.Count;
                            var isMatch = matchRatio >= 0.7; // Cần ít nhất 70% từ khớp
                            
                            if (isMatch)
                            {
                                Console.WriteLine($"DEBUG: MATCH found for '{m.University.Name}' with ratio {matchRatio:P1} - matched: [{string.Join(", ", matchedWords)}]");
                            }
                            
                            return isMatch;
                        }).ToList();
                        
                        // Ưu tiên 1: Tìm theo ShortName chính xác của trường
                        var exactShortNameMajors = majors.Where(m => 
                            queryKeywords.Any(k => string.Equals(m.University?.ShortName, k, StringComparison.OrdinalIgnoreCase)))
                            .ToList();
                        
                        // Ưu tiên 2: Tìm theo từng từ trong tên trường (chính xác hơn)
                        var nameWordMajors = majors.Where(m => 
                        {
                            if (m.University?.Name == null) return false;
                            var universityWords = m.University.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            // Loại bỏ từ khóa chung như "học", "phí", "là", "có"
                            var specificKeywords = queryKeywords.Where(k => 
                                !new[] { "học", "phí", "là", "có", "gì", "thế", "nào", "trường" }.Contains(k, StringComparer.OrdinalIgnoreCase)).ToList();
                            
                            if (!specificKeywords.Any()) return false;
                            
                            // Đếm số từ khóa cụ thể khớp - cần ít nhất 1 từ khớp chính xác
                            var matchCount = specificKeywords.Count(keyword => 
                                universityWords.Any(word => word.Contains(keyword, StringComparison.OrdinalIgnoreCase)));
                            return matchCount > 0;
                        }).ToList();
                        
                        // Ưu tiên 3: Tìm theo tên trường chứa từ khóa đặc biệt  
                        var specialKeywords = new[] { 
                            // Tên trường viết tắt
                            "fpt", "hcmut", "hust", "dtu", "ctu", "hu", "udn", "tnu", "vinhu", "ptit", "ptithcm", 
                            "neu", "vnu", "uit", "uet", "hutech", "buv", "dut", "hsb",
                            
                            // Từ khóa đặc trưng của tên trường
                            "bách", "khoa", "duy", "tân", "vinh", "huế", "ngân", "hàng", "hậu", "cần", 
                            "âm", "nhạc", "báo", "chí", "biên", "phòng", "thanh", "thiếu", "niên", "cảnh", "sát",
                            "an", "ninh", "ngoại", "giao", "nông", "nghiệp", "quân", "y", "tài", "chính",
                            "kỹ", "thuật", "mật", "mã", "hàng", "không", "hành", "chính", "tòa", "án",
                            "phụ", "nữ", "chính", "sách", "phát", "triển", "công", "đoàn", "quốc", "tế",
                            
                            // Địa danh
                            "hà", "nội", "hcm", "cần", "thơ", "đà", "nẵng", "quy", "nhơn", "thái", "nguyên", 
                            "hải", "phòng", "nha", "trang", "vũng", "tàu", "nghệ", "an", "đồng", "nai",
                            "ecopark", "văn", "giang", "hưng", "yên", "long", "thành", "quảng", "trị"
                        };
                        var querySpecialKeywords = queryKeywords.Where(k => specialKeywords.Contains(k, StringComparer.OrdinalIgnoreCase)).ToList();
                        var specialKeywordMajors = majors.Where(m =>
                            querySpecialKeywords.Any(k => 
                                m.University?.Name.Contains(k, StringComparison.OrdinalIgnoreCase) == true || 
                                m.University?.ShortName.Contains(k, StringComparison.OrdinalIgnoreCase) == true))
                            .ToList();
                        
                        // Ưu tiên 4: Tìm theo tên trường đầy đủ
                        var universityNameMajors = majors.Where(m =>
                            m.University?.Name.Contains(query, StringComparison.OrdinalIgnoreCase) == true ||
                            m.University?.ShortName.Contains(query, StringComparison.OrdinalIgnoreCase) == true)
                            .ToList();
                        
                        // Ưu tiên 5: Tìm theo tên ngành
                        var majorNameMajors = majors.Where(m =>
                            m.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            m.Description?.Contains(query, StringComparison.OrdinalIgnoreCase) == true)
                            .ToList();
                        
                        // Ưu tiên 6: Tìm theo keywords tổng quát (linh hoạt hơn)
                        var keywordMajors = majors.Where(m => 
                        {
                            var universityMatches = queryKeywords.Count(keyword => 
                                m.University?.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true ||
                                m.University?.ShortName.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true);
                            var majorMatches = queryKeywords.Count(keyword => 
                                m.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
                            
                            // Ưu tiên trường đại học, nếu có ít từ khóa thì chỉ cần 1 match
                            var requiredMatches = queryKeywords.Count <= 3 ? 1 : 2;
                            return universityMatches >= requiredMatches || majorMatches >= 1;
                        }).ToList();
                        
                        // Debug: In số kết quả từ mỗi phương pháp tìm kiếm
                        Console.WriteLine($"DEBUG: exactUniversityMajors: {exactUniversityMajors.Count}");
                        Console.WriteLine($"DEBUG: exactShortNameMajors: {exactShortNameMajors.Count}");
                        Console.WriteLine($"DEBUG: nameWordMajors: {nameWordMajors.Count}");
                        Console.WriteLine($"DEBUG: specialKeywordMajors: {specialKeywordMajors.Count}");
                        Console.WriteLine($"DEBUG: universityNameMajors: {universityNameMajors.Count}");
                        Console.WriteLine($"DEBUG: majorNameMajors: {majorNameMajors.Count}");
                        Console.WriteLine($"DEBUG: keywordMajors: {keywordMajors.Count}");
                        
                        // Kết hợp theo thứ tự ưu tiên - exactUniversityMajors có ưu tiên cao nhất
                        foundMajors.AddRange(exactUniversityMajors);
                        foundMajors.AddRange(exactShortNameMajors.Where(m => !foundMajors.Contains(m)));
                        foundMajors.AddRange(specialKeywordMajors.Where(m => !foundMajors.Contains(m)));
                        foundMajors.AddRange(nameWordMajors.Where(m => !foundMajors.Contains(m)));
                        foundMajors.AddRange(universityNameMajors.Where(m => !foundMajors.Contains(m)));
                        foundMajors.AddRange(majorNameMajors.Where(m => !foundMajors.Contains(m)));
                        foundMajors.AddRange(keywordMajors.Where(m => !foundMajors.Contains(m)));
                        
                        relevantMajors = foundMajors.Take(20).ToList();
                    }
                }
                Console.WriteLine($"DEBUG: Found {relevantMajors?.Count() ?? 0} relevant majors for query '{query}'");
                
                // Debug: In ra các trường được tìm thấy
                if (relevantMajors?.Any() == true)
                {
                    var universitiesFound = relevantMajors.Select(m => m.University?.Name).Distinct().Where(name => !string.IsNullOrEmpty(name));
                    Console.WriteLine($"DEBUG: Universities found: [{string.Join(", ", universitiesFound)}]");
                }

                if (relevantMajors?.Any() == true)
                {
                    // Xử lý đặc biệt cho câu hỏi về số lượng ngành
                    if (query.Contains("bao nhiều ngành", StringComparison.OrdinalIgnoreCase) ||
                        query.Contains("có mấy ngành", StringComparison.OrdinalIgnoreCase))
                    {
                        var majorsByUniversity = relevantMajors.GroupBy(m => m.University?.Name ?? "Không xác định");
                        
                        context.AppendLine("THÔNG TIN SỐ LƯỢNG NGÀNH:");
                        foreach (var universityGroup in majorsByUniversity)
                        {
                            context.AppendLine($"📊 {universityGroup.Key} có tổng cộng {universityGroup.Count()} ngành đào tạo:");
                            var majorsListSorted = universityGroup.OrderBy(m => m.Name);
                            int count = 1;
                                                         foreach (var major in majorsListSorted)
                             {
                                 context.AppendLine($"  {count}. {major.Name} (Mã: {major.Code})");
                                 if (major.AdmissionScore > 0)
                                     context.AppendLine($"     Điểm chuẩn: {major.AdmissionScore} điểm");
                                 else
                                     context.AppendLine($"     Điểm chuẩn: Chưa cập nhật");
                                 count++;
                             }
                            context.AppendLine();
                        }
                    }
                    else
                    {
                        context.AppendLine("THÔNG TIN NGÀNH HỌC:");
                        
                        // Nhóm theo trường đại học để dễ hiểu
                        var majorsByUniversity = relevantMajors.GroupBy(m => m.University?.Name ?? "Không xác định");
                        
                        foreach (var universityGroup in majorsByUniversity)
                        {
                            context.AppendLine($"🏫 {universityGroup.Key} - Có {universityGroup.Count()} ngành:");
                            foreach (var major in universityGroup)
                            {
                                context.AppendLine($"  • {major.Name} (Mã: {major.Code})");
                                if (major.AdmissionScore > 0)
                                    context.AppendLine($"    Điểm chuẩn: {major.AdmissionScore} điểm");
                                if (!string.IsNullOrEmpty(major.Description))
                                    context.AppendLine($"    Mô tả: {major.Description}");
                            }
                            context.AppendLine();
                        }
                    }

                    // Lấy thông tin học phí từ AcademicPrograms
                    if (relevantMajors?.Any() == true)
                    {
                        var universityIds = relevantMajors.Select(m => m.UniversityId).Distinct().ToList();
                        var academicPrograms = await _academicProgramRepository.GetAllAsync();
                        var relevantPrograms = academicPrograms.Where(p => universityIds.Contains(p.UniversityId)).ToList();
                        
                        if (relevantPrograms.Any())
                        {
                            context.AppendLine("THÔNG TIN HỌC PHÍ:");
                            var programsByUniversity = relevantPrograms.GroupBy(p => 
                                universities?.FirstOrDefault(u => u.Id == p.UniversityId)?.Name ?? "Không xác định");
                            
                            foreach (var universityGroup in programsByUniversity)
                            {
                                context.AppendLine($"💰 {universityGroup.Key}:");
                                foreach (var program in universityGroup)
                                {
                                    context.AppendLine($"  • {program.Name}: {program.Tuition:N0} {program.TuitionUnit}");
                                    if (!string.IsNullOrEmpty(program.Description))
                                        context.AppendLine($"    Mô tả: {program.Description}");
                                }
                                context.AppendLine();
                            }
                        }
                    }
                }

                // Lấy thông tin phương thức tuyển sinh
                var admissionMethods = await _admissionMethodRepository.GetAllAsync();
                var relevantMethods = admissionMethods.Where(m =>
                    m.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    m.Description?.Contains(query, StringComparison.OrdinalIgnoreCase) == true)
                    .Take(10);

                if (relevantMethods.Any())
                {
                    context.AppendLine("THÔNG TIN PHƯƠNG THỨC TUYỂN SINH:");
                    foreach (var method in relevantMethods)
                    {
                        context.AppendLine($"📝 {method.Name}");
                        if (!string.IsNullOrEmpty(method.Description))
                            context.AppendLine($"  Mô tả: {method.Description}");
                        if (method.Year.HasValue)
                            context.AppendLine($"  Năm áp dụng: {method.Year}");
                    }
                    context.AppendLine();
                }

                // Lấy thông tin tiêu chí tuyển sinh
                var admissionCriterias = await _admissionCriteriaRepository.GetAllAsync();
                var relevantCriterias = admissionCriterias.Where(c =>
                    c.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    c.Description.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .Take(15);

                if (relevantCriterias.Any())
                {
                    context.AppendLine("THÔNG TIN TIÊU CHÍ TUYỂN SINH:");
                    foreach (var criteria in relevantCriterias)
                    {
                        context.AppendLine($"📋 {criteria.Name}");
                        context.AppendLine($"  Mô tả: {criteria.Description}");
                        if (criteria.MinimumScore.HasValue)
                            context.AppendLine($"  Điểm tối thiểu: {criteria.MinimumScore}");
                        context.AppendLine($"  Phương thức: {criteria.AdmissionMethod?.Name ?? "Không xác định"}");
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
                Console.WriteLine($"DEBUG: Exception in GetAdmissionContextAsync: {ex}");
                context.AppendLine($"Lỗi khi lấy dữ liệu: {ex.Message}");
            }

            var result = context.ToString();
            Console.WriteLine($"DEBUG: Final context length: {result.Length} characters");
            Console.WriteLine($"DEBUG: Context content preview: {result.Substring(0, Math.Min(200, result.Length))}...");
            return result;
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