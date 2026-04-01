using FootballDashboardAPI.Models;

namespace FootballDashboardAPI.Repositories;

public interface IClubRepository
{
    Task<IEnumerable<Club>> GetAllAsync();
    Task<Club?> GetByIdAsync(string id);
    Task<Club> CreateAsync(Club club);
    Task<Club?> UpdateAsync(Club club);
    Task<bool> DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<bool> ClubNameExistsAsync(string clubName, string? excludeClubId = null);
}