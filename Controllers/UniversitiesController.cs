using AdmissionInfoSystem.DTOs;
using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdmissionInfoSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniversitiesController : ControllerBase
    {
        private readonly IUniversityService _universityService;

        public UniversitiesController(IUniversityService universityService)
        {
            _universityService = universityService;
        }

        // GET: api/Universities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UniversityDTO>>> GetUniversities()
        {
            var universities = await _universityService.GetAllUniversitiesAsync();
            var universityDtos = universities.Select(u => new UniversityDTO
            {
                Id = u.Id,
                Name = u.Name,
                ShortName = u.ShortName,
                Introduction = u.Introduction,
                OfficialWebsite = u.OfficialWebsite,
                AdmissionWebsite = u.AdmissionWebsite,
                Ranking = u.Ranking,
                RankingCriteria = u.RankingCriteria
            });

            return Ok(universityDtos);
        }

        // GET: api/Universities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UniversityDTO>> GetUniversity(int id)
        {
            var university = await _universityService.GetUniversityByIdAsync(id);

            if (university == null)
            {
                return NotFound();
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
                RankingCriteria = university.RankingCriteria
            };

            return universityDto;
        }

        // GET: api/Universities/5/details
        [HttpGet("{id}/details")]
        public async Task<ActionResult<UniversityDTO>> GetUniversityDetails(int id)
        {
            var university = await _universityService.GetUniversityWithDetailsAsync(id);

            if (university == null)
            {
                return NotFound();
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
                RankingCriteria = university.RankingCriteria
            };

            return universityDto;
        }

        // POST: api/Universities
        [HttpPost]
        public async Task<ActionResult<UniversityDTO>> PostUniversity(UniversityDTO universityDto)
        {
            var university = new University
            {
                Name = universityDto.Name,
                ShortName = universityDto.ShortName,
                Introduction = universityDto.Introduction,
                OfficialWebsite = universityDto.OfficialWebsite,
                AdmissionWebsite = universityDto.AdmissionWebsite,
                Ranking = universityDto.Ranking,
                RankingCriteria = universityDto.RankingCriteria
            };

            await _universityService.CreateUniversityAsync(university);

            universityDto.Id = university.Id;

            return CreatedAtAction(nameof(GetUniversity), new { id = university.Id }, universityDto);
        }

        // PUT: api/Universities/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUniversity(int id, UniversityDTO universityDto)
        {
            if (id != universityDto.Id)
            {
                return BadRequest();
            }

            var university = await _universityService.GetUniversityByIdAsync(id);
            if (university == null)
            {
                return NotFound();
            }

            university.Name = universityDto.Name;
            university.ShortName = universityDto.ShortName;
            university.Introduction = universityDto.Introduction;
            university.OfficialWebsite = universityDto.OfficialWebsite;
            university.AdmissionWebsite = universityDto.AdmissionWebsite;
            university.Ranking = universityDto.Ranking;
            university.RankingCriteria = universityDto.RankingCriteria;

            await _universityService.UpdateUniversityAsync(university);

            return NoContent();
        }

        // DELETE: api/Universities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUniversity(int id)
        {
            var university = await _universityService.GetUniversityByIdAsync(id);
            if (university == null)
            {
                return NotFound();
            }

            await _universityService.DeleteUniversityAsync(id);

            return NoContent();
        }
    }
} 