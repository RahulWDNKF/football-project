using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FootballDashboardAPI.Models;

[Table("documents", Schema = "stf")]
public partial class Document
{
    [Key]
    [Column("document_id")]
    [StringLength(50)]
    public string DocumentId { get; set; } = null!;

    [Column("player_id")]
    [StringLength(50)]
    public string? PlayerId { get; set; }

    [Column("club_id")]
    [StringLength(50)]
    public string? ClubId { get; set; }

    [Column("document_name")]
    [StringLength(255)]
    public string DocumentName { get; set; } = null!;

    [Column("document_type")]
    [StringLength(50)]
    public string DocumentType { get; set; } = null!;

    [Column("document_date")]
    [Precision(0)]
    public DateTime DocumentDate { get; set; }

    [Column("file_size_label")]
    [StringLength(50)]
    public string? FileSizeLabel { get; set; }

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("ClubId")]
    [InverseProperty("Documents")]
    public virtual Club? Club { get; set; }

    [ForeignKey("PlayerId")]
    [InverseProperty("Documents")]
    public virtual Player1? Player { get; set; }

    [Column("file_data")]
    public byte[] FileData { get; set; } = null!;
}

public class CreateDocument
{
    public string? PlayerId { get; set; }
    public string? ClubId { get; set; }

    public string DocumentName { get; set; } = null!;

    public string DocumentType { get; set; } = null!;

    public DateTime DocumentDate { get; set; }

    public string? FileSizeLabel { get; set; }

    public byte[] FileData { get; set; } = null!;
}


public class UpdateDocument
{
    public string? PlayerId { get; set; }
    public string? ClubId { get; set; }

    public string? DocumentName { get; set; }
    public string? DocumentType { get; set; }

    public DateTime? DocumentDate { get; set; }

    public string? FileSizeLabel { get; set; }

    public byte[]? FileData { get; set; }
}