using AdmissionInfoSystem.DTOs;
using AdmissionInfoSystem.Models;
using AdmissionInfoSystem.Services;
using AdmissionInfoSystem.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace AdmissionInfoSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdmissionNewsController : ControllerBase
    {
        private readonly IAdmissionNewService _admissionNewService;

        public AdmissionNewsController(IAdmissionNewService admissionNewService)
        {
            _admissionNewService = admissionNewService;
        }

        // GET: api/AdmissionNews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdmissionNewDTO>>> GetAdmissionNews()
        {
            var admissionNews = await _admissionNewService.GetAllAdmissionNewsAsync();
            var admissionNewDtos = admissionNews.Select(an => new AdmissionNewDTO
            {
                Id = an.Id,
                Title = an.Title,
                Content = an.Content,
                PublishDate = an.PublishDate,
                Year = an.Year,
                UniversityId = an.UniversityId,
                UniversityName = an.University?.Name ?? string.Empty
            });

            return Ok(admissionNewDtos);
        }

        // GET: api/AdmissionNews/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdmissionNewDTO>> GetAdmissionNew(int id)
        {
            var admissionNew = await _admissionNewService.GetAdmissionNewByIdAsync(id);

            if (admissionNew == null)
            {
                return NotFound();
            }

            var admissionNewDto = new AdmissionNewDTO
            {
                Id = admissionNew.Id,
                Title = admissionNew.Title,
                Content = admissionNew.Content,
                PublishDate = admissionNew.PublishDate,
                Year = admissionNew.Year,
                UniversityId = admissionNew.UniversityId,
                UniversityName = admissionNew.University?.Name ?? string.Empty
            };

            return admissionNewDto;
        }

        // GET: api/AdmissionNews/university/5
        [HttpGet("university/{universityId}")]
        public async Task<ActionResult<IEnumerable<AdmissionNewDTO>>> GetAdmissionNewsByUniversity(int universityId)
        {
            var admissionNews = await _admissionNewService.GetAdmissionNewsByUniversityAsync(universityId);
            var admissionNewDtos = admissionNews.Select(an => new AdmissionNewDTO
            {
                Id = an.Id,
                Title = an.Title,
                Content = an.Content,
                PublishDate = an.PublishDate,
                Year = an.Year,
                UniversityId = an.UniversityId,
                UniversityName = an.University?.Name ?? string.Empty
            });

            return Ok(admissionNewDtos);
        }

        // GET: api/AdmissionNews/latest/5
        [HttpGet("latest/{count}")]
        public async Task<ActionResult<IEnumerable<AdmissionNewDTO>>> GetLatestAdmissionNews(int count)
        {
            var admissionNews = await _admissionNewService.GetLatestAdmissionNewsAsync(count);
            var admissionNewDtos = admissionNews.Select(an => new AdmissionNewDTO
            {
                Id = an.Id,
                Title = an.Title,
                Content = an.Content,
                PublishDate = an.PublishDate,
                Year = an.Year,
                UniversityId = an.UniversityId,
                UniversityName = an.University?.Name ?? string.Empty
            });

            return Ok(admissionNewDtos);
        }

        // POST: api/AdmissionNews
        [HttpPost]
        [AdminAuthorize]
        public async Task<ActionResult<AdmissionNewDTO>> PostAdmissionNew(AdmissionNewCreateDTO admissionNewDto)
        {
            var admissionNew = new AdmissionNew
            {
                Title = admissionNewDto.Title,
                Content = admissionNewDto.Content,
                PublishDate = admissionNewDto.PublishDate,
                Year = admissionNewDto.Year,
                UniversityId = admissionNewDto.UniversityId
            };

            await _admissionNewService.CreateAdmissionNewAsync(admissionNew);

            var resultDto = new AdmissionNewDTO
            {
                Id = admissionNew.Id,
                Title = admissionNew.Title,
                Content = admissionNew.Content,
                PublishDate = admissionNew.PublishDate,
                Year = admissionNew.Year,
                UniversityId = admissionNew.UniversityId
            };

            return CreatedAtAction(nameof(GetAdmissionNew), new { id = admissionNew.Id }, resultDto);
        }

        // PUT: api/AdmissionNews/5
        [HttpPut("{id}")]
        [AdminAuthorize]
        public async Task<IActionResult> PutAdmissionNew(int id, AdmissionNewUpdateDTO admissionNewDto)
        {
            if (id != admissionNewDto.Id)
            {
                return BadRequest();
            }

            var admissionNew = await _admissionNewService.GetAdmissionNewByIdAsync(id);
            if (admissionNew == null)
            {
                return NotFound();
            }

            admissionNew.Title = admissionNewDto.Title;
            admissionNew.Content = admissionNewDto.Content;
            admissionNew.PublishDate = admissionNewDto.PublishDate;
            admissionNew.Year = admissionNewDto.Year;
            admissionNew.UniversityId = admissionNewDto.UniversityId;

            await _admissionNewService.UpdateAdmissionNewAsync(admissionNew);

            return NoContent();
        }

        // DELETE: api/AdmissionNews/5
        [HttpDelete("{id}")]
        [AdminAuthorize]
        public async Task<IActionResult> DeleteAdmissionNew(int id)
        {
            var admissionNew = await _admissionNewService.GetAdmissionNewByIdAsync(id);
            if (admissionNew == null)
            {
                return NotFound();
            }

            await _admissionNewService.DeleteAdmissionNewAsync(id);

            return NoContent();
        }
    }
} 