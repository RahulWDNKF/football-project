using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FootballDashboardAPI.Models;

[Table("reviews", Schema = "stf")]
[Index("PlayerId", "MatchDate", Name = "IX_reviews_player_match_date", IsDescending = new[] { false, true })]
public partial class Review
{
    [Key]
    [Column("review_id")]
    [StringLength(50)]
    public string ReviewId { get; set; } = null!;

    [Column("player_id")]
    [StringLength(50)]
    public string PlayerId { get; set; } = null!;

    [Column("scout_id")]
    [StringLength(50)]
    public string ScoutId { get; set; } = null!;

    [Column("match_date")]
    public DateOnly? MatchDate { get; set; }

    [Column("club1_id")]
    [StringLength(50)]
    public string? Club1Id { get; set; }

    [Column("club2_id")]
    [StringLength(50)]
    public string? Club2Id { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("Club1Id")]
    [InverseProperty("ReviewClub1s")]
    public virtual Club? Club1 { get; set; }

    [ForeignKey("Club2Id")]
    [InverseProperty("ReviewClub2s")]
    public virtual Club? Club2 { get; set; }

    [ForeignKey("PlayerId")]
    [InverseProperty("Reviews")]
    public virtual Player1 Player { get; set; } = null!;

    [InverseProperty("Review")]
    public virtual ReviewRating? ReviewRating { get; set; }

    [InverseProperty("Review")]
    public virtual ICollection<ReviewSkillDetail> ReviewSkillDetails { get; set; } = new List<ReviewSkillDetail>();

    [ForeignKey("ScoutId")]
    [InverseProperty("Reviews")]
    public virtual Scout Scout { get; set; } = null!;
}

public class CreateReview
{
    public string PlayerId { get; set; } = null!;
    public string ScoutId { get; set; } = null!;
    public DateOnly? MatchDate { get; set; }
    public string? Club1Id { get; set; }
    public string? Club2Id { get; set; }
    public string? Notes { get; set; }
}

public class UpdateReview
{
    public DateOnly? MatchDate { get; set; }
    public string? Club1Id { get; set; }
    public string? Club2Id { get; set; }
    public string? Notes { get; set; }
}
