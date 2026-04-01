using FootballDashboardAPI.Models;
using Npgsql;

namespace FootballDashboardAPI.Repositories;

public class ClubRepository : IClubRepository
{
    private readonly PostgresConnectionProvider _db;

    public ClubRepository(PostgresConnectionProvider db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Club>> GetAllAsync()
    {
        return await _db.ExecuteQueryListAsync(
            "SELECT * FROM stf.fn_clubs_get_all()",
            MapReaderToClub
        );
    }

    public async Task<Club?> GetByIdAsync(string id)
    {
        return await _db.ExecuteQuerySingleAsync(
            "SELECT * FROM stf.fn_clubs_get_by_id(@p_id)",
            MapReaderToClub,
            new NpgsqlParameter("p_id", id)
        );
    }

    public async Task<Club> CreateAsync(Club club)
    {
        var lastIdResult = await _db.ExecuteScalarAsync(
     "SELECT MAX(CAST(club_id AS INTEGER)) FROM stf.clubs WHERE club_id ~ '^\\d+$'"
 );
        int nextNumber = 1;
        if (lastIdResult != null && lastIdResult != DBNull.Value)
        {
            nextNumber = Convert.ToInt32(lastIdResult) + 1;
        }
        club.ClubId = $"{nextNumber}";
        club.CreatedAt = DateTime.UtcNow;

        await _db.ExecuteNonQueryAsync(
            "SELECT stf.fn_clubs_insert(@p_club_id, @p_club_name, @p_country, @p_created_at, @p_address_line, @p_logo_url)",
            new NpgsqlParameter("p_club_id", NpgsqlTypes.NpgsqlDbType.Varchar)
            { Value = club.ClubId },
            new NpgsqlParameter("p_club_name", NpgsqlTypes.NpgsqlDbType.Varchar)
            { Value = club.ClubName },
            new NpgsqlParameter("p_country", NpgsqlTypes.NpgsqlDbType.Varchar)
            { Value = club.Country },
            new NpgsqlParameter("p_created_at", NpgsqlTypes.NpgsqlDbType.Timestamp)
            { Value = DateTime.SpecifyKind(club.CreatedAt, DateTimeKind.Unspecified) },
            new NpgsqlParameter("p_address_line", NpgsqlTypes.NpgsqlDbType.Varchar)
            { Value = club.AddressLine == null ? DBNull.Value : (object)club.AddressLine },
            new NpgsqlParameter("p_logo_url", NpgsqlTypes.NpgsqlDbType.Varchar)
            { Value = club.LogoUrl == null ? DBNull.Value : (object)club.LogoUrl }
        );

        var createdClub = await GetByIdAsync(club.ClubId);
        return createdClub ?? club;
    }

    public async Task<Club?> UpdateAsync(Club club)
    {
        await _db.ExecuteNonQueryAsync(
            "SELECT * FROM stf.fn_clubs_update(@p_club_id, @p_club_name, @p_country, @p_address_line, @p_logo_url)",
            new NpgsqlParameter("p_club_id", club.ClubId),
            new NpgsqlParameter("p_club_name", club.ClubName),
            new NpgsqlParameter("p_country", club.Country),
            new NpgsqlParameter("p_address_line", club.AddressLine == null ? DBNull.Value : (object)club.AddressLine),
            new NpgsqlParameter("p_logo_url", club.LogoUrl == null ? DBNull.Value : (object)club.LogoUrl)
        );
        return await GetByIdAsync(club.ClubId);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT * FROM stf.fn_clubs_delete(@p_id)",
            new NpgsqlParameter("p_id", id)
        );
        var affected = result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        return affected > 0;
    }

    public async Task<bool> ExistsAsync(string id)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT * FROM stf.fn_clubs_exists(@p_id)",
            new NpgsqlParameter("p_id", id)
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    public async Task<bool> ClubNameExistsAsync(string clubName, string? excludeClubId = null)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT * FROM stf.fn_clubs_name_exists(@p_club_name, @p_exclude_club_id)",
            new NpgsqlParameter("p_club_name", clubName),
            new NpgsqlParameter("p_exclude_club_id", excludeClubId == null ? DBNull.Value : (object)excludeClubId)
        );

        // ADD THESE DEBUG LINES
        Console.WriteLine($"ClubNameExistsAsync raw result: {result}");
        Console.WriteLine($"ClubNameExistsAsync result type: {result?.GetType()}");
        Console.WriteLine($"ClubNameExistsAsync converted: {Convert.ToInt32(result ?? 0)}");
        return Convert.ToInt32(result ?? 0) > 0;
    }

    private Club MapReaderToClub(NpgsqlDataReader reader)
    {
        return new Club
        {
            ClubId = reader["club_id"].ToString(),
            ClubName = reader["club_name"].ToString(),
            Country = reader["country"].ToString(),
            AddressLine = reader["address_line"] == DBNull.Value ? null : reader["address_line"].ToString(),
            LogoUrl = reader["logo_url"] == DBNull.Value ? null : reader["logo_url"].ToString(),
            CreatedAt = (DateTime)reader["created_at"]
        };
    }
}