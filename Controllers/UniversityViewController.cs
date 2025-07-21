using AdmissionInfoSystem.DTOs;
using AdmissionInfoSystem.Services;
using AdmissionInfoSystem.Services.Interface;
using AdmissionInfoSystem.Attributes;
using AdmissionInfoSystem.Data;
using AdmissionInfoSystem.Models;
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

        // GET: api/UniversityView/my-majors/paged?page=1&pageSize=10 - Xem ngành học của trường (phân trang)
        [HttpGet("my-majors/paged")]
        [UniversityAuthorize]
        public async Task<ActionResult<PagedMajorDTO>> GetMyMajorsPaged(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                // Validate parameters
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var pagedResult = await _majorService.GetPagedByUniversityIdAsync(universityId.Value, page, pageSize);
                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách ngành học phân trang");
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

        // GET: api/UniversityView/my-admission-scores/paged?page=1&pageSize=10 - Xem điểm chuẩn của trường (phân trang)
        [HttpGet("my-admission-scores/paged")]
        [UniversityAuthorize]
        public async Task<ActionResult<PagedAdmissionScoreDTO>> GetMyAdmissionScoresPaged(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                // Validate parameters
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var pagedResult = await _scoreService.GetPagedByUniversityIdAsync(universityId.Value, page, pageSize);
                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy điểm chuẩn phân trang");
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

        // GET: api/UniversityView/my-admission-news - Xem tin tức tuyển sinh của trường
        [HttpGet("my-admission-news")]
        [UniversityAuthorize]
        public async Task<ActionResult<IEnumerable<object>>> GetMyAdmissionNews()
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                var news = await _newsService.GetAdmissionNewsByUniversityAsync(universityId.Value);
                var newsDtos = news.Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Content,
                    n.PublishDate,
                    n.Year
                });

                return Ok(newsDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tin tức tuyển sinh");
                return StatusCode(500, new { message = "Lỗi server" });
            }
        }

        // GET: api/UniversityView/my-news - Xem tin tức tuyển sinh
        [HttpGet("my-news")]
        [UniversityAuthorize]
        public async Task<ActionResult<IEnumerable<AdmissionNewDTO>>> GetMyNews()
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

        // POST: api/UniversityView/verify-my-university
        [HttpPost("verify-my-university")]
        [UniversityAuthorize]
        public async Task<ActionResult<UniversityDTO>> VerifyMyUniversity()
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                var university = await _universityService.VerifyUniversityAsync(universityId.Value);
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
                    Type = university.Type,
                    IsVerified = university.IsVerified
                };

                return Ok(new { 
                    message = "Trường đại học đã được xác thực thành công", 
                    university = universityDto 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xác thực trường đại học");
                return StatusCode(500, new { message = "Lỗi server" });
            }
        }

        #region University Management
        
        // PUT: api/UniversityView/my-university - Cập nhật thông tin trường
        [HttpPut("my-university")]
        [UniversityAuthorize]
        public async Task<IActionResult> UpdateMyUniversity(UpdateUniversityDTO updateDto)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                if (universityId != updateDto.Id)
                {
                    return BadRequest(new { message = "ID không khớp với trường của bạn" });
                }

                var university = await _universityService.GetUniversityByIdAsync(universityId.Value);
                if (university == null)
                {
                    return NotFound(new { message = "Không tìm thấy trường đại học" });
                }

                university.Name = updateDto.Name;
                university.ShortName = updateDto.ShortName;
                university.Introduction = updateDto.Introduction;
                university.OfficialWebsite = updateDto.OfficialWebsite;
                university.AdmissionWebsite = updateDto.AdmissionWebsite;
                university.Ranking = updateDto.Ranking;
                university.RankingCriteria = updateDto.RankingCriteria;
                university.Locations = updateDto.Locations;
                
                // Chỉ cập nhật Logo khi có giá trị mới
                if (!string.IsNullOrEmpty(updateDto.Logo))
                {
                    university.Logo = updateDto.Logo;
                }
                
                university.Type = updateDto.Type;

                await _universityService.UpdateUniversityAsync(university);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật thông tin trường đại học");
                return StatusCode(500, new { message = "Lỗi khi cập nhật thông tin trường đại học", error = ex.Message });
            }
        }

        // PUT: api/UniversityView/my-university/logo - Cập nhật logo trường
        [HttpPut("my-university/logo")]
        [UniversityAuthorize]
        public async Task<IActionResult> UpdateMyUniversityLogo(UpdateLogoDTO updateLogoDto)
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
                    return NotFound(new { message = "Không tìm thấy trường đại học" });
                }

                university.Logo = updateLogoDto.Logo;
                await _universityService.UpdateUniversityAsync(university);

                return Ok(new { 
                    message = "Cập nhật logo thành công",
                    logo = university.Logo,
                    universityId = university.Id,
                    universityName = university.Name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật logo");
                return StatusCode(500, new { message = "Lỗi khi cập nhật logo", error = ex.Message });
            }
        }

        #endregion

        #region Programs Management

        // POST: api/UniversityView/my-programs - Tạo chương trình đào tạo mới
        [HttpPost("my-programs")]
        [UniversityAuthorize]
        public async Task<ActionResult<AcademicProgram>> CreateMyProgram(AcademicProgram program)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                program.UniversityId = universityId.Value;
                var createdProgram = await _programService.CreateAsync(program);
                return CreatedAtAction(nameof(GetMyPrograms), createdProgram);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo chương trình đào tạo");
                return StatusCode(500, new { message = "Lỗi khi tạo chương trình đào tạo", error = ex.Message });
            }
        }

        // PUT: api/UniversityView/my-programs/{id} - Cập nhật chương trình đào tạo
        [HttpPut("my-programs/{id}")]
        [UniversityAuthorize]
        public async Task<IActionResult> UpdateMyProgram(int id, AcademicProgram program)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                if (id != program.Id)
                {
                    return BadRequest(new { message = "ID không khớp" });
                }

                var existingProgram = await _programService.GetByIdAsync(id);
                if (existingProgram == null || existingProgram.UniversityId != universityId)
                {
                    return NotFound(new { message = "Không tìm thấy chương trình hoặc không thuộc trường của bạn" });
                }

                // Detach existing entity để tránh tracking conflict
                _context.Entry(existingProgram).State = EntityState.Detached;
                
                program.UniversityId = universityId.Value;
                await _programService.UpdateAsync(program);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật chương trình đào tạo");
                return StatusCode(500, new { message = "Lỗi khi cập nhật chương trình đào tạo", error = ex.Message });
            }
        }

        // DELETE: api/UniversityView/my-programs/{id} - Xóa chương trình đào tạo
        [HttpDelete("my-programs/{id}")]
        [UniversityAuthorize]
        public async Task<IActionResult> DeleteMyProgram(int id)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                var program = await _programService.GetByIdAsync(id);
                if (program == null || program.UniversityId != universityId)
                {
                    return NotFound(new { message = "Không tìm thấy chương trình hoặc không thuộc trường của bạn" });
                }

                await _programService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa chương trình đào tạo");
                return StatusCode(500, new { message = "Lỗi khi xóa chương trình đào tạo", error = ex.Message });
            }
        }

        #endregion

        #region Majors Management

        // POST: api/UniversityView/my-majors - Tạo ngành học mới
        [HttpPost("my-majors")]
        [UniversityAuthorize]
        public async Task<ActionResult<Major>> CreateMyMajor(Major major)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                major.UniversityId = universityId.Value;
                var createdMajor = await _majorService.CreateAsync(major);
                return CreatedAtAction(nameof(GetMyMajors), createdMajor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo ngành học");
                return StatusCode(500, new { message = "Lỗi khi tạo ngành học", error = ex.Message });
            }
        }

        // PUT: api/UniversityView/my-majors/{id} - Cập nhật ngành học
        [HttpPut("my-majors/{id}")]
        [UniversityAuthorize]
        public async Task<IActionResult> UpdateMyMajor(int id, Major major)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                if (id != major.Id)
                {
                    return BadRequest(new { message = "ID không khớp" });
                }

                var existingMajor = await _majorService.GetByIdAsync(id);
                if (existingMajor == null || existingMajor.UniversityId != universityId)
                {
                    return NotFound(new { message = "Không tìm thấy ngành học hoặc không thuộc trường của bạn" });
                }

                // Detach existing entity để tránh tracking conflict
                _context.Entry(existingMajor).State = EntityState.Detached;
                
                major.UniversityId = universityId.Value;
                await _majorService.UpdateAsync(major);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật ngành học");
                return StatusCode(500, new { message = "Lỗi khi cập nhật ngành học", error = ex.Message });
            }
        }

        // DELETE: api/UniversityView/my-majors/{id} - Xóa ngành học
        [HttpDelete("my-majors/{id}")]
        [UniversityAuthorize]
        public async Task<IActionResult> DeleteMyMajor(int id)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                var major = await _majorService.GetByIdAsync(id);
                if (major == null || major.UniversityId != universityId)
                {
                    return NotFound(new { message = "Không tìm thấy ngành học hoặc không thuộc trường của bạn" });
                }

                await _majorService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa ngành học");
                return StatusCode(500, new { message = "Lỗi khi xóa ngành học", error = ex.Message });
            }
        }

        #endregion

        #region Admission Methods Management

        // POST: api/UniversityView/my-admission-methods - Tạo phương thức tuyển sinh mới
        [HttpPost("my-admission-methods")]
        [UniversityAuthorize]
        public async Task<ActionResult<AdmissionMethod>> CreateMyAdmissionMethod(AdmissionMethod method)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                method.UniversityId = universityId.Value;
                var createdMethod = await _methodService.CreateAsync(method);
                return CreatedAtAction(nameof(GetMyAdmissionMethods), createdMethod);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo phương thức tuyển sinh");
                return StatusCode(500, new { message = "Lỗi khi tạo phương thức tuyển sinh", error = ex.Message });
            }
        }

        // PUT: api/UniversityView/my-admission-methods/{id} - Cập nhật phương thức tuyển sinh
        [HttpPut("my-admission-methods/{id}")]
        [UniversityAuthorize]
        public async Task<IActionResult> UpdateMyAdmissionMethod(int id, AdmissionMethod method)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                if (id != method.Id)
                {
                    return BadRequest(new { message = "ID không khớp" });
                }

                var existingMethod = await _methodService.GetByIdAsync(id);
                if (existingMethod == null || existingMethod.UniversityId != universityId)
                {
                    return NotFound(new { message = "Không tìm thấy phương thức tuyển sinh hoặc không thuộc trường của bạn" });
                }

                // Detach existing entity để tránh tracking conflict
                _context.Entry(existingMethod).State = EntityState.Detached;
                
                method.UniversityId = universityId.Value;
                await _methodService.UpdateAsync(method);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật phương thức tuyển sinh");
                return StatusCode(500, new { message = "Lỗi khi cập nhật phương thức tuyển sinh", error = ex.Message });
            }
        }

        // DELETE: api/UniversityView/my-admission-methods/{id} - Xóa phương thức tuyển sinh
        [HttpDelete("my-admission-methods/{id}")]
        [UniversityAuthorize]
        public async Task<IActionResult> DeleteMyAdmissionMethod(int id)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                var method = await _methodService.GetByIdAsync(id);
                if (method == null || method.UniversityId != universityId)
                {
                    return NotFound(new { message = "Không tìm thấy phương thức tuyển sinh hoặc không thuộc trường của bạn" });
                }

                await _methodService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa phương thức tuyển sinh");
                return StatusCode(500, new { message = "Lỗi khi xóa phương thức tuyển sinh", error = ex.Message });
            }
        }

        #endregion

        #region Admission News Management

        // POST: api/UniversityView/my-admission-news - Tạo tin tức tuyển sinh mới
        [HttpPost("my-admission-news")]
        [UniversityAuthorize]
        public async Task<ActionResult<AdmissionNew>> CreateMyAdmissionNews(AdmissionNew news)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                news.UniversityId = universityId.Value;
                var createdNews = await _newsService.CreateAdmissionNewAsync(news);
                return CreatedAtAction("GetMyAdmissionNews", createdNews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo tin tức tuyển sinh");
                return StatusCode(500, new { message = "Lỗi khi tạo tin tức tuyển sinh", error = ex.Message });
            }
        }

        // PUT: api/UniversityView/my-admission-news/{id} - Cập nhật tin tức tuyển sinh
        [HttpPut("my-admission-news/{id}")]
        [UniversityAuthorize]
        public async Task<IActionResult> UpdateMyAdmissionNews(int id, AdmissionNew news)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                if (id != news.Id)
                {
                    return BadRequest(new { message = "ID không khớp" });
                }

                var existingNews = await _newsService.GetAdmissionNewByIdAsync(id);
                if (existingNews == null || existingNews.UniversityId != universityId)
                {
                    return NotFound(new { message = "Không tìm thấy tin tức hoặc không thuộc trường của bạn" });
                }

                // Detach existing entity để tránh tracking conflict
                _context.Entry(existingNews).State = EntityState.Detached;
                
                news.UniversityId = universityId.Value;
                await _newsService.UpdateAdmissionNewAsync(news);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật tin tức tuyển sinh");
                return StatusCode(500, new { message = "Lỗi khi cập nhật tin tức tuyển sinh", error = ex.Message });
            }
        }

        // DELETE: api/UniversityView/my-admission-news/{id} - Xóa tin tức tuyển sinh
        [HttpDelete("my-admission-news/{id}")]
        [UniversityAuthorize]
        public async Task<IActionResult> DeleteMyAdmissionNews(int id)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                var news = await _newsService.GetAdmissionNewByIdAsync(id);
                if (news == null || news.UniversityId != universityId)
                {
                    return NotFound(new { message = "Không tìm thấy tin tức hoặc không thuộc trường của bạn" });
                }

                await _newsService.DeleteAdmissionNewAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa tin tức tuyển sinh");
                return StatusCode(500, new { message = "Lỗi khi xóa tin tức tuyển sinh", error = ex.Message });
            }
        }

        #endregion

        #region Scholarships Management

        // POST: api/UniversityView/my-scholarships - Tạo học bổng mới
        [HttpPost("my-scholarships")]
        [UniversityAuthorize]
        public async Task<ActionResult<Scholarship>> CreateMyScholarship(Scholarship scholarship)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                scholarship.UniversityId = universityId.Value;
                var createdScholarship = await _scholarshipService.CreateAsync(scholarship);
                return CreatedAtAction(nameof(GetMyScholarships), createdScholarship);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo học bổng");
                return StatusCode(500, new { message = "Lỗi khi tạo học bổng", error = ex.Message });
            }
        }

        // PUT: api/UniversityView/my-scholarships/{id} - Cập nhật học bổng
        [HttpPut("my-scholarships/{id}")]
        [UniversityAuthorize]
        public async Task<IActionResult> UpdateMyScholarship(int id, Scholarship scholarship)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                if (id != scholarship.Id)
                {
                    return BadRequest(new { message = "ID không khớp" });
                }

                var existingScholarship = await _scholarshipService.GetByIdAsync(id);
                if (existingScholarship == null || existingScholarship.UniversityId != universityId)
                {
                    return NotFound(new { message = "Không tìm thấy học bổng hoặc không thuộc trường của bạn" });
                }

                // Detach existing entity để tránh tracking conflict
                _context.Entry(existingScholarship).State = EntityState.Detached;
                
                scholarship.UniversityId = universityId.Value;
                await _scholarshipService.UpdateAsync(scholarship);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật học bổng");
                return StatusCode(500, new { message = "Lỗi khi cập nhật học bổng", error = ex.Message });
            }
        }

        // DELETE: api/UniversityView/my-scholarships/{id} - Xóa học bổng
        [HttpDelete("my-scholarships/{id}")]
        [UniversityAuthorize]
        public async Task<IActionResult> DeleteMyScholarship(int id)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                var scholarship = await _scholarshipService.GetByIdAsync(id);
                if (scholarship == null || scholarship.UniversityId != universityId)
                {
                    return NotFound(new { message = "Không tìm thấy học bổng hoặc không thuộc trường của bạn" });
                }

                await _scholarshipService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa học bổng");
                return StatusCode(500, new { message = "Lỗi khi xóa học bổng", error = ex.Message });
            }
        }

        #endregion

        #region Admission Scores Management

        // POST: api/UniversityView/my-admission-scores - Tạo điểm chuẩn mới
        [HttpPost("my-admission-scores")]
        [UniversityAuthorize]
        public async Task<ActionResult<AdmissionScore>> CreateMyAdmissionScore(AdmissionScore score)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                // Kiểm tra major thuộc university
                var major = await _majorService.GetByIdAsync(score.MajorId);
                if (major == null || major.UniversityId != universityId)
                {
                    return BadRequest(new { message = "Ngành học không thuộc trường của bạn" });
                }

                var createdScore = await _scoreService.CreateAsync(score);
                return CreatedAtAction(nameof(GetMyAdmissionScores), createdScore);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo điểm chuẩn");
                return StatusCode(500, new { message = "Lỗi khi tạo điểm chuẩn", error = ex.Message });
            }
        }

        // PUT: api/UniversityView/my-admission-scores/{id} - Cập nhật điểm chuẩn
        [HttpPut("my-admission-scores/{id}")]
        [UniversityAuthorize]
        public async Task<IActionResult> UpdateMyAdmissionScore(int id, AdmissionScore score)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                if (id != score.Id)
                {
                    return BadRequest(new { message = "ID không khớp" });
                }

                var existingScore = await _scoreService.GetByIdAsync(id);
                if (existingScore == null || existingScore.Major?.UniversityId != universityId)
                {
                    return NotFound(new { message = "Không tìm thấy điểm chuẩn hoặc không thuộc trường của bạn" });
                }

                // Detach existing entity để tránh tracking conflict
                _context.Entry(existingScore).State = EntityState.Detached;
                
                // Kiểm tra major thuộc university
                var major = await _majorService.GetByIdAsync(score.MajorId);
                if (major == null || major.UniversityId != universityId)
                {
                    return BadRequest(new { message = "Ngành học không thuộc trường của bạn" });
                }

                // Detach major nếu đã được track
                if (_context.Entry(major).State != EntityState.Detached)
                {
                    _context.Entry(major).State = EntityState.Detached;
                }

                await _scoreService.UpdateAsync(score);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật điểm chuẩn");
                return StatusCode(500, new { message = "Lỗi khi cập nhật điểm chuẩn", error = ex.Message });
            }
        }

        // DELETE: api/UniversityView/my-admission-scores/{id} - Xóa điểm chuẩn
        [HttpDelete("my-admission-scores/{id}")]
        [UniversityAuthorize]
        public async Task<IActionResult> DeleteMyAdmissionScore(int id)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                var score = await _scoreService.GetByIdAsync(id);
                if (score == null || score.Major?.UniversityId != universityId)
                {
                    return NotFound(new { message = "Không tìm thấy điểm chuẩn hoặc không thuộc trường của bạn" });
                }

                await _scoreService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa điểm chuẩn");
                return StatusCode(500, new { message = "Lỗi khi xóa điểm chuẩn", error = ex.Message });
            }
        }

        #endregion

        #region Admission Criteria Management

        // POST: api/UniversityView/my-admission-criteria - Tạo tiêu chí tuyển sinh mới
        [HttpPost("my-admission-criteria")]
        [UniversityAuthorize]
        public async Task<ActionResult<AdmissionCriteria>> CreateMyAdmissionCriteria(AdmissionCriteria criteria)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                // Kiểm tra admission method thuộc university
                var method = await _methodService.GetByIdAsync(criteria.AdmissionMethodId);
                if (method == null || method.UniversityId != universityId)
                {
                    return BadRequest(new { message = "Phương thức tuyển sinh không thuộc trường của bạn" });
                }

                var createdCriteria = await _criteriaService.CreateAsync(criteria);
                return CreatedAtAction(nameof(GetMyAdmissionCriteria), createdCriteria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo tiêu chí tuyển sinh");
                return StatusCode(500, new { message = "Lỗi khi tạo tiêu chí tuyển sinh", error = ex.Message });
            }
        }

        // PUT: api/UniversityView/my-admission-criteria/{id} - Cập nhật tiêu chí tuyển sinh
        [HttpPut("my-admission-criteria/{id}")]
        [UniversityAuthorize]
        public async Task<IActionResult> UpdateMyAdmissionCriteria(int id, AdmissionCriteria criteria)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                if (id != criteria.Id)
                {
                    return BadRequest(new { message = "ID không khớp" });
                }

                var existingCriteria = await _criteriaService.GetByIdAsync(id);
                if (existingCriteria == null || existingCriteria.AdmissionMethod?.UniversityId != universityId)
                {
                    return NotFound(new { message = "Không tìm thấy tiêu chí tuyển sinh hoặc không thuộc trường của bạn" });
                }

                // Detach existing entity để tránh tracking conflict
                _context.Entry(existingCriteria).State = EntityState.Detached;
                
                // Kiểm tra admission method thuộc university
                var method = await _methodService.GetByIdAsync(criteria.AdmissionMethodId);
                if (method == null || method.UniversityId != universityId)
                {
                    return BadRequest(new { message = "Phương thức tuyển sinh không thuộc trường của bạn" });
                }

                // Detach method nếu đã được track
                if (_context.Entry(method).State != EntityState.Detached)
                {
                    _context.Entry(method).State = EntityState.Detached;
                }

                await _criteriaService.UpdateAsync(criteria);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật tiêu chí tuyển sinh");
                return StatusCode(500, new { message = "Lỗi khi cập nhật tiêu chí tuyển sinh", error = ex.Message });
            }
        }

        // DELETE: api/UniversityView/my-admission-criteria/{id} - Xóa tiêu chí tuyển sinh
        [HttpDelete("my-admission-criteria/{id}")]
        [UniversityAuthorize]
        public async Task<IActionResult> DeleteMyAdmissionCriteria(int id)
        {
            try
            {
                var universityId = await GetUserUniversityId();
                if (universityId == null)
                {
                    return BadRequest(new { message = "Tài khoản chưa được gán trường đại học" });
                }

                var criteria = await _criteriaService.GetByIdAsync(id);
                if (criteria == null || criteria.AdmissionMethod?.UniversityId != universityId)
                {
                    return NotFound(new { message = "Không tìm thấy tiêu chí tuyển sinh hoặc không thuộc trường của bạn" });
                }

                await _criteriaService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa tiêu chí tuyển sinh");
                return StatusCode(500, new { message = "Lỗi khi xóa tiêu chí tuyển sinh", error = ex.Message });
            }
        }
        

        #endregion

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