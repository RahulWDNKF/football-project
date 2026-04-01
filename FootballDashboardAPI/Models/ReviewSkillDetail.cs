using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FootballDashboardAPI.Models;

[PrimaryKey("ReviewId", "SkillKey")]
[Table("review_skill_details", Schema = "stf")]
public partial class ReviewSkillDetail
{
    [Key]
    [Column("review_id")]
    [StringLength(50)]
    public string ReviewId { get; set; } = null!;

    [Key]
    [Column("skill_key")]
    [StringLength(50)]
    public string SkillKey { get; set; } = null!;

    [Column("rating", TypeName = "decimal(3, 1)")]
    public decimal Rating { get; set; }

    [Column("comment_text")]
    public string? CommentText { get; set; }

    [Column("follow_up_date")]
    public DateOnly? FollowUpDate { get; set; }

    [ForeignKey("ReviewId")]
    [InverseProperty("ReviewSkillDetails")]
    public virtual Review Review { get; set; } = null!;
}
