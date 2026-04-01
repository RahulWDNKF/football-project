using FootballDashboardAPI.Models;

namespace FootballDashboardAPI.Services;

public interface IPlayerService
{
    Task<IEnumerable<Player>> GetAllPlayersAsync();
    Task<Player?> GetPlayerByIdAsync(long id);
    Task<Player> CreatePlayerAsync(CreatePlayer createPlayerDto);
    Task<Player?> UpdatePlayerAsync(long id, UpdatePlayer updatePlayerDto);
    Task<bool> DeletePlayerAsync(long id);
}
