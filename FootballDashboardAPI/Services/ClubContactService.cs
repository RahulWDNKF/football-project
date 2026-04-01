using FootballDashboardAPI.Models;
using FootballDashboardAPI.Repositories;
using Npgsql;
using NpgsqlTypes;

namespace FootballDashboardAPI.Services;

public class ClubContactService : IClubContactService
{
    private readonly PostgresConnectionProvider _db;

    public ClubContactService(PostgresConnectionProvider db)
    {
        _db = db;
    }

    public async Task<IEnumerable<ClubContact>> GetAllAsync()
    {
        return await _db.ExecuteQueryListAsync(
            "SELECT * FROM stf.fn_club_contacts_get_all()",
            MapReaderToClubContact
        );
    }

    public async Task<ClubContact?> GetByIdAsync(string id)
    {
        return await _db.ExecuteQuerySingleAsync(
            "SELECT * FROM stf.fn_club_contacts_get_by_id(@p_id)",
            MapReaderToClubContact,
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
    }

    public async Task<ClubContact> CreateAsync(CreateClubContact c)
    {
        // Validate Club exists
        var clubExists = await _db.ExecuteScalarAsync(
            "SELECT stf.fn_clubs_exists(@p_id)",
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = c.ClubId }
        );
        if (Convert.ToInt32(clubExists ?? 0) == 0)
            throw new InvalidOperationException("Invalid ClubId");

        // var clubContactId = Guid.NewGuid().ToString();
        var lastIdResult = await _db.ExecuteScalarAsync(
     "SELECT MAX(CAST(club_contact_id AS INTEGER)) FROM stf.club_contacts WHERE club_contact_id ~ '^\\d+$'"
         );
                int nextNumber = 1;
                if (lastIdResult != null && lastIdResult != DBNull.Value)
                    nextNumber = Convert.ToInt32(lastIdResult) + 1;
                var clubContactId = $"{nextNumber}";
        var createdAt = DateTime.UtcNow;

        await _db.ExecuteNonQueryAsync(
            "SELECT stf.fn_club_contacts_insert(@p_club_contact_id, @p_club_id, @p_contact_name, @p_role_name, @p_created_at, @p_email, @p_phone)",
            new NpgsqlParameter("p_club_contact_id", NpgsqlDbType.Varchar)
            { Value = clubContactId },
            new NpgsqlParameter("p_club_id", NpgsqlDbType.Varchar)
            { Value = c.ClubId },
            new NpgsqlParameter("p_contact_name", NpgsqlDbType.Varchar)
            { Value = c.ContactName },
            new NpgsqlParameter("p_role_name", NpgsqlDbType.Varchar)
            { Value = c.RoleName },
            new NpgsqlParameter("p_created_at", NpgsqlDbType.Timestamp)
            { Value = DateTime.SpecifyKind(createdAt, DateTimeKind.Unspecified) },
            new NpgsqlParameter("p_email", NpgsqlDbType.Varchar)
            { Value = c.Email == null ? DBNull.Value : (object)c.Email },
            new NpgsqlParameter("p_phone", NpgsqlDbType.Varchar)
            { Value = c.Phone == null ? DBNull.Value : (object)c.Phone }
        );

        return await GetByIdAsync(clubContactId) ?? new ClubContact
        {
            ClubContactId = clubContactId,
            ClubId = c.ClubId,
            ContactName = c.ContactName,
            RoleName = c.RoleName,
            Email = c.Email,
            Phone = c.Phone,
            CreatedAt = createdAt
        };
    }

    public async Task<ClubContact?> UpdateAsync(string id, UpdateClubContact c)
    {
        var existing = await GetByIdAsync(id);
        if (existing == null)
            return null;

        string clubId = c.ClubId ?? existing.ClubId;

        if (!string.IsNullOrEmpty(c.ClubId))
        {
            var clubExists = await _db.ExecuteScalarAsync(
                "SELECT stf.fn_clubs_exists(@p_id)",
                new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = c.ClubId }
            );
            if (Convert.ToInt32(clubExists ?? 0) == 0)
                throw new InvalidOperationException("Invalid ClubId");
        }

        await _db.ExecuteNonQueryAsync(
            "SELECT stf.fn_club_contacts_update(@p_club_contact_id, @p_club_id, @p_contact_name, @p_role_name, @p_email, @p_phone)",
            new NpgsqlParameter("p_club_contact_id", NpgsqlDbType.Varchar)
            { Value = id },
            new NpgsqlParameter("p_club_id", NpgsqlDbType.Varchar)
            { Value = clubId },
            new NpgsqlParameter("p_contact_name", NpgsqlDbType.Varchar)
            { Value = c.ContactName ?? existing.ContactName },
            new NpgsqlParameter("p_role_name", NpgsqlDbType.Varchar)
            { Value = c.RoleName ?? existing.RoleName },
            new NpgsqlParameter("p_email", NpgsqlDbType.Varchar)
            { Value = (c.Email ?? existing.Email) == null ? DBNull.Value : (object)(c.Email ?? existing.Email)! },
            new NpgsqlParameter("p_phone", NpgsqlDbType.Varchar)
            { Value = (c.Phone ?? existing.Phone) == null ? DBNull.Value : (object)(c.Phone ?? existing.Phone)! }
        );

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT stf.fn_club_contacts_delete(@p_id)",
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    private ClubContact MapReaderToClubContact(NpgsqlDataReader reader)
    {
        return new ClubContact
        {
            ClubContactId = reader["club_contact_id"].ToString()!,
            ClubId = reader["club_id"].ToString()!,
            ContactName = reader["contact_name"].ToString()!,
            RoleName = reader["role_name"].ToString()!,
            Email = reader["email"] == DBNull.Value ? null : reader["email"].ToString(),
            Phone = reader["phone"] == DBNull.Value ? null : reader["phone"].ToString(),
            CreatedAt = (DateTime)reader["created_at"]
        };
    }
}