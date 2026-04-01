using FootballDashboardAPI.Models;
using FootballDashboardAPI.Repositories;
using Npgsql;
using NpgsqlTypes;

namespace FootballDashboardAPI.Services;

public class DocumentService : IDocumentService
{
    private readonly PostgresConnectionProvider _db;

    public DocumentService(PostgresConnectionProvider db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Document>> GetAllAsync()
    {
        return await _db.ExecuteQueryListAsync(
            "SELECT * FROM stf.fn_documents_get_all()",
            MapReaderToDocument
        );
    }

    public async Task<Document?> GetByIdAsync(string id)
    {
        return await _db.ExecuteQuerySingleAsync(
            "SELECT * FROM stf.fn_documents_get_by_id(@p_id)",
            MapReaderToDocument,
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
    }

    public async Task<Document> CreateAsync(CreateDocument dto)
    {
        //  var id = Guid.NewGuid().ToString();
        var lastIdResult = await _db.ExecuteScalarAsync(
          "SELECT MAX(CAST(document_id AS INTEGER)) FROM stf.documents WHERE document_id ~ '^\\d+$'"
      );
        int nextNumber = 1;
        if (lastIdResult != null && lastIdResult != DBNull.Value)
        {
            nextNumber = Convert.ToInt32(lastIdResult) + 1;

        }
        var id = $"{nextNumber}";
        var createdAt = DateTime.UtcNow;

        await _db.ExecuteNonQueryAsync(
            "SELECT stf.fn_documents_insert(@p_document_id, @p_document_name, @p_document_type, @p_document_date, @p_created_at, @p_file_data, @p_player_id, @p_club_id, @p_file_size_label)",
            new NpgsqlParameter("p_document_id", NpgsqlDbType.Varchar)
            { Value = id },
            new NpgsqlParameter("p_document_name", NpgsqlDbType.Varchar)
            { Value = dto.DocumentName },
            new NpgsqlParameter("p_document_type", NpgsqlDbType.Varchar)
            { Value = dto.DocumentType },
            new NpgsqlParameter("p_document_date", NpgsqlDbType.Timestamp)
            { Value = DateTime.SpecifyKind(dto.DocumentDate, DateTimeKind.Unspecified) },
            new NpgsqlParameter("p_created_at", NpgsqlDbType.Timestamp)
            { Value = DateTime.SpecifyKind(createdAt, DateTimeKind.Unspecified) },
            new NpgsqlParameter("p_file_data", NpgsqlDbType.Bytea)
            { Value = dto.FileData == null ? DBNull.Value : (object)dto.FileData },
            new NpgsqlParameter("p_player_id", NpgsqlDbType.Varchar)
            { Value = dto.PlayerId == null ? DBNull.Value : (object)dto.PlayerId },
            new NpgsqlParameter("p_club_id", NpgsqlDbType.Varchar)
            { Value = dto.ClubId == null ? DBNull.Value : (object)dto.ClubId },
            new NpgsqlParameter("p_file_size_label", NpgsqlDbType.Varchar)
            { Value = dto.FileSizeLabel == null ? DBNull.Value : (object)dto.FileSizeLabel }
        );

        return await GetByIdAsync(id) ?? new Document
        {
            DocumentId = id,
            DocumentName = dto.DocumentName,
            DocumentType = dto.DocumentType,
            CreatedAt = createdAt
        };
    }

    public async Task<bool> UpdateAsync(string id, UpdateDocument dto)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT stf.fn_documents_update(@p_document_id, @p_document_name, @p_document_type, @p_document_date, @p_file_data, @p_player_id, @p_club_id, @p_file_size_label)",
            new NpgsqlParameter("p_document_id", NpgsqlDbType.Varchar)
            { Value = id },
            new NpgsqlParameter("p_document_name", NpgsqlDbType.Varchar)
            { Value = dto.DocumentName == null ? DBNull.Value : (object)dto.DocumentName },
            new NpgsqlParameter("p_document_type", NpgsqlDbType.Varchar)
            { Value = dto.DocumentType == null ? DBNull.Value : (object)dto.DocumentType },
            new NpgsqlParameter("p_document_date", NpgsqlDbType.Timestamp)
            { Value = dto.DocumentDate == null ? DBNull.Value : (object)DateTime.SpecifyKind(dto.DocumentDate.Value, DateTimeKind.Unspecified) },
            new NpgsqlParameter("p_file_data", NpgsqlDbType.Bytea)
            { Value = dto.FileData == null ? DBNull.Value : (object)dto.FileData },
            new NpgsqlParameter("p_player_id", NpgsqlDbType.Varchar)
            { Value = dto.PlayerId == null ? DBNull.Value : (object)dto.PlayerId },
            new NpgsqlParameter("p_club_id", NpgsqlDbType.Varchar)
            { Value = dto.ClubId == null ? DBNull.Value : (object)dto.ClubId },
            new NpgsqlParameter("p_file_size_label", NpgsqlDbType.Varchar)
            { Value = dto.FileSizeLabel == null ? DBNull.Value : (object)dto.FileSizeLabel }
        );

        return Convert.ToInt32(result ?? 0) > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT stf.fn_documents_delete(@p_id)",
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    private Document MapReaderToDocument(NpgsqlDataReader reader)
    {
        return new Document
        {
            DocumentId = reader["document_id"].ToString()!,
            PlayerId = reader["player_id"] == DBNull.Value ? null : reader["player_id"].ToString(),
            ClubId = reader["club_id"] == DBNull.Value ? null : reader["club_id"].ToString(),
            DocumentName = reader["document_name"].ToString()!,
            DocumentType = reader["document_type"].ToString()!,
            DocumentDate = reader["document_date"] == DBNull.Value ? default : (DateTime)reader["document_date"],
            FileSizeLabel = reader["file_size_label"] == DBNull.Value ? null : reader["file_size_label"].ToString(),
            FileData = reader["file_data"] == DBNull.Value ? null : (byte[])reader["file_data"],
            CreatedAt = (DateTime)reader["created_at"]
        };
    }
}
