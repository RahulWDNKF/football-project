using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FootballDashboardAPI.Models;

[Table("players", Schema = "stf")]
[Index("CurrentClubId", Name = "IX_players_current_club_id")]
public partial class Player1
{
    [Key]
    [Column("player_id")]
    [StringLength(50)]
    public string PlayerId { get; set; } = null!;

    [Column("full_name")]
    [StringLength(150)]
    public string FullName { get; set; } = null!;

    [Column("date_of_birth")]
    public DateOnly DateOfBirth { get; set; }

    [Column("nationality")]
    [StringLength(100)]
    public string Nationality { get; set; } = null!;

    [Column("position_code")]
    [StringLength(10)]
    public string PositionCode { get; set; } = null!;

    [Column("preferred_foot")]
    [StringLength(10)]
    public string PreferredFoot { get; set; } = null!;

    [Column("height_cm")]
    public int HeightCm { get; set; }

    [Column("weight_kg")]
    public int WeightKg { get; set; }

    [Column("current_club_id")]
    [StringLength(50)]
    public string? CurrentClubId { get; set; }

    [Column("contract_start_date")]
    public DateOnly ContractStartDate { get; set; }

    [Column("contract_end_date")]
    public DateOnly ContractEndDate { get; set; }

    [Column("agent_name")]
    [StringLength(150)]
    public string AgentName { get; set; } = null!;

    [Column("player_email")]
    [StringLength(255)]
    public string playerEmail { get; set; } = null!;

    [Column("agent_scout_id")]
    [StringLength(50)]
    public string AgentScoutId { get; set; } = null!;

    [Column("contact_info")]
    [StringLength(255)]
    public string? ContactInfo { get; set; }

    [Column("profile_image_url")]
    [StringLength(500)]
    public string? ProfileImageUrl { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    [Precision(0)]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("AgentScoutId")]
    [InverseProperty("Player1s")]
    public virtual Scout AgentScout { get; set; } = null!;

    [ForeignKey("CurrentClubId")]
    [InverseProperty("Player1s")]
    public virtual Club? CurrentClub { get; set; }

    [InverseProperty("Player")]
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    [InverseProperty("Player")]
    public virtual ICollection<Email> Emails { get; set; } = new List<Email>();

    [InverseProperty("Player")]
    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

    [InverseProperty("Player")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty("Player")]
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
