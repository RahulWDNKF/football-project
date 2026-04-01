using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FootballDashboardAPI.Models;

[Table("notes", Schema = "stf")]
[Index("ClubId", "CreatedAt", Name = "IX_notes_club_created_at", IsDescending = new[] { false, true })]
[Index("PlayerId", "CreatedAt", Name = "IX_notes_player_created_at", IsDescending = new[] { false, true })]
public partial class Note
{
    [Key]
    [Column("note_id")]
    [StringLength(50)]
    public string NoteId { get; set; } = null!;

    [Column("player_id")]
    [StringLength(50)]
    public string? PlayerId { get; set; }

    [Column("club_id")]
    [StringLength(50)]
    public string? ClubId { get; set; }

    [Column("topic")]
    [StringLength(200)]
    public string Topic { get; set; } = null!;

    [Column("description")]
    public string Description { get; set; } = null!;

    [Column("category")]
    [StringLength(30)]
    public string Category { get; set; } = null!;

    [Column("follow_up_date")]
    public DateOnly? FollowUpDate { get; set; }

    [Column("created_by_scout_id")]
    [StringLength(50)]
    public string CreatedByScoutId { get; set; } = null!;

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("ClubId")]
    [InverseProperty("Notes")]
    public virtual Club? Club { get; set; }

    [ForeignKey("CreatedByScoutId")]
    [InverseProperty("Notes")]
    public virtual Scout CreatedByScout { get; set; } = null!;

    [ForeignKey("PlayerId")]
    [InverseProperty("Notes")]
    public virtual Player1? Player { get; set; }
}
public class CreateNote
{
    public string? PlayerId { get; set; }
    public string? ClubId { get; set; }
    public string Topic { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Category { get; set; } = null!;
    public DateOnly? FollowUpDate { get; set; }
    public string CreatedByScoutId { get; set; } = null!;
}

public class UpdateNote
{
    public string Topic { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Category { get; set; } = null!;
    public DateOnly? FollowUpDate { get; set; }
}
