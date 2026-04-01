# Player APIs - Clarification & Architecture

## Overview

There are **TWO different Player APIs** in the system serving different purposes:

| Property | Main Players API | Scouting Players API |
|----------|------------------|----------------------|
| **Controller** | `PlayersController` | `ScoutingPlayersController` |
| **Model** | `Player` | `Player1` |
| **Database Table** | `[dbo].[players]` | `[stf].[players]` |
| **Route** | `/api/players` | `/api/scouting-players` |
| **Implementation** | Service/Repository Pattern | EntityCrudController Pattern |
| **Status** | ? **PRIMARY - USE THIS** | ?? Legacy/Alternative |
| **Purpose** | Main player management | Scouting system players |

---

## 1. Main Players API (RECOMMENDED)

### Details
- **Controller**: `PlayersController`
- **Model**: `Player` 
- **Table**: `[dbo].[players]` (Default schema)
- **Route**: `GET /api/players`
- **Implementation Pattern**: Service ? Repository ? Stored Procedures

### Architecture
```
Request
  ?
PlayersController
  ?
IPlayerService / PlayerService
  ?
IPlayerRepository / PlayerRepository
  ?
Stored Procedures ([dbo].[sp_players_*])
  ?
Database [dbo].[players]
```

### Features
? Full CRUD operations
? Type-safe DTOs (PlayerDto, CreatePlayerDto, UpdatePlayerDto)
? Comprehensive error handling
? Business logic separation
? Proper async/await throughout
? SQL injection protection

### API Endpoints
```http
GET    /api/players              # Get all players
GET    /api/players/{id}         # Get player by ID
POST   /api/players              # Create new player
PUT    /api/players/{id}         # Update player
DELETE /api/players/{id}         # Delete player
```

### Request/Response Example

**Create Player:**
```http
POST /api/players
Content-Type: application/json

{
  "fullName": "Cristiano Ronaldo",
  "dateOfBirth": "1985-02-05",
  "nationality": "Portugal",
  "position": "Forward",
  "preferredFoot": "Left",
  "heightCm": 187,
  "weightKg": 84,
  "currentClub": "Manchester United",
  "agentName": "Jorge Mendes"
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "fullName": "Cristiano Ronaldo",
  "dateOfBirth": "1985-02-05",
  "nationality": "Portugal",
  "position": "Forward",
  "preferredFoot": "Left",
  "heightCm": 187,
  "weightKg": 84,
  "currentClub": "Manchester United",
  "agentName": "Jorge Mendes",
  "agentContact": null,
  "contractStart": null,
  "contractEnd": null,
  "contractStatus": null,
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": null
}
```

### Database Model
```csharp
[Table("players")]  // dbo schema by default
public class Player
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    
    [Column("full_name")]
    public string FullName { get; set; }
    
    [Column("date_of_birth")]
    public DateOnly? DateOfBirth { get; set; }
    
    // ... other properties
}
```

---

## 2. Scouting Players API (LEGACY)

### Details
- **Controller**: `ScoutingPlayersController` (Renamed from Players1Controller)
- **Model**: `Player1`
- **Table**: `[stf].[players]` (Scouting Football Training schema)
- **Route**: `GET /api/scouting-players`
- **Implementation Pattern**: EntityCrudController (Generic pattern)

### Architecture
```
Request
  ?
ScoutingPlayersController : EntityCrudController<Player1>
  ?
Generic CRUD Methods (Reflection-based)
  ?
Stored Procedures ([stf].[sp_players_*])
  ?
Database [stf].[players]
```

### Features
? Automatic CRUD generation
? Reflection-based parameter mapping
? Generic stored procedure calling
?? Less structured than service pattern
?? Mixes concerns (no DTO separation)

### API Endpoints
```http
GET    /api/scouting-players              # Get all scouting players
GET    /api/scouting-players/{id}         # Get scouting player by ID
POST   /api/scouting-players              # Create new scouting player
PUT    /api/scouting-players/{id}         # Update scouting player
DELETE /api/scouting-players/{id}         # Delete scouting player
```

### Request/Response Example

**Create Scouting Player:**
```http
POST /api/scouting-players
Content-Type: application/json

{
  "playerId": "scout-001",
  "fullName": "Young Prospect",
  "dateOfBirth": "2005-03-15",
  "nationality": "Spain",
  "positionCode": "FW",
  "preferredFoot": "Right",
  "heightCm": 180,
  "weightKg": 75,
  "currentClubId": "club-001",
  "contractStartDate": "2023-01-01",
  "contractEndDate": "2025-12-31",
  "agentName": "Scout Agent",
  "agentScoutId": "scout-agent-001"
}
```

### Database Model
```csharp
[Table("players", Schema = "stf")]  // stf schema
public class Player1
{
    [Key]
    [Column("player_id")]
    public string PlayerId { get; set; }
    
    [Column("full_name")]
    public string FullName { get; set; }
    
    [Column("date_of_birth")]
    public DateOnly DateOfBirth { get; set; }
    
    // ... other properties
}
```

---

## ?? KEY DIFFERENCES

### Data Types
| Field | Player (dbo) | Player1 (stf) |
|-------|----------|-----------|
| ID | long (auto-generated) | string (custom) |
| DateOfBirth | DateOnly? (optional) | DateOnly (required) |
| Position | string? "position" | string "position_code" |
| PositionCode | N/A | string |
| HeightCm | int? (optional) | int (required) |
| WeightKg | int? (optional) | int (required) |
| ContractStart | DateOnly? | DateOnly |
| ContractEnd | DateOnly? | DateOnly |

### Required Fields
**Player (dbo):**
- FullName only

**Player1 (stf):**
- PlayerId, FullName, DateOfBirth, Nationality, PositionCode, PreferredFoot, HeightCm, WeightKg, ContractStartDate, ContractEndDate, AgentName, AgentScoutId

---

## ?? WHICH ONE TO USE?

### Use PlayersController if:
? Managing general player information
? Working with optional fields
? Need type-safe DTOs
? Want service-based architecture
? Building main player management system
? **THIS IS THE RECOMMENDED CHOICE**

### Use ScoutingPlayersController if:
? Managing scouting system players
? All fields are mandatory
? Need simple CRUD without services
? Working in the [stf] schema
?? Only use if explicitly required by business logic

---

## ?? SUMMARY

**PRIMARY API**: `PlayersController` ? `/api/players` ? `[dbo].[players]`
```bash
curl GET https://localhost:5001/api/players
```

**SECONDARY API**: `ScoutingPlayersController` ? `/api/scouting-players` ? `[stf].[players]`
```bash
curl GET https://localhost:5001/api/scouting-players
```

---

## ?? Files Involved

### Main Players API
- ? `Models/Player.cs` - Entity model
- ? `DTOs/PlayerDto.cs` - Data transfer objects
- ? `Repositories/IPlayerRepository.cs` - Repository interface
- ? `Repositories/PlayerRepository.cs` - Repository implementation
- ? `Services/IPlayerService.cs` - Service interface
- ? `Services/PlayerService.cs` - Service implementation
- ? `Controllers/PlayersController.cs` - **PRIMARY CONTROLLER**
- ? `Database/StoredProcedures/dbo/sp_players_*.sql` - Stored procedures

### Scouting Players API
- ? `Models/Player1.cs` - Entity model
- ?? No DTOs (returns entity directly)
- ?? No Repository/Service layer
- ? `Controllers/Players1Controller.cs` (Renamed to ScoutingPlayersController)
- ? `Database/StoredProcedures/stf/sp_players_*.sql` - Stored procedures

---

## ? RECOMMENDATION

**Always use `/api/players` (PlayersController) for primary player management.**

The `ScoutingPlayersController` exists for legacy/alternative use cases but should not be the primary endpoint for player data management.

---

## ?? Related Files

- `API_IMPLEMENTATION_GUIDE.md` - Complete architecture guide
- `IMPLEMENTATION_SUMMARY.md` - Implementation details
- `EntityCrudController.cs` - Generic CRUD controller
- `Program.cs` - Dependency injection setup
