using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using FootballDashboardAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FootballDashboardAPI.Controllers;

[Authorize]
[ApiController]
public abstract class EntityCrudController<TEntity> : ControllerBase where TEntity : class, new()
{
    private readonly FootballContext _context;
    private readonly string _keyName;
    private readonly PropertyInfo _keyProperty;
    private readonly string _schemaName;
    private readonly string _tableName;
    private readonly List<PropertyInfo> _mappedProperties;

    protected EntityCrudController(FootballContext context)
    {
        _context = context;

        var entityType = _context.Model.FindEntityType(typeof(TEntity))
            ?? throw new InvalidOperationException($"Entity type '{typeof(TEntity).Name}' was not found in DbContext model.");

        var primaryKey = entityType.FindPrimaryKey()
            ?? throw new InvalidOperationException($"Entity type '{typeof(TEntity).Name}' does not have a primary key.");

        if (primaryKey.Properties.Count != 1)
        {
            throw new InvalidOperationException($"Entity type '{typeof(TEntity).Name}' has a composite primary key which is not supported by this controller.");
        }

        _keyName = primaryKey.Properties[0].Name;
        _keyProperty = typeof(TEntity).GetProperty(_keyName)
            ?? throw new InvalidOperationException($"Primary key property '{_keyName}' was not found on '{typeof(TEntity).Name}'.");

        var tableAttribute = typeof(TEntity).GetCustomAttribute<TableAttribute>();
        _tableName = tableAttribute?.Name ?? entityType.GetTableName() ?? typeof(TEntity).Name;
        _schemaName = tableAttribute?.Schema ?? entityType.GetSchema() ?? "dbo";

        //_mappedProperties = typeof(TEntity)
        //    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        //    .Where(p => p.CanRead && p.CanWrite)
        //    .Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null)
        //    .Where(p => IsSimpleType(p.PropertyType))
        //    .ToList();
        // WITH this � uses EF metadata to get ONLY actual DB columns:
        var efScalarPropertyNames = entityType
    .GetProperties()
    .Select(p => p.PropertyInfo?.Name)
    .Where(name => name != null)
    .ToHashSet();

        _mappedProperties = typeof(TEntity)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => efScalarPropertyNames.Contains(p.Name))
            .Where(p => p.CanRead && p.CanWrite)
            .Where(p => IsSimpleType(p.PropertyType))  // add this back as double safety
            .ToList();
    }

    [HttpGet]
    public virtual async Task<ActionResult<IEnumerable<TEntity>>> GetAll()
    {
        var items = await ExecuteReaderAsync(BuildProcedureName("get_all"), [], MapEntity);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public virtual async Task<ActionResult<TEntity>> GetById(string id)
    {
        var entity = await FindByIdAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    [HttpPost]
    public virtual async Task<ActionResult<TEntity>> Create([FromBody] TEntity entity)
    {
        var parameters = BuildEntityParameters(entity, includeKey: true);
        await ExecuteNonQueryAsync(BuildProcedureName("insert"), parameters);

        var keyValue = GetEntityKeyValue(entity)?.ToString() ?? string.Empty;
        var created = string.IsNullOrWhiteSpace(keyValue) ? entity : await FindByIdAsync(keyValue) ?? entity;

        return CreatedAtAction(nameof(GetById), new { id = keyValue }, created);
    }

    [HttpPut("{id}")]
    public virtual async Task<IActionResult> Update(string id, [FromBody] TEntity entity)
    {
        var entityId = GetEntityKeyValue(entity)?.ToString();
        if (!string.Equals(id, entityId, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Route id does not match entity id.");
        }

        var parameters = BuildEntityParameters(entity, includeKey: true);
        var affected = await ExecuteNonQueryAsync(BuildProcedureName("update"), parameters);

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> Delete(string id)
    {
        var typedId = ConvertIdToKeyType(id);
        if (typedId == null)
        {
            return NotFound();
        }

        var affected = await ExecuteNonQueryAsync(
            BuildProcedureName("delete"),
            [new SqlParameter("@Id", typedId)]);

        if (affected == 0)
        {
            return NotFound();
        }

        return NoContent();
    }

    private async Task<TEntity?> FindByIdAsync(string id)
    {
        var typedId = ConvertIdToKeyType(id);
        if (typedId == null)
        {
            return null;
        }

        var items = await ExecuteReaderAsync(
            BuildProcedureName("get_by_id"),
            [new SqlParameter("@Id", typedId)],
            MapEntity);

        return items.FirstOrDefault();
    }

    private async Task<int> ExecuteNonQueryAsync(string procedureName, IEnumerable<SqlParameter> parameters)
    {
        //await using var connection = _context.Database.GetDbConnection();
        var connectionString = _context.Database.GetConnectionString();
        await using var connection = new SqlConnection(connectionString);

        await using var command = connection.CreateCommand();

        command.CommandText = procedureName;
        command.CommandType = CommandType.StoredProcedure;

        foreach (var parameter in parameters)
        {
            command.Parameters.Add(parameter);
        }

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        return await command.ExecuteNonQueryAsync();
    }

    private async Task<List<T>> ExecuteReaderAsync<T>(string procedureName, IEnumerable<SqlParameter> parameters, Func<SqlDataReader, T> map)
    {
        //await using var connection = (SqlConnection)_context.Database.GetDbConnection();

        var connectionString = _context.Database.GetConnectionString();
        await using var connection = new SqlConnection(connectionString);

        await using var command = new SqlCommand(procedureName, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        foreach (var parameter in parameters)
        {
            command.Parameters.Add(parameter);
        }

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        var result = new List<T>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(map(reader));
        }

        return result;
    }

    private TEntity MapEntity(SqlDataReader reader)
    {
        var entity = new TEntity();

        foreach (var property in _mappedProperties)
        {
            var columnName = GetColumnName(property);
            if (!HasColumn(reader, columnName))
            {
                continue;
            }

            var value = reader[columnName];
            if (value == DBNull.Value)
            {
                continue;
            }

            var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            var convertedValue = Convert.ChangeType(value, targetType);
            property.SetValue(entity, convertedValue);
        }

        return entity;
    }

    //private List<SqlParameter> BuildEntityParameters(TEntity entity, bool includeKey)
    //{
    //    var parameters = new List<SqlParameter>();

    //    foreach (var property in _mappedProperties)
    //    {
    //        if (!includeKey && property.Name == _keyName)
    //        {
    //            continue;
    //        }

    //        var columnName = GetColumnName(property);
    //        var value = property.GetValue(entity) ?? DBNull.Value;
    //        parameters.Add(new SqlParameter($"@{columnName}", value));
    //    }

    //    return parameters;
    //}
    private List<SqlParameter> BuildEntityParameters(TEntity entity, bool includeKey)
    {
        var parameters = new List<SqlParameter>();

        foreach (var property in _mappedProperties)
        {
            if (!includeKey && property.Name == _keyName)
                continue;

            var columnName = GetColumnName(property);

            object? value;
            try
            {
                value = property.GetValue(entity) ?? DBNull.Value;
            }
            catch
            {
                continue; // skip any property that fails reflection
            }

            // Skip if value is a complex object (nav property slipped through)
            if (value != DBNull.Value && value != null && !IsSimpleType(value.GetType()))
                continue;

            parameters.Add(new SqlParameter($"@{columnName}", value));
        }

        return parameters;
    }

    private object? ConvertIdToKeyType(string id)
    {
        if (_keyProperty.PropertyType == typeof(Guid) || _keyProperty.PropertyType == typeof(Guid?))
        {
            return Guid.TryParse(id, out var guidValue) ? guidValue : null;
        }

        if (_keyProperty.PropertyType == typeof(int) || _keyProperty.PropertyType == typeof(int?))
        {
            return int.TryParse(id, out var intValue) ? intValue : null;
        }

        if (_keyProperty.PropertyType == typeof(long) || _keyProperty.PropertyType == typeof(long?))
        {
            return long.TryParse(id, out var longValue) ? longValue : null;
        }

        return id;
    }

    private object? GetEntityKeyValue(TEntity entity) => _keyProperty.GetValue(entity);

    private string BuildProcedureName(string action) => $"[{_schemaName}].[sp_{_tableName}_{action}]";

    private static bool IsSimpleType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        return underlyingType.IsPrimitive
            || underlyingType.IsEnum
            || underlyingType == typeof(string)
            || underlyingType == typeof(decimal)
            || underlyingType == typeof(DateTime)
            || underlyingType == typeof(DateTimeOffset)
            || underlyingType == typeof(TimeSpan)
            || underlyingType == typeof(Guid);
    }

    private static string GetColumnName(PropertyInfo property) =>
        property.GetCustomAttribute<ColumnAttribute>()?.Name ?? property.Name;

    private static bool HasColumn(SqlDataReader reader, string columnName)
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
