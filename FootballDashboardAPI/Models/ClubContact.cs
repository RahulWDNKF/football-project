using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FootballDashboardAPI.Models;

[Table("club_contacts", Schema = "stf")]
public partial class ClubContact
{
    [Key]
    [Column("club_contact_id")]
    [StringLength(50)]
    public string ClubContactId { get; set; } = null!;

    [Column("club_id")]
    [StringLength(50)]
    public string ClubId { get; set; } = null!;

    [Column("contact_name")]
    [StringLength(150)]
    public string ContactName { get; set; } = null!;

    [Column("role_name")]
    [StringLength(100)]
    public string RoleName { get; set; } = null!;

    [Column("email")]
    [StringLength(254)]
    public string? Email { get; set; }

    [Column("phone")]
    [StringLength(50)]
    public string? Phone { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("ClubId")]
    [InverseProperty("ClubContacts")]
    public virtual Club Club { get; set; } = null!;
}


public class CreateClubContact
{
    [Required]
    public string ClubId { get; set; } = null!;

    [Required]
    public string ContactName { get; set; } = null!;

    [Required]
    public string RoleName { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }
}


public class UpdateClubContact
{
    public string? ClubId { get; set; }
    public string? ContactName { get; set; }
    public string? RoleName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}