using AdmissionInfoSystem.DTOs;
using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Services.Interface;
using AdmissionInfoSystem.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace AdmissionInfoSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdmissionScoresController : ControllerBase
    {
        private readonly IAdmissionScoreService _admissionScoreService;

        public AdmissionScoresController(IAdmissionScoreService admissionScoreService)
        {
            _admissionScoreService = admissionScoreService;
        }

        // GET: api/AdmissionScores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdmissionScoreDTO>>> GetAdmissionScores()
        {
            try
            {
                var admissionScores = await _admissionScoreService.GetAllAsync();
                var result = admissionScores.Select(ads => new AdmissionScoreDTO
                {
                    Id = ads.Id,
                    MajorId = ads.MajorId,
                    Year = ads.Year,
                    Score = ads.Score,
                    AdmissionMethodId = ads.AdmissionMethodId,
                    Note = ads.Note,
                    SubjectCombination = ads.SubjectCombination,
                    MajorName = ads.Major?.Name,
                    UniversityName = ads.Major?.University?.Name,
                    AdmissionMethodName = ads.AdmissionMethod?.Name
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách điểm chuẩn", error = ex.Message });
            }
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedAdmissionScoreDTO>> GetPagedAdmissionScores(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var pagedResult = await _admissionScoreService.GetPagedAsync(page, pageSize);
                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách điểm chuẩn phân trang", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdmissionScoreDTO>> GetAdmissionScore(int id)
        {
            try
            {
                var admissionScore = await _admissionScoreService.GetByIdAsync(id);

                if (admissionScore == null)
                {
                    return NotFound(new { message = "Không tìm thấy điểm chuẩn" });
                }

                var result = new AdmissionScoreDTO
                {
                    Id = admissionScore.Id,
                    MajorId = admissionScore.MajorId,
                    Year = admissionScore.Year,
                    Score = admissionScore.Score,
                    AdmissionMethodId = admissionScore.AdmissionMethodId,
                    Note = admissionScore.Note,
                    SubjectCombination = admissionScore.SubjectCombination,
                    MajorName = admissionScore.Major?.Name,
                    UniversityName = admissionScore.Major?.University?.Name,
                    AdmissionMethodName = admissionScore.AdmissionMethod?.Name
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin điểm chuẩn", error = ex.Message });
            }
        }

        [HttpGet("major/{majorId}")]
        public async Task<ActionResult<IEnumerable<AdmissionScoreDTO>>> GetAdmissionScoresByMajor(int majorId)
        {
            try
            {
                var admissionScores = await _admissionScoreService.GetByMajorIdAsync(majorId);
                var result = admissionScores.Select(ads => new AdmissionScoreDTO
                {
                    Id = ads.Id,
                    MajorId = ads.MajorId,
                    Year = ads.Year,
                    Score = ads.Score,
                    AdmissionMethodId = ads.AdmissionMethodId,
                    Note = ads.Note,
                    SubjectCombination = ads.SubjectCombination,
                    MajorName = ads.Major?.Name,
                    UniversityName = ads.Major?.University?.Name,
                    AdmissionMethodName = ads.AdmissionMethod?.Name
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy điểm chuẩn theo ngành", error = ex.Message });
            }
        }

        [HttpGet("major/{majorId}/paged")]
        public async Task<ActionResult<PagedAdmissionScoreDTO>> GetPagedAdmissionScoresByMajor(
            int majorId,
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var pagedResult = await _admissionScoreService.GetPagedByMajorIdAsync(majorId, page, pageSize);
                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy điểm chuẩn phân trang theo ngành", error = ex.Message });
            }
        }

        [HttpGet("year/{year}")]
        public async Task<ActionResult<IEnumerable<AdmissionScoreDTO>>> GetAdmissionScoresByYear(int year)
        {
            try
            {
                var admissionScores = await _admissionScoreService.GetByYearAsync(year);
                var result = admissionScores.Select(ads => new AdmissionScoreDTO
                {
                    Id = ads.Id,
                    MajorId = ads.MajorId,
                    Year = ads.Year,
                    Score = ads.Score,
                    AdmissionMethodId = ads.AdmissionMethodId,
                    Note = ads.Note,
                    SubjectCombination = ads.SubjectCombination,
                    MajorName = ads.Major?.Name,
                    UniversityName = ads.Major?.University?.Name,
                    AdmissionMethodName = ads.AdmissionMethod?.Name
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy điểm chuẩn theo năm", error = ex.Message });
            }
        }

        [HttpGet("year/{year}/paged")]
        public async Task<ActionResult<PagedAdmissionScoreDTO>> GetPagedAdmissionScoresByYear(
            int year,
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var pagedResult = await _admissionScoreService.GetPagedByYearAsync(year, page, pageSize);
                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy điểm chuẩn phân trang theo năm", error = ex.Message });
            }
        }

        [HttpGet("major/{majorId}/year/{year}")]
        public async Task<ActionResult<IEnumerable<AdmissionScoreDTO>>> GetAdmissionScoresByMajorAndYear(int majorId, int year)
        {
            try
            {
                var admissionScores = await _admissionScoreService.GetByMajorAndYearAsync(majorId, year);
                var result = admissionScores.Select(ads => new AdmissionScoreDTO
                {
                    Id = ads.Id,
                    MajorId = ads.MajorId,
                    Year = ads.Year,
                    Score = ads.Score,
                    AdmissionMethodId = ads.AdmissionMethodId,
                    Note = ads.Note,
                    SubjectCombination = ads.SubjectCombination,
                    MajorName = ads.Major?.Name,
                    UniversityName = ads.Major?.University?.Name,
                    AdmissionMethodName = ads.AdmissionMethod?.Name
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy điểm chuẩn theo ngành và năm", error = ex.Message });
            }
        }

        [HttpGet("university/{universityId}")]
        public async Task<ActionResult<IEnumerable<AdmissionScoreDTO>>> GetAdmissionScoresByUniversity(int universityId)
        {
            try
            {
                var admissionScores = await _admissionScoreService.GetByUniversityIdAsync(universityId);

                var result = admissionScores.Select(ads => new AdmissionScoreDTO
                {
                    Id = ads.Id,
                    MajorId = ads.MajorId,
                    Year = ads.Year,
                    Score = ads.Score,
                    AdmissionMethodId = ads.AdmissionMethodId,
                    Note = ads.Note,
                    SubjectCombination = ads.SubjectCombination,
                    MajorName = ads.Major?.Name,
                    UniversityName = ads.Major?.University?.Name,
                    AdmissionMethodName = ads.AdmissionMethod?.Name
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy điểm chuẩn theo trường đại học", error = ex.Message });
            }
        }

        [HttpPost]
        [AdminAuthorize]
        public async Task<ActionResult<AdmissionScoreDTO>> PostAdmissionScore(CreateAdmissionScoreDTO createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var admissionScore = new AdmissionScore
                {
                    MajorId = createDto.MajorId,
                    Year = createDto.Year,
                    Score = createDto.Score,
                    AdmissionMethodId = createDto.AdmissionMethodId,
                    Note = createDto.Note,
                    SubjectCombination = createDto.SubjectCombination
                };

                var createdScore = await _admissionScoreService.CreateAsync(admissionScore);

                var result = new AdmissionScoreDTO
                {
                    Id = createdScore.Id,
                    MajorId = createdScore.MajorId,
                    Year = createdScore.Year,
                    Score = createdScore.Score,
                    AdmissionMethodId = createdScore.AdmissionMethodId,
                    Note = createdScore.Note,
                    SubjectCombination = createdScore.SubjectCombination,
                    MajorName = createdScore.Major?.Name,
                    UniversityName = createdScore.Major?.University?.Name,
                    AdmissionMethodName = createdScore.AdmissionMethod?.Name
                };

                return CreatedAtAction(nameof(GetAdmissionScore), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo điểm chuẩn", error = ex.Message });
            }
        }

        // PUT: api/AdmissionScores/5
        [HttpPut("{id}")]
        [AdminAuthorize]
        public async Task<IActionResult> PutAdmissionScore(int id, AdmissionScoreDTO scoreDto)
        {
            try
            {
                if (id != scoreDto.Id)
                {
                    return BadRequest(new { message = "ID không khớp" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var admissionScore = new AdmissionScore
                {
                    Id = scoreDto.Id,
                    MajorId = scoreDto.MajorId,
                    Year = scoreDto.Year,
                    Score = scoreDto.Score,
                    AdmissionMethodId = scoreDto.AdmissionMethodId,
                    Note = scoreDto.Note,
                    SubjectCombination = scoreDto.SubjectCombination
                };

                await _admissionScoreService.UpdateAsync(admissionScore);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật điểm chuẩn", error = ex.Message });
            }
        }

        // DELETE: api/AdmissionScores/5
        [HttpDelete("{id}")]
        [AdminAuthorize]
        public async Task<IActionResult> DeleteAdmissionScore(int id)
        {
            try
            {
                var success = await _admissionScoreService.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Không tìm thấy điểm chuẩn" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa điểm chuẩn", error = ex.Message });
            }
        }

        // GET: api/AdmissionScores/exists/5
        [HttpGet("exists/{id}")]
        public async Task<ActionResult<bool>> AdmissionScoreExists(int id)
        {
            try
            {
                var exists = await _admissionScoreService.ExistsAsync(id);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi kiểm tra điểm chuẩn", error = ex.Message });
            }
        }
    }
} 