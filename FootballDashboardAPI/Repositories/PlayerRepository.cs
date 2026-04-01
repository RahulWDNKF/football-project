using FootballDashboardAPI.Models;
using Npgsql;

namespace FootballDashboardAPI.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly PostgresConnectionProvider _db;

    public PlayerRepository(PostgresConnectionProvider db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Player1>> GetAllAsync()
    {
        return await _db.ExecuteQueryListAsync(
            "SELECT * FROM stf.sp_players_get_all()",
            MapReaderToPlayer
        );
    }

    public async Task<Player1?> GetByIdAsync(long id)
    {
        return await _db.ExecuteQuerySingleAsync(
            "SELECT * FROM stf.sp_players_get_by_id(@p_id)",
            MapReaderToPlayer,
            new NpgsqlParameter("p_id", id)
        );
    }

    public async Task<Player1> CreateAsync(Player1 player)
    {
        // Get last player_id and increment
        var lastIdResult = await _db.ExecuteScalarAsync(
            "SELECT MAX(CAST(player_id AS BIGINT)) FROM stf.players"
        );
        var lastId = lastIdResult == null || lastIdResult == DBNull.Value ? 0 : Convert.ToInt64(lastIdResult);
        var newPlayerId = (lastId + 1).ToString();

        player.PlayerId = newPlayerId;
        player.CreatedAt = DateTime.UtcNow;
        player.UpdatedAt = DateTime.UtcNow;

        await _db.ExecuteNonQueryAsync(
            "SELECT * FROM stf.sp_players_insert(@player_id, @full_name, @date_of_birth, @nationality, @position_code, @preferred_foot, @height_cm, @weight_kg, @current_club_id, @contract_start_date, @contract_end_date, @agent_name, @agent_scout_id, @contact_info, @profile_image_url, @created_at, @updated_at, @player_email)",
            BuildPlayerParameters(player,"new").ToArray()
        );

        return await GetByIdAsync(lastId + 1) ?? player;
    }

    public async Task<Player1?> GetByCustomIdAsync(long id)
    {
        return await _db.ExecuteQuerySingleAsync(
            "SELECT * FROM stf.sp_players_get_by_id(@p_id)",
            MapReaderToPlayer,
            new NpgsqlParameter("p_id", id)
        );
    }

    /* public async Task<Player1?> UpdateAsync(Player1 player)
     {
         player.UpdatedAt = DateTime.UtcNow;

         var result = await _db.ExecuteScalarAsync(
             "SELECT * FROM stf.sp_players_update(@player_id, @full_name, @date_of_birth, @nationality, @position_code, @preferred_foot, @height_cm, @weight_kg, @current_club_id, @contract_start_date, @contract_end_date, @agent_name, @agent_scout_id, @contact_info, @profile_image_url, @updated_at)",
             BuildPlayerParameters(player).ToArray()
         );

         var affected = result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);

         if (affected == 0)
             return null;

         if (long.TryParse(player.PlayerId, out var longId))
         {
             return await GetByIdAsync(longId);
         }

         return player;
     }*/

    public async Task<Player1?> UpdateAsync(Player1 player)
    {
        player.UpdatedAt = DateTime.UtcNow;

        // Check if exists
        if (!long.TryParse(player.PlayerId, out var longId))
            return null;

        var existing = await GetByIdAsync(longId);
        if (existing == null)
            return null;

        // Perform update
        await _db.ExecuteNonQueryAsync(
            "SELECT stf.sp_players_update(@player_id, @full_name, @date_of_birth, @nationality, @position_code, @preferred_foot, @height_cm, @weight_kg, @current_club_id, @contract_start_date, @contract_end_date, @agent_name, @agent_scout_id, @contact_info, @profile_image_url, @updated_at)",
            BuildPlayerParameters(player,"edit").ToArray()
        );

        // Return updated record
        return await GetByIdAsync(longId);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT * FROM stf.sp_players_delete(@p_id)",
            new NpgsqlParameter("p_id", id)
        );

        var affected = result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        return affected > 0;
    }

    public async Task<bool> ExistsAsync(long id)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT * FROM stf.sp_players_exists(@p_id)",
            new NpgsqlParameter("p_id", id)
        );

        return Convert.ToInt32(result ?? 0) > 0;
    }

    private List<NpgsqlParameter> BuildPlayerParameters(Player1 player,string type)
    {

        var parameters = new List<NpgsqlParameter>
    {
        new("player_id", player.PlayerId ?? ""),
        new("full_name", player.FullName ?? ""),
         new("date_of_birth", player.DateOfBirth == null ? DBNull.Value : (object)player.DateOfBirth),
        new("nationality", player.Nationality ?? ""),
        new("position_code", player.PositionCode ?? ""),
        new("preferred_foot", player.PreferredFoot ?? ""),
        new("height_cm", player.HeightCm == 0 ? DBNull.Value : (object)player.HeightCm),
        new("weight_kg", player.WeightKg == 0 ? DBNull.Value : (object)player.WeightKg),
        new("current_club_id", player.CurrentClubId ?? (object)DBNull.Value),
         new("contract_start_date", player.ContractStartDate == null ? DBNull.Value : (object)player.ContractStartDate),
        new("contract_end_date", player.ContractEndDate == null ? DBNull.Value : (object)player.ContractEndDate),
        new("agent_name", player.AgentName ?? ""),
        new("agent_scout_id", player.AgentScoutId ?? (object)DBNull.Value),
        new("contact_info", player.ContactInfo ?? (object)DBNull.Value),
        new("profile_image_url", player.ProfileImageUrl ?? (object)DBNull.Value),
        new("created_at", player.CreatedAt),
        new("updated_at", player.UpdatedAt)
    };

        // ✅ Add only for edit
        if (type == "edit")
        {
            parameters.Add(new NpgsqlParameter("player_email", player.playerEmail ?? (object)DBNull.Value));
        }

        return parameters;
    }

    private Player1 MapReaderToPlayer(NpgsqlDataReader reader)
    {
        /*  var dateOfBirth = reader["date_of_birth"] == DBNull.Value ? null : (DateOnly?)DateOnly.FromDateTime((DateTime)reader["date_of_birth"]);
          var contractStartDate = reader["contract_start_date"] == DBNull.Value ? null : (DateOnly?)DateOnly.FromDateTime((DateTime)reader["contract_start_date"]);
          var contractEndDate = reader["contract_end_date"] == DBNull.Value ? null : (DateOnly?)DateOnly.FromDateTime((DateTime)reader["contract_end_date"]);
        */
        DateOnly? dateOfBirth = reader["date_of_birth"] == DBNull.Value
            ? null
            : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("date_of_birth")));

        DateOnly? contractStartDate = reader["contract_start_date"] == DBNull.Value
            ? null
            : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("contract_start_date")));

        DateOnly? contractEndDate = reader["contract_end_date"] == DBNull.Value
            ? null
            : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("contract_end_date")));



        var heightCm = reader["height_cm"] == DBNull.Value ? 0 : Convert.ToInt32(reader["height_cm"]);
        var weightKg = reader["weight_kg"] == DBNull.Value ? 0 : Convert.ToInt32(reader["weight_kg"]);

        return new Player1
        {
            PlayerId = reader["player_id"].ToString(),
            FullName = reader["full_name"].ToString(),
            DateOfBirth = dateOfBirth ?? default,
            Nationality = reader["nationality"].ToString(),
            PositionCode = reader["position_code"].ToString(),
            PreferredFoot = reader["preferred_foot"].ToString(),
            HeightCm = heightCm,
            WeightKg = weightKg,
            CurrentClubId = reader["current_club_id"] == DBNull.Value ? null : reader["current_club_id"].ToString(),
            ContractStartDate = contractStartDate ?? default,
            ContractEndDate = contractEndDate ?? default,
            AgentName = reader["agent_name"].ToString(),
            AgentScoutId = reader["agent_scout_id"] == DBNull.Value ? null : reader["agent_scout_id"].ToString(),
            ContactInfo = reader["contact_info"] == DBNull.Value ? null : reader["contact_info"].ToString(),
            ProfileImageUrl = reader["profile_image_url"] == DBNull.Value ? null : reader["profile_image_url"].ToString(),
            playerEmail = HasColumn(reader, "player_email") && reader["player_email"] != DBNull.Value
                ? reader["player_email"].ToString()!
                : string.Empty,
            CreatedAt = (DateTime)reader["created_at"],
            UpdatedAt = (DateTime)reader["updated_at"]
        };
    }

    private static bool HasColumn(NpgsqlDataReader reader, string columnName)
    {
        for (var i = 0; i < reader.FieldCount; i++)
        {
            if (string.Equals(reader.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
