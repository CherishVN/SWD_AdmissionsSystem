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
                RankingCriteria = u.RankingCriteria,
                Locations = u.Locations,
                Type = u.Type
            });

            return Ok(universityDtos);
        }

        // GET: api/Universities/type/Công lập
        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<UniversityDTO>>> GetUniversitiesByType(string type)
        {
            try
            {
                if (type != "Công lập" && type != "Tư thục")
                {
                    return BadRequest(new { message = "Loại trường phải là 'Công lập' hoặc 'Tư thục'" });
                }

                var universities = await _universityService.GetAllUniversitiesAsync();
                var filteredUniversities = universities.Where(u => u.Type == type);
                
                var universityDtos = filteredUniversities.Select(u => new UniversityDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    ShortName = u.ShortName,
                    Introduction = u.Introduction,
                    OfficialWebsite = u.OfficialWebsite,
                    AdmissionWebsite = u.AdmissionWebsite,
                    Ranking = u.Ranking,
                    RankingCriteria = u.RankingCriteria,
                    Locations = u.Locations,
                    Type = u.Type
                });

                return Ok(universityDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lọc trường đại học theo loại", error = ex.Message });
            }
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
                RankingCriteria = university.RankingCriteria,
                Locations = university.Locations,
                Type = university.Type
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
                RankingCriteria = university.RankingCriteria,
                Locations = university.Locations,
                Type = university.Type
            };

            return universityDto;
        }

        // POST: api/Universities
        [HttpPost]
        public async Task<ActionResult<UniversityDTO>> PostUniversity(CreateUniversityDTO createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var university = new University
                {
                    Name = createDto.Name,
                    ShortName = createDto.ShortName,
                    Introduction = createDto.Introduction,
                    OfficialWebsite = createDto.OfficialWebsite,
                    AdmissionWebsite = createDto.AdmissionWebsite,
                    Ranking = createDto.Ranking,
                    RankingCriteria = createDto.RankingCriteria,
                    Locations = createDto.Locations,
                    Type = createDto.Type
                };

                await _universityService.CreateUniversityAsync(university);

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
                    Type = university.Type
                };

                return CreatedAtAction(nameof(GetUniversity), new { id = university.Id }, universityDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo trường đại học", error = ex.Message });
            }
        }

        // PUT: api/Universities/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUniversity(int id, UpdateUniversityDTO updateDto)
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

                var university = await _universityService.GetUniversityByIdAsync(id);
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
                university.Type = updateDto.Type;

                await _universityService.UpdateUniversityAsync(university);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật trường đại học", error = ex.Message });
            }
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