using FootballDashboardAPI.Models;
using Npgsql;
using NpgsqlTypes;

namespace FootballDashboardAPI.Repositories;

public class TemplateRepository : ITemplateRepository
{
    private readonly PostgresConnectionProvider _db;

    public TemplateRepository(PostgresConnectionProvider db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Template>> GetAllAsync()
    {
        return await _db.ExecuteQueryListAsync(
            "SELECT * FROM stf.fn_templates_get_all()",
            MapReaderToTemplate
        );
    }

    public async Task<Template?> GetByIdAsync(string id)
    {
        return await _db.ExecuteQuerySingleAsync(
            "SELECT * FROM stf.fn_templates_get_by_id(@p_id)",
            MapReaderToTemplate,
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
    }

    public async Task<Template> CreateAsync(Template template)
    {

        //var lastIdResult = await _db.ExecuteScalarAsync(
        //    "SELECT MAX(CAST(SUBSTRING(template_id, 5) AS INTEGER)) FROM stf.templates WHERE template_id ~ '^tmpl\\d+$'"
        //);
        //int nextNumber = 1;
        //if (lastIdResult != null && lastIdResult != DBNull.Value)
        //{
        //    nextNumber = Convert.ToInt32(lastIdResult) + 1;
        //}

        //template.TemplateId = $"{nextNumber}";

        var lastIdResult = await _db.ExecuteScalarAsync(
            "SELECT MAX(CAST(template_id AS BIGINT)) FROM stf.templates WHERE template_id ~ '^[0-9]+$'"
        );
        var lastId = lastIdResult == null || lastIdResult == DBNull.Value
            ? 0
            : Convert.ToInt64(lastIdResult);
        template.TemplateId = (lastId + 1).ToString();

        if (template.CreatedAt == default)
            template.CreatedAt = DateTime.UtcNow;

        await _db.ExecuteNonQueryAsync(
            "SELECT stf.fn_templates_insert(@p_template_id, @p_template_name, @p_template_type, @p_body, @p_created_at, @p_subject)",
            new NpgsqlParameter("p_template_id", NpgsqlDbType.Varchar)
            { Value = template.TemplateId },
            new NpgsqlParameter("p_template_name", NpgsqlDbType.Varchar)
            { Value = template.TemplateName },
            new NpgsqlParameter("p_template_type", NpgsqlDbType.Varchar)
            { Value = template.TemplateType },
            new NpgsqlParameter("p_body", NpgsqlDbType.Text)
            { Value = template.Body },
            new NpgsqlParameter("p_created_at", NpgsqlDbType.Timestamp)
            { Value = DateTime.SpecifyKind(template.CreatedAt, DateTimeKind.Unspecified) },
            new NpgsqlParameter("p_subject", NpgsqlDbType.Varchar)
            { Value = template.Subject == null ? DBNull.Value : (object)template.Subject }
        );

        return await GetByIdAsync(template.TemplateId) ?? template;
    }

    public async Task<Template?> UpdateAsync(Template template)
    {
        await _db.ExecuteNonQueryAsync(
            "SELECT stf.fn_templates_update(@p_template_id, @p_template_name, @p_template_type, @p_body, @p_subject)",
            new NpgsqlParameter("p_template_id", NpgsqlDbType.Varchar)
            { Value = template.TemplateId },
            new NpgsqlParameter("p_template_name", NpgsqlDbType.Varchar)
            { Value = template.TemplateName },
            new NpgsqlParameter("p_template_type", NpgsqlDbType.Varchar)
            { Value = template.TemplateType },
            new NpgsqlParameter("p_body", NpgsqlDbType.Text)
            { Value = template.Body },
            new NpgsqlParameter("p_subject", NpgsqlDbType.Varchar)
            { Value = template.Subject == null ? DBNull.Value : (object)template.Subject }
        );

        return await GetByIdAsync(template.TemplateId);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT stf.fn_templates_delete(@p_id)",
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    public async Task<bool> ExistsAsync(string id)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT stf.fn_templates_exists(@p_id)",
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    public async Task<bool> TemplateNameExistsAsync(string templateName, string? excludeTemplateId = null)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT stf.fn_templates_name_exists(@p_template_name, @p_exclude_template_id)",
            new NpgsqlParameter("p_template_name", NpgsqlDbType.Varchar)
            { Value = templateName },
            new NpgsqlParameter("p_exclude_template_id", NpgsqlDbType.Varchar)
            { Value = excludeTemplateId == null ? DBNull.Value : (object)excludeTemplateId }
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    private Template MapReaderToTemplate(NpgsqlDataReader reader)
    {
        return new Template
        {
            TemplateId = reader["template_id"].ToString()!,
            TemplateName = reader["template_name"].ToString()!,
            TemplateType = reader["template_type"].ToString()!,
            Subject = reader["subject"] == DBNull.Value ? null : reader["subject"].ToString(),
            Body = reader["body"].ToString()!,
            CreatedAt = (DateTime)reader["created_at"]
        };
    }
}
