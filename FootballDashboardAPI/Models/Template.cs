using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FootballDashboardAPI.Models;

[Table("templates", Schema = "stf")]
public partial class Template
{
    [Key]
    [Column("template_id")]
    [StringLength(50)]
    public string TemplateId { get; set; } = null!;

    [Column("template_name")]
    [StringLength(150)]
    public string TemplateName { get; set; } = null!;

    [Column("template_type")]
    [StringLength(20)]
    public string TemplateType { get; set; } = null!;

    [Column("subject")]
    [StringLength(300)]
    public string? Subject { get; set; }

    [Column("body")]
    public string Body { get; set; } = null!;

    [Column("created_at")]
    [Precision(0)]
    public DateTime CreatedAt { get; set; }
}

public class CreateTemplate
{
    public string TemplateName { get; set; } = null!;
    public string TemplateType { get; set; } = null!;
    public string? Subject { get; set; }
    public string Body { get; set; } = null!;
}

public class UpdateTemplate
{
    public string TemplateName { get; set; } = null!;
    public string TemplateType { get; set; } = null!;
    public string? Subject { get; set; }
    public string Body { get; set; } = null!;
}
