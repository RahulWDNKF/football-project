using FootballDashboardAPI.Models;
using Npgsql;

namespace FootballDashboardAPI.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PostgresConnectionProvider _db;

    public UserRepository(PostgresConnectionProvider db)
    {
        _db = db;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _db.ExecuteQueryListAsync(
            "SELECT * FROM stf.fn_users_get_all()",
            MapReaderToUser
        );
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        return await _db.ExecuteQuerySingleAsync(
            "SELECT * FROM stf.fn_users_get_by_id(@p_id)",
            MapReaderToUser,
            new NpgsqlParameter("p_id", id)
        );
    }

    public async Task<User> CreateAsync(User user)
    {
        user.CreatedAt ??= DateTime.UtcNow;

        await _db.ExecuteNonQueryAsync(
            "SELECT stf.fn_users_insert(@p_name, @p_email, @p_password, @p_role, @p_created_at, @p_phone, @p_status)",
            new NpgsqlParameter("p_name", NpgsqlTypes.NpgsqlDbType.Varchar)
            { Value = user.Name },
            new NpgsqlParameter("p_email", NpgsqlTypes.NpgsqlDbType.Varchar)
            { Value = user.Email },
            new NpgsqlParameter("p_password", NpgsqlTypes.NpgsqlDbType.Varchar)
            { Value = user.Password },
            new NpgsqlParameter("p_role", NpgsqlTypes.NpgsqlDbType.Varchar)
            { Value = user.Role },
            new NpgsqlParameter("p_created_at", NpgsqlTypes.NpgsqlDbType.Timestamp)
            { Value = DateTime.SpecifyKind(user.CreatedAt.Value, DateTimeKind.Unspecified) },
            new NpgsqlParameter("p_phone", NpgsqlTypes.NpgsqlDbType.Varchar)
            { Value = user.Phone == null ? DBNull.Value : (object)user.Phone },
            new NpgsqlParameter("p_status", NpgsqlTypes.NpgsqlDbType.Boolean)
            { Value = user.Status == null ? DBNull.Value : (object)user.Status }
        );

        return await GetByEmailAsync(user.Email) ?? user;
    }

    public async Task<User?> UpdateAsync(User user)
    {
        await _db.ExecuteNonQueryAsync(
            "SELECT stf.fn_users_update(@p_id, @p_name, @p_email, @p_role, @p_updated_at, @p_phone, @p_status)",
            new NpgsqlParameter("p_id", NpgsqlTypes.NpgsqlDbType.Bigint)
            { Value = user.Id },
            new NpgsqlParameter("p_name", NpgsqlTypes.NpgsqlDbType.Varchar)
            { Value = user.Name },
            new NpgsqlParameter("p_email", NpgsqlTypes.NpgsqlDbType.Varchar)
            { Value = user.Email },
            new NpgsqlParameter("p_role", NpgsqlTypes.NpgsqlDbType.Varchar)
            { Value = user.Role },
            new NpgsqlParameter("p_updated_at", NpgsqlTypes.NpgsqlDbType.Timestamp)
            { Value = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified) },
            new NpgsqlParameter("p_phone", NpgsqlTypes.NpgsqlDbType.Varchar)
            { Value = user.Phone == null ? DBNull.Value : (object)user.Phone },
            new NpgsqlParameter("p_status", NpgsqlTypes.NpgsqlDbType.Boolean)
            { Value = user.Status == null ? DBNull.Value : (object)user.Status }
        );

        return await GetByIdAsync(user.Id);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT * FROM stf.fn_users_delete(@p_id)",
            new NpgsqlParameter("p_id", id)
        );
        var affected = result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        return affected > 0;
    }

    public async Task<bool> ExistsAsync(long id)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT * FROM stf.fn_users_exists(@p_id)",
            new NpgsqlParameter("p_id", id)
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    public async Task<bool> EmailExistsAsync(string email, long? excludeUserId = null)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT * FROM stf.fn_users_email_exists(@p_email, @p_exclude_user_id)",
            new NpgsqlParameter("p_email", email),
            new NpgsqlParameter("p_exclude_user_id", excludeUserId == null ? DBNull.Value : (object)excludeUserId)
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    private async Task<User?> GetByEmailAsync(string email)
    {
        return await _db.ExecuteQuerySingleAsync(
            "SELECT * FROM stf.fn_users_get_by_email(@p_email)",
            MapReaderToUser,
            new NpgsqlParameter("p_email", email)
        );
    }

    private User MapReaderToUser(NpgsqlDataReader reader)
    {
        return new User
        {
            Id = (long)reader["id"],
            Name = reader["name"].ToString(),
            Email = reader["email"].ToString(),
            Password = reader["password"].ToString(),
            Role = reader["role"].ToString(),
            Phone = reader["phone"] == DBNull.Value ? null : reader["phone"].ToString(),
            Status = reader["status"] == DBNull.Value ? null : (bool?)reader["status"],
            CreatedAt = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"],
            UpdatedAt = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"]
        };
    }
}
