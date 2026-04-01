using FootballDashboardAPI.Models;
using FootballDashboardAPI.Repositories;

namespace FootballDashboardAPI.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewService(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<IEnumerable<Review>> GetAllReviewsAsync()
    {
        var reviews = await _reviewRepository.GetAllAsync();
        return reviews.Select(MapToDto);
    }

    public async Task<Review?> GetReviewByIdAsync(string id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        return review == null ? null : MapToDto(review);
    }

    public async Task<IEnumerable<Review>> GetReviewsByPlayerIdAsync(string playerId)
    {
        var reviews = await _reviewRepository.GetByPlayerIdAsync(playerId);
        return reviews.Select(MapToDto);
    }

    public async Task<IEnumerable<Review>> GetReviewsByScoutIdAsync(string scoutId)
    {
        var reviews = await _reviewRepository.GetByScoutIdAsync(scoutId);
        return reviews.Select(MapToDto);
    }

    //public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto createReviewDto)
    //{
    //    var review = new Review
    //    {
    //        ReviewId = Guid.NewGuid().ToString(),
    //        PlayerId = createReviewDto.PlayerId,
    //        ScoutId = createReviewDto.ScoutId,
    //        MatchDate = createReviewDto.MatchDate,
    //        Club1Id = createReviewDto.Club1Id,
    //        Club2Id = createReviewDto.Club2Id,
    //        Notes = createReviewDto.Notes,
    //        CreatedAt = DateTime.UtcNow
    //    };

    //    var createdReview = await _reviewRepository.CreateAsync(review);
    //    return MapToDto(createdReview);
    //}
    public async Task<Review> CreateReviewAsync(CreateReview createReviewDto)
    {
        // Get last ID and increment
        //var lastId = await _reviewRepository.GetLastIdAsync();
        //var lastNumber = int.Parse(lastId.Substring(1));   // strips "r" gets the number
        //var newId = $"r{lastNumber + 1}";                  // r7 to r8

        var lastId = await _reviewRepository.GetLastIdAsync();
        var newId = (lastId + 1).ToString();

        var review = new Review
        {
            ReviewId = newId,           // sequential ID instead of Guid.NewGuid()
            PlayerId = createReviewDto.PlayerId,
            ScoutId = createReviewDto.ScoutId,
            MatchDate = createReviewDto.MatchDate,
            Club1Id = createReviewDto.Club1Id,
            Club2Id = createReviewDto.Club2Id,
            Notes = createReviewDto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        var createdReview = await _reviewRepository.CreateAsync(review);
        return MapToDto(createdReview);
    }

    public async Task<Review?> UpdateReviewAsync(string id, UpdateReview updateReviewDto)
    {
        var existingReview = await _reviewRepository.GetByIdAsync(id);
        if (existingReview == null)
            return null;

        var review = new Review
        {
            ReviewId = id,
            PlayerId = existingReview.PlayerId,
            ScoutId = existingReview.ScoutId,
            MatchDate = updateReviewDto.MatchDate,
            Club1Id = updateReviewDto.Club1Id,
            Club2Id = updateReviewDto.Club2Id,
            Notes = updateReviewDto.Notes,
            CreatedAt = existingReview.CreatedAt
        };

        var updatedReview = await _reviewRepository.UpdateAsync(review);
        return updatedReview == null ? null : MapToDto(updatedReview);
    }

    public async Task<bool> DeleteReviewAsync(string id)
    {
        return await _reviewRepository.DeleteAsync(id);
    }

    private static Review MapToDto(Review review)
    {
        return new Review
        {
            ReviewId = review.ReviewId,
            PlayerId = review.PlayerId,
            ScoutId = review.ScoutId,
            MatchDate = review.MatchDate,
            Club1Id = review.Club1Id,
            Club2Id = review.Club2Id,
            Notes = review.Notes,
            CreatedAt = review.CreatedAt
        };
    }
}
