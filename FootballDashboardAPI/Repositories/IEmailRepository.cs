using FootballDashboardAPI.Models;

namespace FootballDashboardAPI.Repositories;

public interface IEmailRepository
{
    Task<IEnumerable<Email>> GetAllAsync();
    Task<Email?> GetByIdAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<Email> CreateAsync(Email email);
    Task<Email?> UpdateAsync(Email email);
    Task<bool> DeleteAsync(string id);
}
