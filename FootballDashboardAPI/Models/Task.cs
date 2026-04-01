using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FootballDashboardAPI.Models;

[Table("tasks", Schema = "stf")]
[Index("AssignedToScoutId", "Status", "DueDate", Name = "IX_tasks_assigned_to")]
[Index("DueDate", "Status", Name = "IX_tasks_due_status")]
public partial class Task
{
    [Key]
    [Column("task_id")]
    [StringLength(50)]
    public string TaskId { get; set; } = null!;

    [Column("title")]
    [StringLength(200)]
    public string Title { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("player_id")]
    [StringLength(50)]
    public string? PlayerId { get; set; }

    [Column("club_id")]
    [StringLength(50)]
    public string? ClubId { get; set; }

    [Column("assigned_to_scout_id")]
    [StringLength(50)]
    public string AssignedToScoutId { get; set; } = null!;

    [Column("due_date")]
    public DateOnly DueDate { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = null!;

    [Column("source")]
    [StringLength(20)]
    public string Source { get; set; } = null!;

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("AssignedToScoutId")]
    [InverseProperty("Tasks")]
    public virtual Scout AssignedToScout { get; set; } = null!;

    [ForeignKey("ClubId")]
    [InverseProperty("Tasks")]
    public virtual Club? Club { get; set; }

    [ForeignKey("PlayerId")]
    [InverseProperty("Tasks")]
    public virtual Player1? Player { get; set; }
}

public class CreateTask
{
    //[Required]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? PlayerId { get; set; }

    public string? ClubId { get; set; }

    //[Required]
    public string AssignedToScoutId { get; set; } = null!;

    //[Required]
    public DateOnly DueDate { get; set; }

    //[Required]
    public string Status { get; set; } = null!;

    //[Required]
    public string Source { get; set; } = null!;
}

public class UpdateTask
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? PlayerId { get; set; }

    public string? ClubId { get; set; }

    public string? AssignedToScoutId { get; set; }

    public DateOnly? DueDate { get; set; }

    public string? Status { get; set; }

    public string? Source { get; set; }
}