using Npgsql;

namespace FootballDashboardAPI.Repositories;

/// <summary>
/// Base class for PostgreSQL repository operations
/// Provides common patterns for executing commands and mapping results
/// </summary>
public abstract class BasePostgresRepository
{
    protected readonly PostgresConnectionProvider ConnectionProvider;

    protected BasePostgresRepository(PostgresConnectionProvider connectionProvider)
    {
        ConnectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    /// <summary>
    /// Executes a PostgreSQL function that returns a scalar value
    /// </summary>
    protected async Task<object?> ExecuteScalarAsync(string functionCall, params NpgsqlParameter[] parameters)
    {
        await using var connection = await ConnectionProvider.GetOpenConnectionAsync();
        await using var command = new NpgsqlCommand(functionCall, connection);

        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }

        return await command.ExecuteScalarAsync();
    }

    /// <summary>
    /// Executes a PostgreSQL function that returns no result
    /// </summary>
    protected async Task ExecuteNonQueryAsync(string functionCall, params NpgsqlParameter[] parameters)
    {
        await using var connection = await ConnectionProvider.GetOpenConnectionAsync();
        await using var command = new NpgsqlCommand(functionCall, connection);

        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }

        await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Executes a PostgreSQL function and returns a DataReader
    /// Caller must handle disposing the connection
    /// </summary>
    protected async Task<(NpgsqlConnection Connection, NpgsqlDataReader Reader)> ExecuteReaderAsync(string functionCall, params NpgsqlParameter[] parameters)
    {
        var connection = await ConnectionProvider.GetOpenConnectionAsync();
        var command = new NpgsqlCommand(functionCall, connection);

        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }

        var reader = await command.ExecuteReaderAsync();
        return (connection, reader);
    }

    /// <summary>
    /// Safely converts DBNull to the appropriate null value
    /// </summary>
    protected static object? GetDbValue(object? value)
    {
        return value == null || value == DBNull.Value ? null : value;
    }
}
