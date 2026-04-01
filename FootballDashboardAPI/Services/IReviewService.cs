using FootballDashboardAPI.Models;

namespace FootballDashboardAPI.Services;

public interface IReviewService
{
    Task<IEnumerable<Review>> GetAllReviewsAsync();
    Task<Review?> GetReviewByIdAsync(string id);
    Task<IEnumerable<Review>> GetReviewsByPlayerIdAsync(string playerId);
    Task<IEnumerable<Review>> GetReviewsByScoutIdAsync(string scoutId);
    Task<Review> CreateReviewAsync(CreateReview createReviewDto);
    Task<Review?> UpdateReviewAsync(string id, UpdateReview updateReviewDto);
    Task<bool> DeleteReviewAsync(string id);
}
