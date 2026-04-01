using FootballDashboardAPI.Models;

namespace FootballDashboardAPI.Repositories;

public interface IPlayerRepository
{
    Task<IEnumerable<Player1>> GetAllAsync();
    Task<Player1?> GetByIdAsync(long id);
    Task<Player1?> GetByCustomIdAsync(long id);
    Task<Player1> CreateAsync(Player1 player);
    Task<Player1?> UpdateAsync(Player1 player);
    Task<bool> DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
}
