using FootballDashboardAPI.Models;

namespace FootballDashboardAPI.Repositories;

public interface IReviewSkillDetailRepository
{
    Task<IEnumerable<ReviewSkillDetail>> GetAllAsync();
    Task<ReviewSkillDetail?> GetByIdAsync(string reviewId, string skillKey);
    Task<IEnumerable<ReviewSkillDetail>> GetByReviewIdAsync(string reviewId);
    Task<bool> ExistsAsync(string reviewId, string skillKey);
    Task<ReviewSkillDetail> CreateAsync(ReviewSkillDetail detail);
    Task<ReviewSkillDetail?> UpdateAsync(ReviewSkillDetail detail);
    Task<bool> DeleteAsync(string reviewId, string skillKey);
}
