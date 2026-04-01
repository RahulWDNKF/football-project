using FootballDashboardAPI.Models;
using Npgsql;
using NpgsqlTypes;

namespace FootballDashboardAPI.Repositories;

public class EmailRepository : IEmailRepository
{
    private readonly PostgresConnectionProvider _db;

    public EmailRepository(PostgresConnectionProvider db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Email>> GetAllAsync()
    {
        return await _db.ExecuteQueryListAsync(
            "SELECT * FROM stf.fn_emails_get_all()",
            MapReaderToEmail
        );
    }

    public async Task<Email?> GetByIdAsync(string id)
    {
        return await _db.ExecuteQuerySingleAsync(
            "SELECT * FROM stf.fn_emails_get_by_id(@p_id)",
            MapReaderToEmail,
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
    }

    public async Task<bool> ExistsAsync(string id)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT stf.fn_emails_exists(@p_id)",
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    public async Task<Email> CreateAsync(Email email)
    {
        // Generate next numeric ID: 1, 2, 3...
        var lastIdResult = await _db.ExecuteScalarAsync(
            "SELECT MAX(CAST(email_id AS INTEGER)) FROM stf.emails WHERE email_id ~ '^\\d+$'"
        );
        int nextNumber = 1;
        if (lastIdResult != null && lastIdResult != DBNull.Value)
        {
            nextNumber = Convert.ToInt32(lastIdResult) + 1;
        }
        email.EmailId = $"{nextNumber}";
        await _db.ExecuteNonQueryAsync(
            "SELECT stf.fn_emails_insert(@p_email_id, @p_recipient_email, @p_subject, @p_body, @p_sent_by_scout_id, @p_sent_at, @p_player_id, @p_club_id)",
            new NpgsqlParameter("p_email_id", NpgsqlDbType.Varchar)
            { Value = email.EmailId },
            new NpgsqlParameter("p_recipient_email", NpgsqlDbType.Varchar)
            { Value = email.RecipientEmail },
            new NpgsqlParameter("p_subject", NpgsqlDbType.Varchar)
            { Value = email.Subject },
            new NpgsqlParameter("p_body", NpgsqlDbType.Text)
            { Value = email.Body },
            new NpgsqlParameter("p_sent_by_scout_id", NpgsqlDbType.Varchar)
            { Value = email.SentByScoutId },
            new NpgsqlParameter("p_sent_at", NpgsqlDbType.Timestamp)
            { Value = DateTime.SpecifyKind(email.SentAt, DateTimeKind.Unspecified) },
            new NpgsqlParameter("p_player_id", NpgsqlDbType.Varchar)
            { Value = email.PlayerId == null ? DBNull.Value : (object)email.PlayerId },
            new NpgsqlParameter("p_club_id", NpgsqlDbType.Varchar)
            { Value = email.ClubId == null ? DBNull.Value : (object)email.ClubId }
        );

        return await GetByIdAsync(email.EmailId) ?? email;
    }

    public async Task<Email?> UpdateAsync(Email email)
    {
        var existing = await GetByIdAsync(email.EmailId);
        if (existing == null) return null;

        await _db.ExecuteNonQueryAsync(
            "SELECT stf.fn_emails_update(@p_email_id, @p_recipient_email, @p_subject, @p_body, @p_sent_by_scout_id, @p_sent_at, @p_player_id, @p_club_id)",
            new NpgsqlParameter("p_email_id", NpgsqlDbType.Varchar)
            { Value = email.EmailId },
            new NpgsqlParameter("p_recipient_email", NpgsqlDbType.Varchar)
            { Value = email.RecipientEmail },
            new NpgsqlParameter("p_subject", NpgsqlDbType.Varchar)
            { Value = email.Subject },
            new NpgsqlParameter("p_body", NpgsqlDbType.Text)
            { Value = email.Body },
            new NpgsqlParameter("p_sent_by_scout_id", NpgsqlDbType.Varchar)
            { Value = email.SentByScoutId },
            new NpgsqlParameter("p_sent_at", NpgsqlDbType.Timestamp)
            { Value = DateTime.SpecifyKind(email.SentAt, DateTimeKind.Unspecified) },
            new NpgsqlParameter("p_player_id", NpgsqlDbType.Varchar)
            { Value = email.PlayerId == null ? DBNull.Value : (object)email.PlayerId },
            new NpgsqlParameter("p_club_id", NpgsqlDbType.Varchar)
            { Value = email.ClubId == null ? DBNull.Value : (object)email.ClubId }
        );

        return await GetByIdAsync(email.EmailId);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT stf.fn_emails_delete(@p_id)",
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    private Email MapReaderToEmail(NpgsqlDataReader reader)
    {
        return new Email
        {
            EmailId = reader["email_id"].ToString()!,
            PlayerId = reader["player_id"] == DBNull.Value ? null : reader["player_id"].ToString(),
            ClubId = reader["club_id"] == DBNull.Value ? null : reader["club_id"].ToString(),
            RecipientEmail = reader["recipient_email"].ToString()!,
            Subject = reader["subject"].ToString()!,
            Body = reader["body"].ToString()!,
            SentByScoutId = reader["sent_by_scout_id"].ToString()!,
            SentAt = (DateTime)reader["sent_at"]
        };
    }
}
