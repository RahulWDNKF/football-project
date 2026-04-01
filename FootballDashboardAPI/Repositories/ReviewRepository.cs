using FootballDashboardAPI.Models;
using Npgsql;
using NpgsqlTypes;

namespace FootballDashboardAPI.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly PostgresConnectionProvider _db;

    public ReviewRepository(PostgresConnectionProvider db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Review>> GetAllAsync()
    {
        return await _db.ExecuteQueryListAsync(
            "SELECT * FROM stf.fn_reviews_get_all()",
            MapReaderToReview
        );
    }

    public async Task<Review?> GetByIdAsync(string id)
    {
        return await _db.ExecuteQuerySingleAsync(
            "SELECT * FROM stf.fn_reviews_get_by_id(@p_id)",
            MapReaderToReview,
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
    }

    public async Task<Review> CreateAsync(Review review)
    {
        await _db.ExecuteNonQueryAsync(
            "SELECT stf.fn_reviews_insert(@p_review_id, @p_player_id, @p_scout_id, @p_match_date, @p_created_at, @p_club1_id, @p_club2_id, @p_notes)",
            new NpgsqlParameter("p_review_id", NpgsqlDbType.Varchar)
            { Value = review.ReviewId },
            new NpgsqlParameter("p_player_id", NpgsqlDbType.Varchar)
            { Value = review.PlayerId },
            new NpgsqlParameter("p_scout_id", NpgsqlDbType.Varchar)
            { Value = review.ScoutId },
            new NpgsqlParameter("p_match_date", NpgsqlDbType.Date)
            { Value = review.MatchDate.HasValue ? (object)review.MatchDate.Value.ToDateTime(System.TimeOnly.MinValue) : DBNull.Value },
            new NpgsqlParameter("p_created_at", NpgsqlDbType.Timestamp)
            { Value = DateTime.SpecifyKind(review.CreatedAt, DateTimeKind.Unspecified) },
            new NpgsqlParameter("p_club1_id", NpgsqlDbType.Varchar)
            { Value = review.Club1Id == null ? DBNull.Value : (object)review.Club1Id },
            new NpgsqlParameter("p_club2_id", NpgsqlDbType.Varchar)
            { Value = review.Club2Id == null ? DBNull.Value : (object)review.Club2Id },
            new NpgsqlParameter("p_notes", NpgsqlDbType.Text)
            { Value = review.Notes == null ? DBNull.Value : (object)review.Notes }
        );

        return await GetByIdAsync(review.ReviewId) ?? review;
    }

    public async Task<Review?> UpdateAsync(Review review)
    {
        await _db.ExecuteNonQueryAsync(
            "SELECT stf.fn_reviews_update(@p_review_id, @p_match_date, @p_club1_id, @p_club2_id, @p_notes)",
            new NpgsqlParameter("p_review_id", NpgsqlDbType.Varchar)
            { Value = review.ReviewId },
            new NpgsqlParameter("p_match_date", NpgsqlDbType.Date)
            { Value = review.MatchDate.HasValue ? (object)review.MatchDate.Value.ToDateTime(System.TimeOnly.MinValue) : DBNull.Value },
            new NpgsqlParameter("p_club1_id", NpgsqlDbType.Varchar)
            { Value = review.Club1Id == null ? DBNull.Value : (object)review.Club1Id },
            new NpgsqlParameter("p_club2_id", NpgsqlDbType.Varchar)
            { Value = review.Club2Id == null ? DBNull.Value : (object)review.Club2Id },
            new NpgsqlParameter("p_notes", NpgsqlDbType.Text)
            { Value = review.Notes == null ? DBNull.Value : (object)review.Notes }
        );

        return await GetByIdAsync(review.ReviewId);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT stf.fn_reviews_delete(@p_id)",
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    public async Task<bool> ExistsAsync(string id)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT stf.fn_reviews_exists(@p_id)",
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    public async Task<IEnumerable<Review>> GetByPlayerIdAsync(string playerId)
    {
        return await _db.ExecuteQueryListAsync(
            "SELECT * FROM stf.fn_reviews_get_by_player_id(@p_player_id)",
            MapReaderToReview,
            new NpgsqlParameter("p_player_id", NpgsqlDbType.Varchar) { Value = playerId }
        );
    }

    public async Task<IEnumerable<Review>> GetByScoutIdAsync(string scoutId)
    {
        return await _db.ExecuteQueryListAsync(
            "SELECT * FROM stf.fn_reviews_get_by_scout_id(@p_scout_id)",
            MapReaderToReview,
            new NpgsqlParameter("p_scout_id", NpgsqlDbType.Varchar) { Value = scoutId }
        );
    }

    //public async Task<string> GetLastIdAsync()
    //{
    //    var result = await _db.ExecuteScalarAsync(
    //        "SELECT MAX(CAST(SUBSTRING(review_id, 2) AS INTEGER)) FROM stf.reviews WHERE review_id ~ '^r\\d+$'"
    //    );
    //    var maxNumber = result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
    //    return $"r{maxNumber}";
    //}
    public async Task<long> GetLastIdAsync()
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT MAX(CAST(review_id AS BIGINT)) FROM stf.reviews WHERE review_id ~ '^[0-9]+$'"
        );
        return result == null || result == DBNull.Value ? 0 : Convert.ToInt64(result);
    }

    private Review MapReaderToReview(NpgsqlDataReader reader)
    {
        return new Review
        {
            ReviewId = reader["review_id"].ToString()!,
            PlayerId = reader["player_id"].ToString()!,
            ScoutId = reader["scout_id"].ToString()!,
            MatchDate = reader["match_date"] == DBNull.Value ? null : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("match_date"))),
            Club1Id = reader["club1_id"] == DBNull.Value ? null : reader["club1_id"].ToString(),
            Club2Id = reader["club2_id"] == DBNull.Value ? null : reader["club2_id"].ToString(),
            Notes = reader["notes"] == DBNull.Value ? null : reader["notes"].ToString(),
            CreatedAt = (DateTime)reader["created_at"]
        };
    }
}
