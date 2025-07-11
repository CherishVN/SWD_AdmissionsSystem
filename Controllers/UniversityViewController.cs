using AdmissionInfoSystem.DTOs;
using AdmissionInfoSystem.Services;
using AdmissionInfoSystem.Services.Interface;
using AdmissionInfoSystem.Attributes;
using AdmissionInfoSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AdmissionInfoSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UniversityViewController : ControllerBase
    {
        private readonly IUniversityService _universityService;
        private readonly IAcademicProgramService _programService;
        private readonly IMajorService _majorService;
        private readonly IAdmissionMethodService _methodService;
        private readonly IAdmissionNewService _newsService;
        private readonly IScholarshipService _scholarshipService;
        private readonly IAdmissionScoreService _scoreService;
        private readonly IAdmissionCriteriaService _criteriaService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UniversityViewController> _logger;

        public UniversityViewController(
            IUniversityService universityService,
            IAcademicProgramService programService,
            IMajorService majorService,
            IAdmissionMethodService methodService,
            IAdmissionNewService newsService,
            IScholarshipService scholarshipService,
            IAdmissionScoreService scoreService,
            IAdmissionCriteriaService criteriaService,
            ApplicationDbContext context,
            ILogger<UniversityViewController> logger)
        {
            _universityService = universityService;
            _programService = programService;
            _majorService = majorService;
            _methodService = methodService;
            _newsService = newsService;
            _scholarshipService = scholarshipService;
            _scoreService = scoreService;
            _criteriaService = criteriaService;
            _context = context;
            _logger = logger;
        }

        // GET: api/UniversityView/my-university - Chỉ xem thông tin cơ bản của trường
        [HttpGet("my-university")]
        [UniversityAuthorize]
        public async Task<ActionResult<UniversityDTO>> GetMyUniversity()
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                var university = await _universityService.GetUniversityByIdAsync(universityId.Value);
                if (university == null)
                {
                    return NotFound(new { message = "Không tìm thấy thông tin trường đại học" });
                }

                var universityDto = new UniversityDTO
                {
                    Id = university.Id,
                    Name = university.Name,
                    ShortName = university.ShortName,
                    Introduction = university.Introduction,
                    OfficialWebsite = university.OfficialWebsite,
                    AdmissionWebsite = university.AdmissionWebsite,
                    Ranking = university.Ranking,
                    RankingCriteria = university.RankingCriteria,
                    Locations = university.Locations,
                    Logo = university.Logo,
                    Type = university.Type
                };

                return Ok(universityDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin trường đại học");
                return StatusCode(500, new { message = "Lỗi server" });
            }
        }

        // GET: api/UniversityView/my-programs - Xem chương trình đào tạo của trường
        [HttpGet("my-programs")]
        [UniversityAuthorize]
        public async Task<ActionResult<IEnumerable<object>>> GetMyPrograms()
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                var programs = await _programService.GetByUniversityIdAsync(universityId.Value);
                var programDtos = programs.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Tuition,
                    p.TuitionUnit,
                    p.Year
                });

                return Ok(programDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chương trình đào tạo");
                return StatusCode(500, new { message = "Lỗi server" });
            }
        }

        // GET: api/UniversityView/my-majors - Xem ngành học của trường
        [HttpGet("my-majors")]
        [UniversityAuthorize]
        public async Task<ActionResult<IEnumerable<object>>> GetMyMajors()
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                var majors = await _majorService.GetByUniversityIdAsync(universityId.Value);
                var majorDtos = majors.Select(m => new
                {
                    m.Id,
                    m.Name,
                    m.Code,
                    m.Description,
                    m.AdmissionScore,
                    m.Year
                });

                return Ok(majorDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách ngành học");
                return StatusCode(500, new { message = "Lỗi server" });
            }
        }

        // GET: api/UniversityView/my-admission-methods - Xem phương thức tuyển sinh
        [HttpGet("my-admission-methods")]
        [UniversityAuthorize]
        public async Task<ActionResult<IEnumerable<object>>> GetMyAdmissionMethods()
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                var methods = await _methodService.GetByUniversityIdAsync(universityId.Value);
                var methodDtos = methods.Select(am => new
                {
                    am.Id,
                    am.Name,
                    am.Description,
                    am.Criteria,
                    am.Year
                });

                return Ok(methodDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy phương thức tuyển sinh");
                return StatusCode(500, new { message = "Lỗi server" });
            }
        }

        // GET: api/UniversityView/my-admission-scores - Xem điểm chuẩn của trường
        [HttpGet("my-admission-scores")]
        [UniversityAuthorize]
        public async Task<ActionResult<IEnumerable<AdmissionScoreDTO>>> GetMyAdmissionScores()
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                // Lấy tất cả điểm chuẩn của các ngành thuộc trường này
                var majors = await _majorService.GetByUniversityIdAsync(universityId.Value);
                var majorIds = majors.Select(m => m.Id).ToList();

                var allScores = new List<AdmissionScoreDTO>();
                
                foreach (var majorId in majorIds)
                {
                    var scores = await _scoreService.GetByMajorIdAsync(majorId);
                    var scoreDtos = scores.Select(s => new AdmissionScoreDTO
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
                    
                    allScores.AddRange(scoreDtos);
                }

                // Sắp xếp theo năm giảm dần, sau đó theo tên ngành
                var sortedScores = allScores.OrderByDescending(s => s.Year).ThenBy(s => s.MajorName);

                return Ok(sortedScores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy điểm chuẩn");
                return StatusCode(500, new { message = "Lỗi server" });
            }
        }

        // GET: api/UniversityView/my-admission-scores/year/{year} - Xem điểm chuẩn theo năm
        [HttpGet("my-admission-scores/year/{year}")]
        [UniversityAuthorize]
        public async Task<ActionResult<IEnumerable<AdmissionScoreDTO>>> GetMyAdmissionScoresByYear(int year)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                var majors = await _majorService.GetByUniversityIdAsync(universityId.Value);
                var majorIds = majors.Select(m => m.Id).ToList();

                var allScores = new List<AdmissionScoreDTO>();
                
                foreach (var majorId in majorIds)
                {
                    var scores = await _scoreService.GetByMajorAndYearAsync(majorId, year);
                    var scoreDtos = scores.Select(s => new AdmissionScoreDTO
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
                    
                    allScores.AddRange(scoreDtos);
                }

                var sortedScores = allScores.OrderBy(s => s.MajorName);

                return Ok(sortedScores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy điểm chuẩn theo năm");
                return StatusCode(500, new { message = "Lỗi server" });
            }
        }

        // GET: api/UniversityView/my-admission-criteria - Xem tiêu chí tuyển sinh
        [HttpGet("my-admission-criteria")]
        [UniversityAuthorize]
        public async Task<ActionResult<IEnumerable<object>>> GetMyAdmissionCriteria()
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                // Lấy tất cả phương thức tuyển sinh của trường
                var methods = await _methodService.GetByUniversityIdAsync(universityId.Value);
                var methodIds = methods.Select(m => m.Id).ToList();

                var allCriteria = new List<object>();
                
                foreach (var methodId in methodIds)
                {
                    var criteria = await _criteriaService.GetByAdmissionMethodIdAsync(methodId);
                    var criteriaDtos = criteria.Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Description,
                        c.MinimumScore,
                        AdmissionMethodId = c.AdmissionMethodId,
                        AdmissionMethodName = methods.FirstOrDefault(m => m.Id == c.AdmissionMethodId)?.Name
                    });
                    
                    allCriteria.AddRange(criteriaDtos);
                }

                // Sắp xếp theo tên phương thức tuyển sinh, sau đó theo tên tiêu chí
                var sortedCriteria = allCriteria.OrderBy(c => 
                    methods.FirstOrDefault(m => m.Id == ((dynamic)c).AdmissionMethodId)?.Name)
                    .ThenBy(c => ((dynamic)c).Name);

                return Ok(sortedCriteria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tiêu chí tuyển sinh");
                return StatusCode(500, new { message = "Lỗi server" });
            }
        }

        // GET: api/UniversityView/my-news - Xem tin tức tuyển sinh
        [HttpGet("my-news")]
        [UniversityAuthorize]
        public async Task<ActionResult<IEnumerable<AdmissionNewDTO>>> GetMyAdmissionNews()
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                var news = await _newsService.GetAdmissionNewsByUniversityAsync(universityId.Value);
                var newsDtos = news.Select(an => new AdmissionNewDTO
                {
                    Id = an.Id,
                    Title = an.Title,
                    Content = an.Content,
                    PublishDate = an.PublishDate,
                    Year = an.Year,
                    UniversityId = an.UniversityId,
                    UniversityName = an.University?.Name ?? string.Empty
                });

                return Ok(newsDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tin tức tuyển sinh");
                return StatusCode(500, new { message = "Lỗi server" });
            }
        }

        // GET: api/UniversityView/my-scholarships - Xem học bổng
        [HttpGet("my-scholarships")]
        [UniversityAuthorize]
        public async Task<ActionResult<IEnumerable<object>>> GetMyScholarships()
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                var scholarships = await _scholarshipService.GetByUniversityIdAsync(universityId.Value);
                var scholarshipDtos = scholarships.Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Description,
                    s.Value,
                    s.ValueType,
                    s.Criteria,
                    s.Year
                });

                return Ok(scholarshipDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin học bổng");
                return StatusCode(500, new { message = "Lỗi server" });
            }
        }

        // Helper method để lấy UniversityId của user hiện tại
        private async Task<int?> GetUserUniversityId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return null;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return user?.UniversityId;
        }
    }
} 