using FootballDashboardAPI.Models;

namespace FootballDashboardAPI.Services;

public interface INoteService
{
    Task<IEnumerable<Note>> GetAllNotesAsync();
    Task<Note?> GetNoteByIdAsync(string id);
    Task<IEnumerable<Note>> GetNotesByClubIdAsync(string clubId);
    Task<IEnumerable<Note>> GetNotesByPlayerIdAsync(string playerId);
    Task<Note> CreateNoteAsync(CreateNote createNoteDto);
    Task<Note?> UpdateNoteAsync(string id, UpdateNote updateNoteDto);
    Task<bool> DeleteNoteAsync(string id);
}
