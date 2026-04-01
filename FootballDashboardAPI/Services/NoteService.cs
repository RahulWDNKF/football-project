using FootballDashboardAPI.Models;
using FootballDashboardAPI.Repositories;

namespace FootballDashboardAPI.Services;

public class NoteService : INoteService
{
    private readonly INoteRepository _noteRepository;

    public NoteService(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    public async Task<IEnumerable<Note>> GetAllNotesAsync()
    {
        var notes = await _noteRepository.GetAllAsync();
        return notes.Select(MapToDto);
    }

    public async Task<Note?> GetNoteByIdAsync(string id)
    {
        var note = await _noteRepository.GetByIdAsync(id);
        return note == null ? null : MapToDto(note);
    }

    public async Task<IEnumerable<Note>> GetNotesByClubIdAsync(string clubId)
    {
        var notes = await _noteRepository.GetByClubIdAsync(clubId);
        return notes.Select(MapToDto);
    }

    public async Task<IEnumerable<Note>> GetNotesByPlayerIdAsync(string playerId)
    {
        var notes = await _noteRepository.GetByPlayerIdAsync(playerId);
        return notes.Select(MapToDto);
    }

    //public async Task<NoteDto> CreateNoteAsync(CreateNoteDto createNoteDto)
    //{
    //    var note = new Note
    //    {
    //        NoteId = Guid.NewGuid().ToString(),
    //        PlayerId = createNoteDto.PlayerId,
    //        ClubId = createNoteDto.ClubId,
    //        Topic = createNoteDto.Topic,
    //        Description = createNoteDto.Description,
    //        Category = createNoteDto.Category,
    //        FollowUpDate = createNoteDto.FollowUpDate,
    //        CreatedByScoutId = createNoteDto.CreatedByScoutId,
    //        CreatedAt = DateTime.UtcNow
    //    };

    //    var createdNote = await _noteRepository.CreateAsync(note);
    //    return MapToDto(createdNote);
    //}
    public async Task<Note> CreateNoteAsync(CreateNote createNoteDto)
    {
        // Get the next sequential ID
        var nextId = await GenerateNextNoteIdAsync();

        var note = new Note
        {
            NoteId = nextId,  // Use sequential ID instead of Guid
            PlayerId = createNoteDto.PlayerId,
            ClubId = createNoteDto.ClubId,
            Topic = createNoteDto.Topic,
            Description = createNoteDto.Description,
            Category = createNoteDto.Category,
            FollowUpDate = createNoteDto.FollowUpDate,
            CreatedByScoutId = createNoteDto.CreatedByScoutId,
            CreatedAt = DateTime.UtcNow
        };

        var createdNote = await _noteRepository.CreateAsync(note);
        return MapToDto(createdNote);
    }

    public async Task<Note?> UpdateNoteAsync(string id, UpdateNote updateNoteDto)
    {
        var existingNote = await _noteRepository.GetByIdAsync(id);
        if (existingNote == null)
            return null;

        var note = new Note
        {
            NoteId = id,
            PlayerId = existingNote.PlayerId,
            ClubId = existingNote.ClubId,
            Topic = updateNoteDto.Topic,
            Description = updateNoteDto.Description,
            Category = updateNoteDto.Category,
            FollowUpDate = updateNoteDto.FollowUpDate,
            CreatedByScoutId = existingNote.CreatedByScoutId,
            CreatedAt = existingNote.CreatedAt
        };

        var updatedNote = await _noteRepository.UpdateAsync(note);
        return updatedNote == null ? null : MapToDto(updatedNote);
    }

    public async Task<bool> DeleteNoteAsync(string id)
    {
        return await _noteRepository.DeleteAsync(id);
    }

    private static Note MapToDto(Note note)
    {
        return new Note
        {
            NoteId = note.NoteId,
            PlayerId = note.PlayerId,
            ClubId = note.ClubId,
            Topic = note.Topic,
            Description = note.Description,
            Category = note.Category,
            FollowUpDate = note.FollowUpDate,
            CreatedByScoutId = note.CreatedByScoutId,
            CreatedAt = note.CreatedAt
        };
    }


    private async Task<string> GenerateNextNoteIdAsync()
    {
        var maxId = await _noteRepository.GetMaxNoteIdAsync();

        Console.WriteLine($"MaxNoteId: {maxId}");

        if (maxId == null)
            return "1";

        return int.TryParse(maxId, out var num) ? $"{num + 1}" : "1";
    }
}
