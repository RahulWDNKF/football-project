using FootballDashboardAPI.Models;
using FootballDashboardAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballDashboardAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyProfileController : ControllerBase
    {
        private readonly ICompanyProfileRepository _repository;
        private readonly IWebHostEnvironment _env;
        private readonly string _uploadsPath;

        public CompanyProfileController(ICompanyProfileRepository repository, IWebHostEnvironment env)
        {
            _repository = repository;
            _env = env;
            _uploadsPath = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads");
            Directory.CreateDirectory(_uploadsPath);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var profile = await _repository.GetAsync();
            if (profile == null)
                return Ok(new CompanyProfileDto());
            // Map to DTO
            var dto = new CompanyProfileDto
            {
                CompanyName = profile.CompanyName,
                ShortName = profile.ShortName,
                Tagline = profile.Tagline,
                Description = profile.Description,
                FoundedYear = profile.FoundedYear,
                LogoUrl = profile.LogoUrl,
                PrimaryColor = profile.PrimaryColor,
                Email = profile.Email,
                PhoneNumber = profile.PhoneNumber,
                AlternatePhone = profile.AlternatePhone,
                AddressLine1 = profile.AddressLine1,
                AddressLine2 = profile.AddressLine2,
                AreaLocality = profile.AreaLocality,
                City = profile.City,
                District = profile.District,
                State = profile.State,
                Country = profile.Country,
                PostalCode = profile.PostalCode,
                OrganizationType = profile.OrganizationType,
                SportType = profile.SportType,
                FacebookUrl = profile.FacebookUrl,
                InstagramUrl = profile.InstagramUrl,
                TwitterUrl = profile.TwitterUrl,
                LinkedinUrl = profile.LinkedinUrl,
                YoutubeUrl = profile.YoutubeUrl
            };
            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upsert([FromBody] CompanyProfileDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CompanyName))
                return BadRequest(new { message = "Company name is required." });
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { message = "Email is required." });
            // Add more validation as needed

            var entity = await _repository.UpsertAsync(dto);
            return Ok(entity);
        }

        [HttpPost("upload-logo")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadLogo([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file provided" });
            }

            var fileName = $"company-logo{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_uploadsPath, fileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            //var relativeUrl = $"/uploads/{fileName}";
            var relativeUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
            return Ok(new { logoUrl = relativeUrl });
        }
    }
}
