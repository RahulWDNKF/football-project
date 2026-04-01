using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FootballDashboardAPI.Models;

[Table("players")]
public class Player
{
    public long Id { get; set; }
    public string FullName { get; set; } = null!;
    public DateOnly? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string? Position { get; set; }
    public string? PreferredFoot { get; set; }
    public int? HeightCm { get; set; }
    public int? WeightKg { get; set; }
    public string? CurrentClub { get; set; }
    public DateOnly? ContractStart { get; set; }
    public DateOnly? ContractEnd { get; set; }
    public string? ContractStatus { get; set; }
    public string? AgentName { get; set; }
    public string? contact_info { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? agent_scout_id { get; set; }

    public string? profileImage { get; set; }

    [JsonPropertyName("player_email")]
    public string? PlayerEmail { get; set; }
}

public class CreatePlayer
{
    public string FullName { get; set; } = null!;
    public DateOnly? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string? Position { get; set; }
    public string? PreferredFoot { get; set; }
    public int? HeightCm { get; set; }
    public int? WeightKg { get; set; }
    public string? CurrentClub { get; set; }
    public DateOnly? ContractStart { get; set; }
    public DateOnly? ContractEnd { get; set; }
    public string? ContractStatus { get; set; }
    public string? AgentName { get; set; }
    public string? AgentContact { get; set; }

    [JsonPropertyName("agent_scout_id")] // FIX: maps snake_case from frontend
    public string? AgentScoutId { get; set; }

    [JsonPropertyName("contact_info")]   // FIX: also add this since frontend sends contact_info
    public string? ContactInfo { get; set; }

    [JsonPropertyName("profileImage")] 
    public string? ProfileImage { get; set; }

    [JsonPropertyName("player_email")]
    public string? PlayerEmail { get; set; }
}

public class UpdatePlayer
{
    public string FullName { get; set; } = null!;
    public DateOnly? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string? Position { get; set; }
    public string? PreferredFoot { get; set; }
    public int? HeightCm { get; set; }
    public int? WeightKg { get; set; }
    public string? CurrentClub { get; set; }
    public DateOnly? ContractStart { get; set; }
    public DateOnly? ContractEnd { get; set; }
    public string? ContractStatus { get; set; }
    public string? AgentName { get; set; }
    public string? AgentContact { get; set; }

    [JsonPropertyName("agent_scout_id")]
    public string? AgentScoutId { get; set; }

    [JsonPropertyName("contact_info")]
    public string? ContactInfo { get; set; }

    [JsonPropertyName("profileImage")]  
    public string? ProfileImage { get; set; }
}

