using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Services;
using AdmissionInfoSystem.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace AdmissionInfoSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdmissionMethodsController : ControllerBase
    {
        private readonly IAdmissionMethodService _methodService;

        public AdmissionMethodsController(IAdmissionMethodService methodService)
        {
            _methodService = methodService;
        }

        // GET: api/AdmissionMethods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdmissionMethod>>> GetAdmissionMethods()
        {
            var methods = await _methodService.GetAllAsync();
            return Ok(methods);
        }

        // GET: api/AdmissionMethods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdmissionMethod>> GetAdmissionMethod(int id)
        {
            var method = await _methodService.GetByIdAsync(id);

            if (method == null)
            {
                return NotFound();
            }

            return Ok(method);
        }

        // GET: api/AdmissionMethods/University/5
        [HttpGet("University/{universityId}")]
        public async Task<ActionResult<IEnumerable<AdmissionMethod>>> GetMethodsByUniversity(int universityId)
        {
            var methods = await _methodService.GetByUniversityIdAsync(universityId);
            return Ok(methods);
        }

        // POST: api/AdmissionMethods
        [HttpPost]
        [AdminAuthorize]
        public async Task<ActionResult<AdmissionMethod>> PostAdmissionMethod(AdmissionMethod method)
        {
            var createdMethod = await _methodService.CreateAsync(method);
            return CreatedAtAction(nameof(GetAdmissionMethod), new { id = createdMethod.Id }, createdMethod);
        }

        // PUT: api/AdmissionMethods/5
        [HttpPut("{id}")]
        [AdminAuthorize]
        public async Task<IActionResult> PutAdmissionMethod(int id, AdmissionMethod method)
        {
            if (id != method.Id)
            {
                return BadRequest();
            }

            await _methodService.UpdateAsync(method);
            return NoContent();
        }

        // DELETE: api/AdmissionMethods/5
        [HttpDelete("{id}")]
        [AdminAuthorize]
        public async Task<IActionResult> DeleteAdmissionMethod(int id)
        {
            await _methodService.DeleteAsync(id);
            return NoContent();
        }
    }
} 