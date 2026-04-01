using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FootballDashboardAPI.Models;

[Table("clubs", Schema = "stf")]
[Index("ClubName", Name = "UQ_clubs_club_name", IsUnique = true)]
public partial class Club
{
    [Key]
    [Column("club_id")]
    [StringLength(50)]
    public string ClubId { get; set; } = null!;

    [Column("club_name")]
    [StringLength(150)]
    public string ClubName { get; set; } = null!;

    [Column("country")]
    [StringLength(100)]
    public string Country { get; set; } = null!;

    [Column("address_line")]
    [StringLength(300)]
    public string? AddressLine { get; set; }

    [Column("logo_url")]
    [StringLength(500)]
    public string? LogoUrl { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [InverseProperty("Club")]
    public virtual ICollection<ClubContact> ClubContacts { get; set; } = new List<ClubContact>();

    [InverseProperty("Club")]
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    [InverseProperty("Club")]
    public virtual ICollection<Email> Emails { get; set; } = new List<Email>();

    [InverseProperty("Club")]
    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

    [InverseProperty("CurrentClub")]
    public virtual ICollection<Player1> Player1s { get; set; } = new List<Player1>();

    [InverseProperty("Club1")]
    public virtual ICollection<Review> ReviewClub1s { get; set; } = new List<Review>();

    [InverseProperty("Club2")]
    public virtual ICollection<Review> ReviewClub2s { get; set; } = new List<Review>();

    [InverseProperty("Club")]
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
//namespace FootballDashboardAPI.DTOs;


public class CreateClub
{
    public string ClubName { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string? AddressLine { get; set; }
    public string? LogoUrl { get; set; }
}

public class UpdateClub
{
    public string ClubName { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string? AddressLine { get; set; }
    public string? LogoUrl { get; set; }
}