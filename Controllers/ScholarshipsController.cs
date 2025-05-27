using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdmissionInfoSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScholarshipsController : ControllerBase
    {
        private readonly IScholarshipService _scholarshipService;

        public ScholarshipsController(IScholarshipService scholarshipService)
        {
            _scholarshipService = scholarshipService;
        }

        // GET: api/Scholarships
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Scholarship>>> GetScholarships()
        {
            var scholarships = await _scholarshipService.GetAllAsync();
            return Ok(scholarships);
        }

        // GET: api/Scholarships/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Scholarship>> GetScholarship(int id)
        {
            var scholarship = await _scholarshipService.GetByIdAsync(id);

            if (scholarship == null)
            {
                return NotFound();
            }

            return Ok(scholarship);
        }

        // GET: api/Scholarships/University/5
        [HttpGet("University/{universityId}")]
        public async Task<ActionResult<IEnumerable<Scholarship>>> GetScholarshipsByUniversity(int universityId)
        {
            var scholarships = await _scholarshipService.GetByUniversityIdAsync(universityId);
            return Ok(scholarships);
        }

        // POST: api/Scholarships
        [HttpPost]
        public async Task<ActionResult<Scholarship>> PostScholarship(Scholarship scholarship)
        {
            var createdScholarship = await _scholarshipService.CreateAsync(scholarship);
            return CreatedAtAction(nameof(GetScholarship), new { id = createdScholarship.Id }, createdScholarship);
        }

        // PUT: api/Scholarships/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutScholarship(int id, Scholarship scholarship)
        {
            if (id != scholarship.Id)
            {
                return BadRequest();
            }

            await _scholarshipService.UpdateAsync(scholarship);
            return NoContent();
        }

        // DELETE: api/Scholarships/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScholarship(int id)
        {
            await _scholarshipService.DeleteAsync(id);
            return NoContent();
        }
    }
} 