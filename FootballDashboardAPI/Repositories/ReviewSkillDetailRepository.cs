using FootballDashboardAPI.Models;
using Npgsql;
using NpgsqlTypes;

namespace FootballDashboardAPI.Repositories;

public class ReviewSkillDetailRepository : IReviewSkillDetailRepository
{
    private readonly PostgresConnectionProvider _db;

    public ReviewSkillDetailRepository(PostgresConnectionProvider db)
    {
        _db = db;
    }

    public async Task<IEnumerable<ReviewSkillDetail>> GetAllAsync()
    {
        return await _db.ExecuteQueryListAsync(
            "SELECT * FROM stf.fn_review_skill_details_get_all()",
            MapReaderToReviewSkillDetail
        );
    }

    public async Task<ReviewSkillDetail?> GetByIdAsync(string reviewId, string skillKey)
    {
        return await _db.ExecuteQuerySingleAsync(
            "SELECT * FROM stf.fn_review_skill_details_get_by_id(@p_review_id, @p_skill_key)",
            MapReaderToReviewSkillDetail,
            new NpgsqlParameter("p_review_id", NpgsqlDbType.Varchar) { Value = reviewId },
            new NpgsqlParameter("p_skill_key", NpgsqlDbType.Varchar) { Value = skillKey }
        );
    }

    public async Task<IEnumerable<ReviewSkillDetail>> GetByReviewIdAsync(string reviewId)
    {
        return await _db.ExecuteQueryListAsync(
            "SELECT * FROM stf.fn_review_skill_details_get_by_review_id(@p_review_id)",
            MapReaderToReviewSkillDetail,
            new NpgsqlParameter("p_review_id", NpgsqlDbType.Varchar) { Value = reviewId }
        );
    }

    public async Task<bool> ExistsAsync(string reviewId, string skillKey)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT stf.fn_review_skill_details_exists(@p_review_id, @p_skill_key)",
            new NpgsqlParameter("p_review_id", NpgsqlDbType.Varchar) { Value = reviewId },
            new NpgsqlParameter("p_skill_key", NpgsqlDbType.Varchar) { Value = skillKey }
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    public async Task<ReviewSkillDetail> CreateAsync(ReviewSkillDetail detail)
    {
        await _db.ExecuteNonQueryAsync(
            "SELECT stf.fn_review_skill_details_insert(@p_review_id, @p_skill_key, @p_rating, @p_comment_text, @p_follow_up_date)",
            new NpgsqlParameter("p_review_id", NpgsqlDbType.Varchar)
            { Value = detail.ReviewId },
            new NpgsqlParameter("p_skill_key", NpgsqlDbType.Varchar)
            { Value = detail.SkillKey },
            new NpgsqlParameter("p_rating", NpgsqlDbType.Numeric)
            { Value = detail.Rating },
            new NpgsqlParameter("p_comment_text", NpgsqlDbType.Text)
            { Value = detail.CommentText == null ? DBNull.Value : (object)detail.CommentText },
            new NpgsqlParameter("p_follow_up_date", NpgsqlDbType.Date)
            { Value = detail.FollowUpDate == null ? DBNull.Value : (object)detail.FollowUpDate }
        );

        return await GetByIdAsync(detail.ReviewId, detail.SkillKey) ?? detail;
    }

    public async Task<ReviewSkillDetail?> UpdateAsync(ReviewSkillDetail detail)
    {
        var existing = await GetByIdAsync(detail.ReviewId, detail.SkillKey);
        if (existing == null) return null;

        await _db.ExecuteNonQueryAsync(
            "SELECT stf.fn_review_skill_details_update(@p_review_id, @p_skill_key, @p_rating, @p_comment_text, @p_follow_up_date)",
            new NpgsqlParameter("p_review_id", NpgsqlDbType.Varchar)
            { Value = detail.ReviewId },
            new NpgsqlParameter("p_skill_key", NpgsqlDbType.Varchar)
            { Value = detail.SkillKey },
            new NpgsqlParameter("p_rating", NpgsqlDbType.Numeric)
            { Value = detail.Rating },
            new NpgsqlParameter("p_comment_text", NpgsqlDbType.Text)
            { Value = detail.CommentText == null ? DBNull.Value : (object)detail.CommentText },
            new NpgsqlParameter("p_follow_up_date", NpgsqlDbType.Date)
            { Value = detail.FollowUpDate == null ? DBNull.Value : (object)detail.FollowUpDate }
        );

        return await GetByIdAsync(detail.ReviewId, detail.SkillKey);
    }

    public async Task<bool> DeleteAsync(string reviewId, string skillKey)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT stf.fn_review_skill_details_delete(@p_review_id, @p_skill_key)",
            new NpgsqlParameter("p_review_id", NpgsqlDbType.Varchar) { Value = reviewId },
            new NpgsqlParameter("p_skill_key", NpgsqlDbType.Varchar) { Value = skillKey }
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    private ReviewSkillDetail MapReaderToReviewSkillDetail(NpgsqlDataReader reader)
    {
        return new ReviewSkillDetail
        {
            ReviewId = reader["review_id"].ToString()!,
            SkillKey = reader["skill_key"].ToString()!,
            Rating = (decimal)reader["rating"],
            CommentText = reader["comment_text"] == DBNull.Value ? null : reader["comment_text"].ToString(),
            FollowUpDate = reader["follow_up_date"] == DBNull.Value ? null : (DateOnly?)DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("follow_up_date")))
        };
    }
}
