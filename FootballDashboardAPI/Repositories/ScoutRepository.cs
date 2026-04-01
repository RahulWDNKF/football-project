using FootballDashboardAPI.Models;
using Npgsql;
using NpgsqlTypes;

namespace FootballDashboardAPI.Repositories;

public class ScoutRepository : IScoutRepository
{
    private readonly PostgresConnectionProvider _db;

    public ScoutRepository(PostgresConnectionProvider db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Scout>> GetAllAsync()
    {
        return await _db.ExecuteQueryListAsync(
            "SELECT * FROM stf.scouts WHERE \"IsDeleted\" IS NOT TRUE ORDER BY scout_id",
            MapReaderToScout
        );
    }
    public async Task<Scout?> GetByIdAsync(string id)
    {
        return await _db.ExecuteQuerySingleAsync(
            "SELECT * FROM stf.scouts WHERE scout_id = @p_id",
            MapReaderToScout,
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
    }
    public async Task<string?> GetMaxScoutIdAsync()
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT MAX(CAST(scout_id AS INTEGER)) FROM stf.scouts WHERE scout_id ~ '^\\d+$'"
        );
        return result == null || result == DBNull.Value ? null : result.ToString();
    }

    public async Task<Scout> CreateAsync(Scout scout)
    {
        await _db.ExecuteNonQueryAsync(
            @"INSERT INTO stf.scouts (
                  scout_id, scout_name, role_name, first_name, last_name, email, phone_number,
                  address_line1, address_line2, city, state, postal_code, country, created_at
              ) VALUES (
                  @p_scout_id, @p_scout_name, @p_role_name, @p_first_name, @p_last_name, @p_email, @p_phone_number,
                  @p_address_line1, @p_address_line2, @p_city, @p_state, @p_postal_code, @p_country, @p_created_at
              )",
            new NpgsqlParameter("p_scout_id", NpgsqlDbType.Varchar)
            { Value = scout.ScoutId },
            new NpgsqlParameter("p_scout_name", NpgsqlDbType.Varchar)
            { Value = scout.ScoutName },
            new NpgsqlParameter("p_role_name", NpgsqlDbType.Varchar)
            { Value = scout.RoleName },
            new NpgsqlParameter("p_first_name", NpgsqlDbType.Varchar)
            { Value = (object?)scout.FirstName ?? DBNull.Value },
            new NpgsqlParameter("p_last_name", NpgsqlDbType.Varchar)
            { Value = (object?)scout.LastName ?? DBNull.Value },
            new NpgsqlParameter("p_email", NpgsqlDbType.Varchar)
            { Value = (object?)scout.Email ?? DBNull.Value },
            new NpgsqlParameter("p_phone_number", NpgsqlDbType.Varchar)
            { Value = (object?)scout.PhoneNumber ?? DBNull.Value },
            new NpgsqlParameter("p_address_line1", NpgsqlDbType.Varchar)
            { Value = (object?)scout.AddressLine1 ?? DBNull.Value },
            new NpgsqlParameter("p_address_line2", NpgsqlDbType.Varchar)
            { Value = (object?)scout.AddressLine2 ?? DBNull.Value },
            new NpgsqlParameter("p_city", NpgsqlDbType.Varchar)
            { Value = (object?)scout.City ?? DBNull.Value },
            new NpgsqlParameter("p_state", NpgsqlDbType.Varchar)
            { Value = (object?)scout.State ?? DBNull.Value },
            new NpgsqlParameter("p_postal_code", NpgsqlDbType.Varchar)
            { Value = (object?)scout.PostalCode ?? DBNull.Value },
            new NpgsqlParameter("p_country", NpgsqlDbType.Varchar)
            { Value = (object?)scout.Country ?? DBNull.Value },
            new NpgsqlParameter("p_created_at", NpgsqlDbType.Timestamp)
            { Value = DateTime.SpecifyKind(scout.CreatedAt, DateTimeKind.Unspecified) }
        );

        return await GetByIdAsync(scout.ScoutId) ?? scout;
    }

    public async Task<Scout?> UpdateAsync(Scout scout)
    {
        await _db.ExecuteNonQueryAsync(
            @"UPDATE stf.scouts
              SET scout_name = @p_scout_name,
                  role_name = @p_role_name,
                  first_name = @p_first_name,
                  last_name = @p_last_name,
                  email = @p_email,
                  phone_number = @p_phone_number,
                  address_line1 = @p_address_line1,
                  address_line2 = @p_address_line2,
                  city = @p_city,
                  state = @p_state,
                  postal_code = @p_postal_code,
                  country = @p_country
              WHERE scout_id = @p_scout_id",
            new NpgsqlParameter("p_scout_id", NpgsqlDbType.Varchar)
            { Value = scout.ScoutId },
            new NpgsqlParameter("p_scout_name", NpgsqlDbType.Varchar)
            { Value = scout.ScoutName },
            new NpgsqlParameter("p_role_name", NpgsqlDbType.Varchar)
            { Value = scout.RoleName },
            new NpgsqlParameter("p_first_name", NpgsqlDbType.Varchar)
            { Value = (object?)scout.FirstName ?? DBNull.Value },
            new NpgsqlParameter("p_last_name", NpgsqlDbType.Varchar)
            { Value = (object?)scout.LastName ?? DBNull.Value },
            new NpgsqlParameter("p_email", NpgsqlDbType.Varchar)
            { Value = (object?)scout.Email ?? DBNull.Value },
            new NpgsqlParameter("p_phone_number", NpgsqlDbType.Varchar)
            { Value = (object?)scout.PhoneNumber ?? DBNull.Value },
            new NpgsqlParameter("p_address_line1", NpgsqlDbType.Varchar)
            { Value = (object?)scout.AddressLine1 ?? DBNull.Value },
            new NpgsqlParameter("p_address_line2", NpgsqlDbType.Varchar)
            { Value = (object?)scout.AddressLine2 ?? DBNull.Value },
            new NpgsqlParameter("p_city", NpgsqlDbType.Varchar)
            { Value = (object?)scout.City ?? DBNull.Value },
            new NpgsqlParameter("p_state", NpgsqlDbType.Varchar)
            { Value = (object?)scout.State ?? DBNull.Value },
            new NpgsqlParameter("p_postal_code", NpgsqlDbType.Varchar)
            { Value = (object?)scout.PostalCode ?? DBNull.Value },
            new NpgsqlParameter("p_country", NpgsqlDbType.Varchar)
            { Value = (object?)scout.Country ?? DBNull.Value }
        );

        return await GetByIdAsync(scout.ScoutId);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        /* var result = await _db.ExecuteScalarAsync(
             "SELECT stf.fn_scouts_delete(@p_id)",
             new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
         );

         return Convert.ToInt32(result ?? 0) > 0;*/


        var rowsAffected = await _db.ExecuteNonQueryAsync(
        "UPDATE stf.scouts SET \"IsDeleted\" = true WHERE scout_id = @id AND \"IsDeleted\" IS NOT TRUE",
        new NpgsqlParameter("id", id)
    );

        return rowsAffected > 0;
    }

    public async Task<bool> ExistsAsync(string id)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT stf.fn_scouts_exists(@p_id)",
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    public async Task<bool> ScoutNameExistsAsync(string scoutName, string? excludeScoutId = null)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT stf.fn_scouts_name_exists(@p_scout_name, @p_exclude_scout_id)",
            new NpgsqlParameter("p_scout_name", NpgsqlDbType.Varchar)
            { Value = scoutName },
            new NpgsqlParameter("p_exclude_scout_id", NpgsqlDbType.Varchar)
            { Value = excludeScoutId == null ? DBNull.Value : (object)excludeScoutId }
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    private Scout MapReaderToScout(NpgsqlDataReader reader)
    {
        return new Scout
        {
            ScoutId = reader["scout_id"].ToString()!,
            ScoutName = reader["scout_name"].ToString()!,
            RoleName = reader["role_name"].ToString()!,
            FirstName = reader["first_name"] as string,
            LastName = reader["last_name"] as string,
            Email = reader["email"] as string,
            PhoneNumber = reader["phone_number"] as string,
            AddressLine1 = reader["address_line1"] as string,
            AddressLine2 = reader["address_line2"] as string,
            City = reader["city"] as string,
            State = reader["state"] as string,
            PostalCode = reader["postal_code"] as string,
            Country = reader["country"] as string,
            CreatedAt = (DateTime)reader["created_at"]
        };
    }
}
