using FootballDashboardAPI.Models;
using Npgsql;
using NpgsqlTypes;

namespace FootballDashboardAPI.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly PostgresConnectionProvider _db;

    public NoteRepository(PostgresConnectionProvider db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Note>> GetAllAsync()
    {
        return await _db.ExecuteQueryListAsync(
            "SELECT * FROM stf.fn_notes_get_all()",
            MapReaderToNote
        );
    }

    public async Task<Note?> GetByIdAsync(string id)
    {
        return await _db.ExecuteQuerySingleAsync(
            "SELECT * FROM stf.fn_notes_get_by_id(@p_id)",
            MapReaderToNote,
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
    }

    public async Task<IEnumerable<Note>> GetByPlayerIdAsync(string playerId)
    {
        return await _db.ExecuteQueryListAsync(
            "SELECT * FROM stf.fn_notes_get_by_player_id(@p_player_id)",
            MapReaderToNote,
            new NpgsqlParameter("p_player_id", NpgsqlDbType.Varchar) { Value = playerId }
        );
    }

    public async Task<IEnumerable<Note>> GetByClubIdAsync(string clubId)
    {
        return await _db.ExecuteQueryListAsync(
            "SELECT * FROM stf.fn_notes_get_by_club_id(@p_club_id)",
            MapReaderToNote,
            new NpgsqlParameter("p_club_id", NpgsqlDbType.Varchar) { Value = clubId }
        );
    }

    public async Task<bool> ExistsAsync(string id)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT stf.fn_notes_exists(@p_id)",
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    public async Task<string?> GetMaxNoteIdAsync()
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT MAX(CAST(note_id AS INTEGER)) FROM stf.notes WHERE note_id ~ '^\\d+$'"
        );
        return result == null || result == DBNull.Value ? null : result.ToString();
    }

    public async Task<Note> CreateAsync(Note note)
    {
        await _db.ExecuteNonQueryAsync(
            "SELECT stf.fn_notes_insert(@p_note_id, @p_topic, @p_description, @p_category, @p_created_by_scout_id, @p_created_at, @p_player_id, @p_club_id, @p_follow_up_date)",
            new NpgsqlParameter("p_note_id", NpgsqlDbType.Varchar)
            { Value = note.NoteId },
            new NpgsqlParameter("p_topic", NpgsqlDbType.Varchar)
            { Value = note.Topic },
            new NpgsqlParameter("p_description", NpgsqlDbType.Text)
            { Value = note.Description },
            new NpgsqlParameter("p_category", NpgsqlDbType.Varchar)
            { Value = note.Category },
            new NpgsqlParameter("p_created_by_scout_id", NpgsqlDbType.Varchar)
            { Value = note.CreatedByScoutId },
            new NpgsqlParameter("p_created_at", NpgsqlDbType.Timestamp)
            { Value = DateTime.SpecifyKind(note.CreatedAt, DateTimeKind.Unspecified) },
            new NpgsqlParameter("p_player_id", NpgsqlDbType.Varchar)
            { Value = note.PlayerId == null ? DBNull.Value : (object)note.PlayerId },
            new NpgsqlParameter("p_club_id", NpgsqlDbType.Varchar)
            { Value = note.ClubId == null ? DBNull.Value : (object)note.ClubId },
            new NpgsqlParameter("p_follow_up_date", NpgsqlDbType.Date)
            { Value = note.FollowUpDate == null ? DBNull.Value : (object)note.FollowUpDate }
        );

        return await GetByIdAsync(note.NoteId) ?? note;
    }

    public async Task<Note?> UpdateAsync(Note note)
    {
        await _db.ExecuteNonQueryAsync(
            "SELECT stf.fn_notes_update(@p_note_id, @p_topic, @p_description, @p_category, @p_follow_up_date)",
            new NpgsqlParameter("p_note_id", NpgsqlDbType.Varchar)
            { Value = note.NoteId },
            new NpgsqlParameter("p_topic", NpgsqlDbType.Varchar)
            { Value = note.Topic },
            new NpgsqlParameter("p_description", NpgsqlDbType.Text)
            { Value = note.Description },
            new NpgsqlParameter("p_category", NpgsqlDbType.Varchar)
            { Value = note.Category },
            new NpgsqlParameter("p_follow_up_date", NpgsqlDbType.Date)
            { Value = note.FollowUpDate == null ? DBNull.Value : (object)note.FollowUpDate }
        );

        return await GetByIdAsync(note.NoteId);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT stf.fn_notes_delete(@p_id)",
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    private Note MapReaderToNote(NpgsqlDataReader reader)
    {
        return new Note
        {
            NoteId = reader["note_id"].ToString()!,
            PlayerId = reader["player_id"] == DBNull.Value ? null : reader["player_id"].ToString(),
            ClubId = reader["club_id"] == DBNull.Value ? null : reader["club_id"].ToString(),
            Topic = reader["topic"].ToString()!,
            Description = reader["description"].ToString()!,
            Category = reader["category"].ToString()!,
            FollowUpDate = reader["follow_up_date"] == DBNull.Value ? null : (DateOnly?)DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("follow_up_date"))),
            CreatedByScoutId = reader["created_by_scout_id"].ToString()!,
            CreatedAt = (DateTime)reader["created_at"]
        };
    }
}
