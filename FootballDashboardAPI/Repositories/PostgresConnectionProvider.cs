using Npgsql;

namespace FootballDashboardAPI.Repositories;

/// <summary>
/// Provides PostgreSQL database connections and helper methods for all repositories
/// </summary>
public class PostgresConnectionProvider
{
    private readonly string _connectionString;

    public PostgresConnectionProvider(string connectionString)
    {
        _connectionString =
            connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    /// <summary>
    /// Create connection
    /// </summary>
    public NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }

    /// <summary>
    /// Open connection async
    /// </summary>
    public async Task<NpgsqlConnection> GetOpenConnectionAsync()
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    /// <summary>
    /// Execute query and map custom
    /// </summary>
    public async Task<T> ExecuteQueryAsync<T>(
        string sql,
        Func<NpgsqlDataReader, Task<T>> mapperFunc,
        params NpgsqlParameter[] parameters)
    {
        await using var connection = await GetOpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);

        if (parameters != null && parameters.Length > 0)
            command.Parameters.AddRange(parameters);

        await using var reader = await command.ExecuteReaderAsync();

        return await mapperFunc(reader);
    }

    /// <summary>
    /// Execute query list
    /// </summary>
    public async Task<List<T>> ExecuteQueryListAsync<T>(
        string sql,
        Func<NpgsqlDataReader, T> mapperFunc,
        params NpgsqlParameter[] parameters)
    {
        var results = new List<T>();

        await using var connection = await GetOpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);

        if (parameters != null && parameters.Length > 0)
            command.Parameters.AddRange(parameters);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            results.Add(mapperFunc(reader));
        }

        return results;
    }

    /// <summary>
    /// Execute query single
    /// </summary>
    public async Task<T?> ExecuteQuerySingleAsync<T>(
        string sql,
        Func<NpgsqlDataReader, T> mapperFunc,
        params NpgsqlParameter[] parameters)
        where T : class
    {
        await using var connection = await GetOpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);

        if (parameters != null && parameters.Length > 0)
            command.Parameters.AddRange(parameters);

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return mapperFunc(reader);
        }

        return null;
    }

    /// <summary>
    /// Execute scalar
    /// </summary>
    public async Task<object?> ExecuteScalarAsync(
        string sql,
        params NpgsqlParameter[] parameters)
    {
        await using var connection = await GetOpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);

        if (parameters != null && parameters.Length > 0)
            command.Parameters.AddRange(parameters);

        var result = await command.ExecuteScalarAsync();

        return result == DBNull.Value ? null : result;
    }

    /// <summary>
    /// Execute non query
    /// </summary>
    public async Task<int> ExecuteNonQueryAsync(
        string sql,
        params NpgsqlParameter[] parameters)
    {
        await using var connection = await GetOpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);

        if (parameters != null && parameters.Length > 0)
            command.Parameters.AddRange(parameters);

        return await command.ExecuteNonQueryAsync();
    }
}