using FootballDashboardAPI.Models;
using FootballDashboardAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballDashboardAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
    {
        var reviews = await _reviewService.GetAllReviewsAsync();
        return Ok(reviews);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Review>> GetReview(string id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);
        
        if (review == null)
        {
            return NotFound(new { message = $"Review with ID '{id}' not found." });
        }

        return Ok(review);
    }

    [HttpGet("player/{playerId}")]
    public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByPlayer(string playerId)
    {
        var reviews = await _reviewService.GetReviewsByPlayerIdAsync(playerId);
        return Ok(reviews);
    }

    [HttpGet("scout/{scoutId}")]
    public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByScout(string scoutId)
    {
        var reviews = await _reviewService.GetReviewsByScoutIdAsync(scoutId);
        return Ok(reviews);
    }

    [HttpPost]
    public async Task<ActionResult<Review>> CreateReview(CreateReview createReviewDto)
    {
        var review = await _reviewService.CreateReviewAsync(createReviewDto);
        return CreatedAtAction(nameof(GetReview), new { id = review.ReviewId }, review);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Review>> UpdateReview(string id, UpdateReview updateReviewDto)
    {
        var review = await _reviewService.UpdateReviewAsync(id, updateReviewDto);
        
        if (review == null)
        {
            return NotFound(new { message = $"Review with ID '{id}' not found." });
        }

        return Ok(review);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(string id)
    {
        var result = await _reviewService.DeleteReviewAsync(id);
        
        if (!result)
        {
            return NotFound(new { message = $"Review with ID '{id}' not found." });
        }

        return NoContent();
    }
}
