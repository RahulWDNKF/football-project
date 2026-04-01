using FootballDashboardAPI.Models;
using FootballDashboardAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballDashboardAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ReviewRatingsController : ControllerBase
{
    private readonly IReviewRatingRepository _repository;

    public ReviewRatingsController(IReviewRatingRepository repository)
    {
        _repository = repository;
    }

    // GET: api/ReviewRatings
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewRating>>> GetAll()
    {
        var ratings = await _repository.GetAllAsync();
        return Ok(ratings);
    }

    // GET: api/ReviewRatings/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewRating>> GetById(string id)
    {
        var rating = await _repository.GetByIdAsync(id);
        if (rating == null)
            return NotFound(new { message = $"ReviewRating with ID '{id}' not found." });

        return Ok(rating);
    }

    // POST: api/ReviewRatings
    [HttpPost]
    public async Task<ActionResult<ReviewRating>> Create([FromBody] ReviewRating rating)
    {
        try
        {
            // Check if rating already exists for this review
            var exists = await _repository.ExistsAsync(rating.ReviewId);
            if (exists)
                return Conflict(new { message = $"Rating for review '{rating.ReviewId}' already exists." });

            var created = await _repository.CreateAsync(rating);
            return CreatedAtAction(nameof(GetById), new { id = created.ReviewId }, created);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating review rating: {ex.Message}");
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // PUT: api/ReviewRatings/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<ReviewRating>> Update(string id, [FromBody] ReviewRating rating)
    {
        rating.ReviewId = id;

        var updated = await _repository.UpdateAsync(rating);
        if (updated == null)
            return NotFound(new { message = $"ReviewRating with ID '{id}' not found." });

        return Ok(updated);
    }

    // DELETE: api/ReviewRatings/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _repository.DeleteAsync(id);
        if (!result)
            return NotFound(new { message = $"ReviewRating with ID '{id}' not found." });

        return NoContent();
    }
}
