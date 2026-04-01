using FootballDashboardAPI.Models;

namespace FootballDashboardAPI.Repositories;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetAllAsync();
    Task<Review?> GetByIdAsync(string id);
    Task<Review> CreateAsync(Review review);
    Task<Review?> UpdateAsync(Review review);
    Task<bool> DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<IEnumerable<Review>> GetByPlayerIdAsync(string playerId);
    Task<IEnumerable<Review>> GetByScoutIdAsync(string scoutId);
    //Task<string> GetLastIdAsync();

    Task<long> GetLastIdAsync();
}
