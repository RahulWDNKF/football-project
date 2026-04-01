using FootballDashboardAPI.Models;
using Npgsql;
using NpgsqlTypes;

namespace FootballDashboardAPI.Repositories;

public class ReviewRatingRepository : IReviewRatingRepository
{
    private readonly PostgresConnectionProvider _db;

    public ReviewRatingRepository(PostgresConnectionProvider db)
    {
        _db = db;
    }

    public async Task<IEnumerable<ReviewRating>> GetAllAsync()
    {
        return await _db.ExecuteQueryListAsync(
            "SELECT * FROM stf.fn_review_ratings_get_all()",
            MapReaderToReviewRating
        );
    }

    public async Task<ReviewRating?> GetByIdAsync(string id)
    {
        return await _db.ExecuteQuerySingleAsync(
            "SELECT * FROM stf.fn_review_ratings_get_by_id(@p_id)",
            MapReaderToReviewRating,
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
    }

    public async Task<bool> ExistsAsync(string id)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT stf.fn_review_ratings_exists(@p_id)",
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    public async Task<ReviewRating> CreateAsync(ReviewRating rating)
    {
        await _db.ExecuteNonQueryAsync(
            "SELECT stf.fn_review_ratings_insert(@p_review_id, @p_passing, @p_shooting, @p_dribbling, @p_tactical_awareness, @p_defensive_contribution, @p_physical_strength, @p_behavior, @p_overall_performance)",
            new NpgsqlParameter("p_review_id", NpgsqlDbType.Varchar)
            { Value = rating.ReviewId },
            new NpgsqlParameter("p_passing", NpgsqlDbType.Numeric)
            { Value = rating.Passing },
            new NpgsqlParameter("p_shooting", NpgsqlDbType.Numeric)
            { Value = rating.Shooting },
            new NpgsqlParameter("p_dribbling", NpgsqlDbType.Numeric)
            { Value = rating.Dribbling },
            new NpgsqlParameter("p_tactical_awareness", NpgsqlDbType.Numeric)
            { Value = rating.TacticalAwareness },
            new NpgsqlParameter("p_defensive_contribution", NpgsqlDbType.Numeric)
            { Value = rating.DefensiveContribution },
            new NpgsqlParameter("p_physical_strength", NpgsqlDbType.Numeric)
            { Value = rating.PhysicalStrength },
            new NpgsqlParameter("p_behavior", NpgsqlDbType.Numeric)
            { Value = rating.Behavior },
            new NpgsqlParameter("p_overall_performance", NpgsqlDbType.Numeric)
            { Value = rating.OverallPerformance }
        );

        return await GetByIdAsync(rating.ReviewId) ?? rating;
    }

    public async Task<ReviewRating?> UpdateAsync(ReviewRating rating)
    {
        var existing = await GetByIdAsync(rating.ReviewId);
        if (existing == null) return null;

        await _db.ExecuteNonQueryAsync(
            "SELECT stf.fn_review_ratings_update(@p_review_id, @p_passing, @p_shooting, @p_dribbling, @p_tactical_awareness, @p_defensive_contribution, @p_physical_strength, @p_behavior, @p_overall_performance)",
            new NpgsqlParameter("p_review_id", NpgsqlDbType.Varchar)
            { Value = rating.ReviewId },
            new NpgsqlParameter("p_passing", NpgsqlDbType.Numeric)
            { Value = rating.Passing },
            new NpgsqlParameter("p_shooting", NpgsqlDbType.Numeric)
            { Value = rating.Shooting },
            new NpgsqlParameter("p_dribbling", NpgsqlDbType.Numeric)
            { Value = rating.Dribbling },
            new NpgsqlParameter("p_tactical_awareness", NpgsqlDbType.Numeric)
            { Value = rating.TacticalAwareness },
            new NpgsqlParameter("p_defensive_contribution", NpgsqlDbType.Numeric)
            { Value = rating.DefensiveContribution },
            new NpgsqlParameter("p_physical_strength", NpgsqlDbType.Numeric)
            { Value = rating.PhysicalStrength },
            new NpgsqlParameter("p_behavior", NpgsqlDbType.Numeric)
            { Value = rating.Behavior },
            new NpgsqlParameter("p_overall_performance", NpgsqlDbType.Numeric)
            { Value = rating.OverallPerformance }
        );

        return await GetByIdAsync(rating.ReviewId);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _db.ExecuteScalarAsync(
            "SELECT stf.fn_review_ratings_delete(@p_id)",
            new NpgsqlParameter("p_id", NpgsqlDbType.Varchar) { Value = id }
        );
        return Convert.ToInt32(result ?? 0) > 0;
    }

    private ReviewRating MapReaderToReviewRating(NpgsqlDataReader reader)
    {
        return new ReviewRating
        {
            ReviewId = reader["review_id"].ToString()!,
            Passing = (decimal)reader["passing"],
            Shooting = (decimal)reader["shooting"],
            Dribbling = (decimal)reader["dribbling"],
            TacticalAwareness = (decimal)reader["tactical_awareness"],
            DefensiveContribution = (decimal)reader["defensive_contribution"],
            PhysicalStrength = (decimal)reader["physical_strength"],
            Behavior = (decimal)reader["behavior"],
            OverallPerformance = (decimal)reader["overall_performance"]
        };
    }
}
