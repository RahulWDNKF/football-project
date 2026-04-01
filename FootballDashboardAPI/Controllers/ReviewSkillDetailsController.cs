using FootballDashboardAPI.Models;
using FootballDashboardAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballDashboardAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ReviewSkillDetailsController : ControllerBase
{
    private readonly IReviewSkillDetailRepository _repository;

    public ReviewSkillDetailsController(IReviewSkillDetailRepository repository)
    {
        _repository = repository;
    }

    // GET: api/ReviewSkillDetails
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewSkillDetail>>> GetAll()
    {
        var details = await _repository.GetAllAsync();
        return Ok(details);
    }

    // GET: api/ReviewSkillDetails/{reviewId}/{skillKey}
    [HttpGet("{reviewId}/{skillKey}")]
    public async Task<ActionResult<ReviewSkillDetail>> GetById(string reviewId, string skillKey)
    {
        var detail = await _repository.GetByIdAsync(reviewId, skillKey);
        if (detail == null)
            return NotFound(new { message = $"ReviewSkillDetail for review '{reviewId}' and skill '{skillKey}' not found." });

        return Ok(detail);
    }

    // GET: api/ReviewSkillDetails/review/{reviewId}
    [HttpGet("review/{reviewId}")]
    public async Task<ActionResult<IEnumerable<ReviewSkillDetail>>> GetByReviewId(string reviewId)
    {
        var details = await _repository.GetByReviewIdAsync(reviewId);
        return Ok(details);
    }

    // POST: api/ReviewSkillDetails
    [HttpPost]
    public async Task<ActionResult<ReviewSkillDetail>> Create([FromBody] ReviewSkillDetail detail)
    {
        try
        {
            var exists = await _repository.ExistsAsync(detail.ReviewId, detail.SkillKey);
            if (exists)
                return Conflict(new { message = $"Skill detail for review '{detail.ReviewId}' and skill '{detail.SkillKey}' already exists." });

            var created = await _repository.CreateAsync(detail);
            return CreatedAtAction(
                nameof(GetById),
                new { reviewId = created.ReviewId, skillKey = created.SkillKey },
                created
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating review skill detail: {ex.Message}");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // PUT: api/ReviewSkillDetails/{reviewId}/{skillKey}
    [HttpPut("{reviewId}/{skillKey}")]
    public async Task<ActionResult<ReviewSkillDetail>> Update(
        string reviewId,
        string skillKey,
        [FromBody] ReviewSkillDetail detail)
    {
        detail.ReviewId = reviewId;
        detail.SkillKey = skillKey;

        var updated = await _repository.UpdateAsync(detail);
        if (updated == null)
            return NotFound(new { message = $"ReviewSkillDetail for review '{reviewId}' and skill '{skillKey}' not found." });

        return Ok(updated);
    }

    // DELETE: api/ReviewSkillDetails/{reviewId}/{skillKey}
    [HttpDelete("{reviewId}/{skillKey}")]
    public async Task<IActionResult> Delete(string reviewId, string skillKey)
    {
        var result = await _repository.DeleteAsync(reviewId, skillKey);
        if (!result)
            return NotFound(new { message = $"ReviewSkillDetail for review '{reviewId}' and skill '{skillKey}' not found." });

        return NoContent();
    }
}
