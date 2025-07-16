using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Services;
using AdmissionInfoSystem.Attributes;
using AdmissionInfoSystem.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AdmissionInfoSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MajorsController : ControllerBase
    {
        private readonly IMajorService _majorService;

        public MajorsController(IMajorService majorService)
        {
            _majorService = majorService;
        }

        // GET: api/Majors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Major>>> GetMajors()
        {
            try
            {
                var majors = await _majorService.GetAllAsync();
                return Ok(majors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách ngành học", error = ex.Message });
            }
        }

        // GET: api/Majors/paged?page=1&pageSize=10
        [HttpGet("paged")]
        public async Task<ActionResult<PagedMajorDTO>> GetPagedMajors(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Validate parameters
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10; // Giới hạn tối đa 100 items/page

                var pagedResult = await _majorService.GetPagedAsync(page, pageSize);
                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách ngành học phân trang", error = ex.Message });
            }
        }

        // GET: api/Majors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Major>> GetMajor(int id)
        {
            try
            {
                var major = await _majorService.GetByIdAsync(id);

                if (major == null)
                {
                    return NotFound(new { message = "Không tìm thấy ngành học" });
                }

                return Ok(major);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin ngành học", error = ex.Message });
            }
        }

        // GET: api/Majors/University/5
        [HttpGet("University/{universityId}")]
        public async Task<ActionResult<IEnumerable<Major>>> GetMajorsByUniversity(int universityId)
        {
            try
            {
                var majors = await _majorService.GetByUniversityIdAsync(universityId);
                return Ok(majors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy ngành học theo trường đại học", error = ex.Message });
            }
        }

        // GET: api/Majors/University/5/paged?page=1&pageSize=10
        [HttpGet("University/{universityId}/paged")]
        public async Task<ActionResult<PagedMajorDTO>> GetPagedMajorsByUniversity(
            int universityId,
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Validate parameters
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var pagedResult = await _majorService.GetPagedByUniversityIdAsync(universityId, page, pageSize);
                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy ngành học phân trang theo trường đại học", error = ex.Message });
            }
        }

        // POST: api/Majors
        [HttpPost]
        [AdminAuthorize]
        public async Task<ActionResult<Major>> PostMajor(Major major)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdMajor = await _majorService.CreateAsync(major);
                return CreatedAtAction(nameof(GetMajor), new { id = createdMajor.Id }, createdMajor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo ngành học", error = ex.Message });
            }
        }

        // PUT: api/Majors/5
        [HttpPut("{id}")]
        [AdminAuthorize]
        public async Task<IActionResult> PutMajor(int id, Major major)
        {
            try
            {
                if (id != major.Id)
                {
                    return BadRequest(new { message = "ID không khớp" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _majorService.UpdateAsync(major);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật ngành học", error = ex.Message });
            }
        }

        // DELETE: api/Majors/5
        [HttpDelete("{id}")]
        [AdminAuthorize]
        public async Task<IActionResult> DeleteMajor(int id)
        {
            try
            {
                await _majorService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa ngành học", error = ex.Message });
            }
        }
    }
} 