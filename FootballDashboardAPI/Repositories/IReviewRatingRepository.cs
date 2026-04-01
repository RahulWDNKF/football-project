using FootballDashboardAPI.Models;

namespace FootballDashboardAPI.Repositories;

public interface IReviewRatingRepository
{
    Task<IEnumerable<ReviewRating>> GetAllAsync();
    Task<ReviewRating?> GetByIdAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<ReviewRating> CreateAsync(ReviewRating rating);
    Task<ReviewRating?> UpdateAsync(ReviewRating rating);
    Task<bool> DeleteAsync(string id);
}
