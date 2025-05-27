using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdmissionInfoSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicProgramsController : ControllerBase
    {
        private readonly IAcademicProgramService _programService;

        public AcademicProgramsController(IAcademicProgramService programService)
        {
            _programService = programService;
        }

        // GET: api/AcademicPrograms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AcademicProgram>>> GetPrograms()
        {
            var programs = await _programService.GetAllAsync();
            return Ok(programs);
        }

        // GET: api/AcademicPrograms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AcademicProgram>> GetProgram(int id)
        {
            var program = await _programService.GetByIdAsync(id);

            if (program == null)
            {
                return NotFound();
            }

            return Ok(program);
        }

        // GET: api/AcademicPrograms/University/5
        [HttpGet("University/{universityId}")]
        public async Task<ActionResult<IEnumerable<AcademicProgram>>> GetProgramsByUniversity(int universityId)
        {
            var programs = await _programService.GetByUniversityIdAsync(universityId);
            return Ok(programs);
        }

        // POST: api/AcademicPrograms
        [HttpPost]
        public async Task<ActionResult<AcademicProgram>> PostProgram(AcademicProgram program)
        {
            var createdProgram = await _programService.CreateAsync(program);
            return CreatedAtAction(nameof(GetProgram), new { id = createdProgram.Id }, createdProgram);
        }

        // PUT: api/AcademicPrograms/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProgram(int id, AcademicProgram program)
        {
            if (id != program.Id)
            {
                return BadRequest();
            }

            await _programService.UpdateAsync(program);
            return NoContent();
        }

        // DELETE: api/AcademicPrograms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProgram(int id)
        {
            await _programService.DeleteAsync(id);
            return NoContent();
        }
    }
} 