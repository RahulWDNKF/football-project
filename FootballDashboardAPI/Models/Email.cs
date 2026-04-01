using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace FootballDashboardAPI.Models;

[Table("emails", Schema = "stf")]
[Index("SentAt", Name = "IX_emails_sent_at", AllDescending = true)]
public partial class Email
{
    [Key]
    [Column("email_id")]
    [StringLength(50)]
    public string EmailId { get; set; } = null!;

    [Column("player_id")]
    [StringLength(50)]
    public string? PlayerId { get; set; }

    [Column("club_id")]
    [StringLength(50)]
    public string? ClubId { get; set; }

    [Column("recipient_email")]
    [StringLength(254)]
    public string RecipientEmail { get; set; } = null!;

    [Column("subject")]
    [StringLength(300)]
    public string Subject { get; set; } = null!;

    [Column("body")]
    public string Body { get; set; } = null!;

    [Column("sent_by_scout_id")]
    [StringLength(50)]
    public string SentByScoutId { get; set; } = null!;

    [Column("sent_at")]
    [Precision(0)]
    public DateTime SentAt { get; set; }

    [ForeignKey("ClubId")]
    [InverseProperty("Emails")]
    [JsonIgnore]
    public virtual Club? Club { get; set; }

    [ForeignKey("PlayerId")]
    [InverseProperty("Emails")]
    public virtual Player1? Player { get; set; }

    [ForeignKey("SentByScoutId")]
    [InverseProperty("Emails")]
    public virtual Scout SentByScout { get; set; } = null!;
}
