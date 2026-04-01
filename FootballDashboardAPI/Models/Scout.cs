using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FootballDashboardAPI.Models;

[Table("scouts", Schema = "stf")]
public partial class Scout
{
    [Key]
    [Column("scout_id")]
    [StringLength(50)]
    public string ScoutId { get; set; } = null!;

    [Column("scout_name")]
    [StringLength(150)]
    public string ScoutName { get; set; } = null!;

    [Column("role_name")]
    [StringLength(100)]
    public string RoleName { get; set; } = null!;

    [Column("first_name")]
    [StringLength(100)]
    public string? FirstName { get; set; }

    [Column("last_name")]
    [StringLength(100)]
    public string? LastName { get; set; }

    [Column("email")]
    [StringLength(255)]
    [EmailAddress]
    public string? Email { get; set; }

    [Column("phone_number")]
    [StringLength(20)]
    [Phone]
    public string? PhoneNumber { get; set; }

    [Column("address_line1")]
    [StringLength(255)]
    public string? AddressLine1 { get; set; }

    [Column("address_line2")]
    [StringLength(255)]
    public string? AddressLine2 { get; set; }

    [Column("city")]
    [StringLength(100)]
    public string? City { get; set; }

    [Column("state")]
    [StringLength(100)]
    public string? State { get; set; }

    [Column("postal_code")]
    [StringLength(20)]
    public string? PostalCode { get; set; }

    [Column("country")]
    [StringLength(100)]
    public string? Country { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    [InverseProperty("SentByScout")]
    public virtual ICollection<Email> Emails { get; set; } = new List<Email>();

    [InverseProperty("CreatedByScout")]
    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

    [InverseProperty("AgentScout")]
    public virtual ICollection<Player1> Player1s { get; set; } = new List<Player1>();

    [InverseProperty("Scout")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty("AssignedToScout")]
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}

public class CreateScout
{
    public string ScoutName { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
}

public class UpdateScout
{
    public string ScoutName { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
}
