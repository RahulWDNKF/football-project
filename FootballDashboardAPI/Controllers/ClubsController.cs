using FootballDashboardAPI.Models;
using FootballDashboardAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballDashboardAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ClubsController : ControllerBase
{
    private readonly IClubService _clubService;

    public ClubsController(IClubService clubService)
    {
        _clubService = clubService;
    }

    // GET: api/clubs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Club>>> GetClubs()
    {
        var clubs = await _clubService.GetAllClubsAsync();
        return Ok(clubs);
    }

    // GET: api/clubs/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Club>> GetClub(string id)
    {
        var club = await _clubService.GetClubByIdAsync(id);
        
        if (club == null)
        {
            return NotFound(new { message = $"Club with ID '{id}' not found." });
        }

        return Ok(club);
    }

    // POST: api/clubs
    [HttpPost]
    public async Task<ActionResult<Club>> CreateClub(CreateClub createClubDto)
    {
        try
        {
            // Validate input
            if (createClubDto == null)
            {
                return BadRequest(new { message = "Request body is required." });
            }

            var club = await _clubService.CreateClubAsync(createClubDto);
            return CreatedAtAction(nameof(GetClub), new { id = club.ClubId }, club);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            // Log unexpected errors
            Console.WriteLine($"Unexpected error in CreateClub: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            
            return StatusCode(500, new 
            { 
                message = "An unexpected error occurred while creating the club.",
                details = ex.Message,
                innerException = ex.InnerException?.Message
            });
        }
    }

    // PUT: api/clubs/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<Club>> UpdateClub(string id, UpdateClub updateClubDto)
    {
        try
        {
            var club = await _clubService.UpdateClubAsync(id, updateClubDto);
            
            if (club == null)
            {
                return NotFound(new { message = $"Club with ID '{id}' not found." });
            }

            return Ok(club);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // DELETE: api/clubs/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClub(string id)
    {
        var result = await _clubService.DeleteClubAsync(id);
        
        if (!result)
        {
            return NotFound(new { message = $"Club with ID '{id}' not found." });
        }

        return NoContent();
    }

    [HttpPost("upload-logo/{clubId}")]
    public async Task<IActionResult> UploadClubLogo(string clubId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "club-logos");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        // Delete any existing logo for this club
        var existingFiles = Directory.GetFiles(folderPath, $"club-{clubId}.*");
        foreach (var existingFile in existingFiles)
            System.IO.File.Delete(existingFile);

        var extension = Path.GetExtension(file.FileName).ToLower();
        var fileName = $"club-{clubId}{extension}";
        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
            await file.CopyToAsync(stream);

        var logoUrl = $"{Request.Scheme}://{Request.Host}/club-logos/{fileName}";

        return Ok(new { logoUrl });
    }

    [HttpGet("test-email")]
    public async Task<IActionResult> TestEmail(
    [FromServices] IEmailNotificationService emailService)
    {
        await emailService.SendEmailAsync(
            toEmail: "rahul@wdnkf.onmicrosoft.comm",   // your real email
            toName: "Test User",
            subject: "Test Email from Football Dashboard",
            htmlContent: @"
            <div style='font-family:Arial; padding:20px;'>
                <h1 style='color:green;'> It works!</h1>
                <p>SMTP with Outlook is configured correctly.</p>
            </div>"
        );
        return Ok(new { message = "Email sent successfully" });
    }
}