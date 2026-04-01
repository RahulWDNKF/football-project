using FootballDashboardAPI.Models;

namespace FootballDashboardAPI.Repositories;

public interface INoteRepository
{
    Task<IEnumerable<Note>> GetAllAsync();
    Task<Note?> GetByIdAsync(string id);
    Task<Note> CreateAsync(Note note);
    Task<Note?> UpdateAsync(Note note);
    Task<bool> DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<IEnumerable<Note>> GetByClubIdAsync(string clubId);
    Task<IEnumerable<Note>> GetByPlayerIdAsync(string playerId);
    Task<string?> GetMaxNoteIdAsync();
}
