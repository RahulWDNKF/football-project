using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace FootballDashboardAPI.Models;

[Table("review_ratings", Schema = "stf")]
public partial class ReviewRating
{
    [Key]
    [Column("review_id")]
    [StringLength(50)]
    public string ReviewId { get; set; } = null!;

    [Column("passing", TypeName = "decimal(3, 1)")]
    public decimal Passing { get; set; }

    [Column("shooting", TypeName = "decimal(3, 1)")]
    public decimal Shooting { get; set; }

    [Column("dribbling", TypeName = "decimal(3, 1)")]
    public decimal Dribbling { get; set; }

    [Column("tactical_awareness", TypeName = "decimal(3, 1)")]
    public decimal TacticalAwareness { get; set; }

    [Column("defensive_contribution", TypeName = "decimal(3, 1)")]
    public decimal DefensiveContribution { get; set; }

    [Column("physical_strength", TypeName = "decimal(3, 1)")]
    public decimal PhysicalStrength { get; set; }

    [Column("behavior", TypeName = "decimal(3, 1)")]
    public decimal Behavior { get; set; }

    [Column("overall_performance", TypeName = "decimal(3, 1)")]
    public decimal OverallPerformance { get; set; }

    [ForeignKey("ReviewId")]
    [InverseProperty("ReviewRating")]
    [JsonIgnore]
    public virtual Review Review { get; set; }
}
