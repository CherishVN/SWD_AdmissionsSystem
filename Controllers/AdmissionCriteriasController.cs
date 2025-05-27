using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdmissionInfoSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdmissionCriteriasController : ControllerBase
    {
        private readonly IAdmissionCriteriaService _criteriaService;

        public AdmissionCriteriasController(IAdmissionCriteriaService criteriaService)
        {
            _criteriaService = criteriaService;
        }

        // GET: api/AdmissionCriterias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdmissionCriteria>>> GetAdmissionCriterias()
        {
            var criterias = await _criteriaService.GetAllAsync();
            return Ok(criterias);
        }

        // GET: api/AdmissionCriterias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdmissionCriteria>> GetAdmissionCriteria(int id)
        {
            var criteria = await _criteriaService.GetByIdAsync(id);

            if (criteria == null)
            {
                return NotFound();
            }

            return Ok(criteria);
        }

        // GET: api/AdmissionCriterias/AdmissionMethod/5
        [HttpGet("AdmissionMethod/{admissionMethodId}")]
        public async Task<ActionResult<IEnumerable<AdmissionCriteria>>> GetCriteriasByAdmissionMethod(int admissionMethodId)
        {
            var criterias = await _criteriaService.GetByAdmissionMethodIdAsync(admissionMethodId);
            return Ok(criterias);
        }

        // POST: api/AdmissionCriterias
        [HttpPost]
        public async Task<ActionResult<AdmissionCriteria>> PostAdmissionCriteria(AdmissionCriteria criteria)
        {
            var createdCriteria = await _criteriaService.CreateAsync(criteria);
            return CreatedAtAction(nameof(GetAdmissionCriteria), new { id = createdCriteria.Id }, createdCriteria);
        }

        // PUT: api/AdmissionCriterias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdmissionCriteria(int id, AdmissionCriteria criteria)
        {
            if (id != criteria.Id)
            {
                return BadRequest();
            }

            await _criteriaService.UpdateAsync(criteria);
            return NoContent();
        }

        // DELETE: api/AdmissionCriterias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmissionCriteria(int id)
        {
            await _criteriaService.DeleteAsync(id);
            return NoContent();
        }
    }
} 