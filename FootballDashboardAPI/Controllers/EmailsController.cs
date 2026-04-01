using FootballDashboardAPI.Models;
using FootballDashboardAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballDashboardAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class EmailsController : ControllerBase
{
    private readonly IEmailRepository _repository;

    public EmailsController(IEmailRepository repository)
    {
        _repository = repository;
    }

    // GET: api/Emails
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Email>>> GetAll()
    {
        var emails = await _repository.GetAllAsync();
        return Ok(emails);
    }

    // GET: api/Emails/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Email>> GetById(string id)
    {
        var email = await _repository.GetByIdAsync(id);
        if (email == null)
            return NotFound(new { message = $"Email with ID '{id}' not found." });

        return Ok(email);
    }

    // POST: api/Emails
    [HttpPost]
    public async Task<ActionResult<Email>> Create([FromBody] Email dto)
    {
        try
        {
            dto.EmailId = Guid.NewGuid().ToString();
            dto.SentAt = DateTime.UtcNow;

            var created = await _repository.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.EmailId }, created);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating email: {ex.Message}");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // PUT: api/Emails/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<Email>> Update(string id, [FromBody] Email dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = $"Email with ID '{id}' not found." });

        existing.PlayerId = dto.PlayerId ?? existing.PlayerId;
        existing.ClubId = dto.ClubId ?? existing.ClubId;
        existing.RecipientEmail = dto.RecipientEmail ?? existing.RecipientEmail;
        existing.Subject = dto.Subject ?? existing.Subject;
        existing.Body = dto.Body ?? existing.Body;
        existing.SentByScoutId = dto.SentByScoutId ?? existing.SentByScoutId;

        var updated = await _repository.UpdateAsync(existing);
        if (updated == null)
            return NotFound(new { message = $"Email with ID '{id}' not found." });

        return Ok(updated);
    }

    // DELETE: api/Emails/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _repository.DeleteAsync(id);
        if (!result)
            return NotFound(new { message = $"Email with ID '{id}' not found." });

        return NoContent();
    }
}
