using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Services;
using AdmissionInfoSystem.Attributes;
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
            var majors = await _majorService.GetAllAsync();
            return Ok(majors);
        }

        // GET: api/Majors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Major>> GetMajor(int id)
        {
            var major = await _majorService.GetByIdAsync(id);

            if (major == null)
            {
                return NotFound();
            }

            return Ok(major);
        }

        // GET: api/Majors/University/5
        [HttpGet("University/{universityId}")]
        public async Task<ActionResult<IEnumerable<Major>>> GetMajorsByUniversity(int universityId)
        {
            var majors = await _majorService.GetByUniversityIdAsync(universityId);
            return Ok(majors);
        }

        // POST: api/Majors
        [HttpPost]
        [AdminAuthorize]
        public async Task<ActionResult<Major>> PostMajor(Major major)
        {
            var createdMajor = await _majorService.CreateAsync(major);
            return CreatedAtAction(nameof(GetMajor), new { id = createdMajor.Id }, createdMajor);
        }

        // PUT: api/Majors/5
        [HttpPut("{id}")]
        [AdminAuthorize]
        public async Task<IActionResult> PutMajor(int id, Major major)
        {
            if (id != major.Id)
            {
                return BadRequest();
            }

            await _majorService.UpdateAsync(major);
            return NoContent();
        }

        // DELETE: api/Majors/5
        [HttpDelete("{id}")]
        [AdminAuthorize]
        public async Task<IActionResult> DeleteMajor(int id)
        {
            await _majorService.DeleteAsync(id);
            return NoContent();
        }
    }
} 