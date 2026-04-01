# ?? Player APIs - Quick Reference

## PRIMARY API ? (USE THIS)

**Endpoint:** `GET /api/players`
**Controller:** `PlayersController`
**Model:** `Player` ([dbo].[players])

```bash
# Get all players
curl GET https://localhost:5001/api/players

# Get player by ID
curl GET https://localhost:5001/api/players/1

# Create player
curl POST https://localhost:5001/api/players \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "John Doe",
    "dateOfBirth": "2000-01-15",
    "nationality": "Portugal",
    "position": "Forward",
    "preferredFoot": "Left",
    "heightCm": 187,
    "weightKg": 84,
    "currentClub": "FC Porto",
    "agentName": "Agent Name"
  }'

# Update player
curl PUT https://localhost:5001/api/players/1 \
  -H "Content-Type: application/json" \
  -d '{...}'

# Delete player
curl DELETE https://localhost:5001/api/players/1
```

**Architecture:**
```
Controller ? Service ? Repository ? Stored Procedures
```

**Features:**
- ? Type-safe DTOs
- ? Service layer for business logic
- ? Proper error handling
- ? SQL injection protection

---

## SECONDARY API ?? (LEGACY)

**Endpoint:** `GET /api/scouting-players`
**Controller:** `ScoutingPlayersController` (renamed from Players1Controller)
**Model:** `Player1` ([stf].[players])

**Only use if you need scouting system players!**

```bash
curl GET https://localhost:5001/api/scouting-players
```

---

## Key Differences

| Field | Players API | Scouting API |
|-------|------------|--------------|
| Route | `/api/players` | `/api/scouting-players` |
| Model | `Player` | `Player1` |
| Schema | `[dbo]` | `[stf]` |
| DateOfBirth | Optional | Required |
| HeightCm | Optional | Required |
| WeightKg | Optional | Required |

---

## Recommendation

**Use `/api/players` for all player management.**

---

?? See: `PLAYER_APIS_CLARIFICATION.md`
