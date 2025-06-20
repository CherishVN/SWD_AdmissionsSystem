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

        // GET: api/AdmissionScores/5
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

        // GET: api/AdmissionScores/major/5
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

        // GET: api/AdmissionScores/year/2024
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

        // GET: api/AdmissionScores/major/5/year/2024
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

        // POST: api/AdmissionScores
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
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo điểm chuẩn", error = ex.Message });
            }
        }

        // PUT: api/AdmissionScores/5
        [HttpPut("{id}")]
        [AdminAuthorize]
        public async Task<IActionResult> PutAdmissionScore(int id, UpdateAdmissionScoreDTO updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                {
                    return BadRequest(new { message = "ID không khớp" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var admissionScore = new AdmissionScore
                {
                    Id = updateDto.Id,
                    MajorId = updateDto.MajorId,
                    Year = updateDto.Year,
                    Score = updateDto.Score,
                    AdmissionMethodId = updateDto.AdmissionMethodId,
                    Note = updateDto.Note,
                    SubjectCombination = updateDto.SubjectCombination
                };

                var updatedScore = await _admissionScoreService.UpdateAsync(admissionScore);

                var result = new AdmissionScoreDTO
                {
                    Id = updatedScore.Id,
                    MajorId = updatedScore.MajorId,
                    Year = updatedScore.Year,
                    Score = updatedScore.Score,
                    AdmissionMethodId = updatedScore.AdmissionMethodId,
                    Note = updatedScore.Note,
                    SubjectCombination = updatedScore.SubjectCombination,
                    MajorName = updatedScore.Major?.Name,
                    UniversityName = updatedScore.Major?.University?.Name,
                    AdmissionMethodName = updatedScore.AdmissionMethod?.Name
                };

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
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
                var result = await _admissionScoreService.DeleteAsync(id);

                if (!result)
                {
                    return NotFound(new { message = "Không tìm thấy điểm chuẩn để xóa" });
                }

                return Ok(new { message = "Xóa điểm chuẩn thành công" });
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