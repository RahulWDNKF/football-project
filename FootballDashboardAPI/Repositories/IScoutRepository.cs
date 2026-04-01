using FootballDashboardAPI.Models;

namespace FootballDashboardAPI.Repositories;

public interface IScoutRepository
{
    Task<IEnumerable<Scout>> GetAllAsync();
    Task<Scout?> GetByIdAsync(string id);
    Task<Scout> CreateAsync(Scout scout);
    Task<string?> GetMaxScoutIdAsync();
    Task<Scout?> UpdateAsync(Scout scout);
    Task<bool> DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<bool> ScoutNameExistsAsync(string scoutName, string? excludeScoutId = null);
}
