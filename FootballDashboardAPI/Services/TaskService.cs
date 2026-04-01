using FootballDashboardAPI.Models;
using FootballDashboardAPI.Repositories;
using Npgsql;
using NpgsqlTypes;
using Task = FootballDashboardAPI.Models.Task;

namespace FootballDashboardAPI.Services;

public class TaskService : ITaskService
{
    private readonly PostgresConnectionProvider _db;

    public TaskService(PostgresConnectionProvider db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Task>> GetAllTasksAsync()
    {
        return await _db.ExecuteQueryListAsync(
            "SELECT * FROM stf.fn_tasks_get_all()",
            MapReaderToTask
        );
    }

    public async Task<Task?> GetTaskByIdAsync(string id)
    {
        return await _db.ExecuteQuerySingleAsync(
            "SELECT * FROM stf.fn_tasks_get_by_id(@p_id)",
            MapReaderToTask,
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
    }

    public async Task<Task> CreateTaskAsync(CreateTask t)
    {
        //var taskId = Guid.NewGuid().ToString();

        var lastIdResult = await _db.ExecuteScalarAsync(
        "SELECT MAX(CAST(task_id AS BIGINT)) FROM stf.tasks"
          );
        var lastId = lastIdResult == null || lastIdResult == DBNull.Value
            ? 0
            : Convert.ToInt64(lastIdResult);
        var taskId = (lastId + 1).ToString();

        var createdAt = DateTime.UtcNow;

        await _db.ExecuteNonQueryAsync(
            "SELECT stf.fn_tasks_insert(@p_task_id, @p_title, @p_assigned_to_scout_id, @p_due_date, @p_status, @p_source, @p_created_at, @p_description, @p_player_id, @p_club_id)",
            new NpgsqlParameter("p_task_id", NpgsqlDbType.Varchar)
            { Value = taskId },
            new NpgsqlParameter("p_title", NpgsqlDbType.Varchar)
            { Value = t.Title },
            new NpgsqlParameter("p_assigned_to_scout_id", NpgsqlDbType.Varchar)
            { Value = t.AssignedToScoutId },
            new NpgsqlParameter("p_due_date", NpgsqlDbType.Date)
            { Value = t.DueDate },
            new NpgsqlParameter("p_status", NpgsqlDbType.Varchar)
            { Value = t.Status },
            new NpgsqlParameter("p_source", NpgsqlDbType.Varchar)
            { Value = t.Source },
            new NpgsqlParameter("p_created_at", NpgsqlDbType.Timestamp)
            { Value = DateTime.SpecifyKind(createdAt, DateTimeKind.Unspecified) },
            new NpgsqlParameter("p_description", NpgsqlDbType.Text)
            { Value = t.Description == null ? DBNull.Value : (object)t.Description },
            new NpgsqlParameter("p_player_id", NpgsqlDbType.Varchar)
            { Value = t.PlayerId == null ? DBNull.Value : (object)t.PlayerId },
            new NpgsqlParameter("p_club_id", NpgsqlDbType.Varchar)
            { Value = t.ClubId == null ? DBNull.Value : (object)t.ClubId }
        );

        return await GetTaskByIdAsync(taskId) ?? new Task
        {
            TaskId = taskId,
            Title = t.Title,
            Description = t.Description,
            PlayerId = t.PlayerId,
            ClubId = t.ClubId,
            AssignedToScoutId = t.AssignedToScoutId,
            DueDate = t.DueDate,
            Status = t.Status,
            Source = t.Source,
            CreatedAt = createdAt
        };
    }

    public async Task<Task?> UpdateTaskAsync(string id, UpdateTask dto)
    {
        var existing = await GetTaskByIdAsync(id);
        if (existing == null)
            return null;

        await _db.ExecuteNonQueryAsync(
            "SELECT stf.fn_tasks_update(@p_task_id, @p_title, @p_assigned_to_scout_id, @p_due_date, @p_status, @p_source, @p_description, @p_player_id, @p_club_id)",
            new NpgsqlParameter("p_task_id", NpgsqlDbType.Varchar)
            { Value = id },
            new NpgsqlParameter("p_title", NpgsqlDbType.Varchar)
            { Value = dto.Title ?? existing.Title },
            new NpgsqlParameter("p_assigned_to_scout_id", NpgsqlDbType.Varchar)
            { Value = dto.AssignedToScoutId ?? existing.AssignedToScoutId },
            new NpgsqlParameter("p_due_date", NpgsqlDbType.Date)
            { Value = dto.DueDate ?? existing.DueDate },
            new NpgsqlParameter("p_status", NpgsqlDbType.Varchar)
            { Value = dto.Status ?? existing.Status },
            new NpgsqlParameter("p_source", NpgsqlDbType.Varchar)
            { Value = dto.Source ?? existing.Source },
            new NpgsqlParameter("p_description", NpgsqlDbType.Text)
            { Value = (dto.Description ?? existing.Description) == null ? DBNull.Value : (object)(dto.Description ?? existing.Description)! },
            new NpgsqlParameter("p_player_id", NpgsqlDbType.Varchar)
            { Value = (dto.PlayerId ?? existing.PlayerId) == null ? DBNull.Value : (object)(dto.PlayerId ?? existing.PlayerId)! },
            new NpgsqlParameter("p_club_id", NpgsqlDbType.Varchar)
            { Value = (dto.ClubId ?? existing.ClubId) == null ? DBNull.Value : (object)(dto.ClubId ?? existing.ClubId)! }
        );

        return await GetTaskByIdAsync(id);
    }

    public async Task<bool> DeleteTaskAsync(string id)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT stf.fn_tasks_delete(@p_id)",
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    private Task MapReaderToTask(NpgsqlDataReader reader)
    {
        return new Task
        {
            TaskId = reader["task_id"].ToString()!,
            Title = reader["title"].ToString()!,
            Description = reader["description"] == DBNull.Value ? null : reader["description"].ToString(),
            PlayerId = reader["player_id"] == DBNull.Value ? null : reader["player_id"].ToString(),
            ClubId = reader["club_id"] == DBNull.Value ? null : reader["club_id"].ToString(),
            AssignedToScoutId = reader["assigned_to_scout_id"].ToString()!,
            DueDate = reader["due_date"] == DBNull.Value ? default : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("due_date"))),
            Status = reader["status"].ToString()!,
            Source = reader["source"].ToString()!,
            CreatedAt = (DateTime)reader["created_at"]
        };
    }
}
