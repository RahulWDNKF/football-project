# Football Dashboard API - Comprehensive API Implementation Guide

## Overview

This document describes the complete implementation of a comprehensive REST API for the Football Dashboard application following the same architecture pattern as the Club model. The implementation includes full CRUD operations for all major models using stored procedures for best performance and maintainability.

## Architecture Pattern

The API follows a **Service-Repository Pattern** with the following layers:

### 1. **Models Layer** (`Models/`)
- Entity models that map to database tables
- Configured with Entity Framework Core data annotations
- Includes relationships and database constraints

### 2. **DTOs Layer** (`DTOs/`)
- Data Transfer Objects for API requests/responses
- Separates internal model structure from API contracts
- Includes separate Create and Update DTOs for flexibility

### 3. **Repository Layer** (`Repositories/`)
- Interfaces (`I*Repository.cs`) - Contract definitions
- Implementations (`*Repository.cs`) - Data access logic
- Uses SQL stored procedures for all operations
- Implements proper parameter handling with SqlParameter

### 4. **Service Layer** (`Services/`)
- Interfaces (`I*Service.cs`) - Service contracts
- Implementations (`*Service.cs`) - Business logic
- DTO mapping and validation
- Orchestrates repository calls

### 5. **Controller Layer** (`Controllers/`)
- RESTful endpoints for each resource
- Proper HTTP method mapping (GET, POST, PUT, DELETE)
- Comprehensive error handling and status codes
- Request/response serialization via DTOs

## Implemented Models

### 1. **Club** (Already Existed)
- **Repository**: `IClubRepository`, `ClubRepository`
- **Service**: `IClubService`, `ClubService`
- **Controller**: `ClubsController`
- **DTOs**: `ClubDto`, `CreateClubDto`, `UpdateClubDto`
- **Stored Procedures**:
  - `sp_clubs_get_all`
  - `sp_clubs_get_by_id`
  - `sp_clubs_insert`
  - `sp_clubs_update`
  - `sp_clubs_delete`
  - `sp_clubs_exists`
  - `sp_clubs_name_exists`

### 2. **Player**
- **Repository**: `IPlayerRepository`, `PlayerRepository`
- **Service**: `IPlayerService`, `PlayerService`
- **Controller**: `PlayersController` (Updated)
- **DTOs**: `PlayerDto`, `CreatePlayerDto`, `UpdatePlayerDto`
- **Stored Procedures**:
  - `sp_players_get_all`
  - `sp_players_get_by_id`
  - `sp_players_insert`
  - `sp_players_update`
  - `sp_players_delete`
  - `sp_players_exists`

### 3. **Scout**
- **Repository**: `IScoutRepository`, `ScoutRepository`
- **Service**: `IScoutService`, `ScoutService`
- **Controller**: `ScoutsController` (Updated)
- **DTOs**: `ScoutDto`, `CreateScoutDto`, `UpdateScoutDto`
- **Stored Procedures**:
  - `sp_scouts_get_all`
  - `sp_scouts_get_by_id`
  - `sp_scouts_insert`
  - `sp_scouts_update`
  - `sp_scouts_delete`
  - `sp_scouts_exists`
  - `sp_scouts_name_exists`

### 4. **User**
- **Repository**: `IUserRepository`, `UserRepository`
- **Service**: `IUserService`, `UserService`
- **Controller**: `UsersController` (Updated)
- **DTOs**: `UserDto`, `CreateUserDto`, `UpdateUserDto`
- **Stored Procedures**:
  - `sp_users_get_all`
  - `sp_users_get_by_id`
  - `sp_users_insert`
  - `sp_users_update`
  - `sp_users_delete`
  - `sp_users_exists`
  - `sp_users_email_exists`

### 5. **Template**
- **Repository**: `ITemplateRepository`, `TemplateRepository`
- **Service**: `ITemplateService`, `TemplateService`
- **Controller**: `TemplatesController` (Updated)
- **DTOs**: `TemplateDto`, `CreateTemplateDto`, `UpdateTemplateDto`
- **Stored Procedures**:
  - `sp_templates_get_all`
  - `sp_templates_get_by_id`
  - `sp_templates_insert`
  - `sp_templates_update`
  - `sp_templates_delete`
  - `sp_templates_exists`
  - `sp_templates_name_exists`

### 6. **Note**
- **Repository**: `INoteRepository`, `NoteRepository`
- **Service**: `INoteService`, `NoteService`
- **Controller**: `NotesController` (Updated)
- **DTOs**: `NoteDto`, `CreateNoteDto`, `UpdateNoteDto`
- **Stored Procedures**:
  - `sp_notes_get_all`
  - `sp_notes_get_by_id`
  - `sp_notes_insert`
  - `sp_notes_update`
  - `sp_notes_delete`
  - `sp_notes_exists`
  - `sp_notes_get_by_club_id`
  - `sp_notes_get_by_player_id`
- **Additional Endpoints**:
  - `GET /api/notes/club/{clubId}` - Get notes by club
  - `GET /api/notes/player/{playerId}` - Get notes by player

### 7. **Review**
- **Repository**: `IReviewRepository`, `ReviewRepository`
- **Service**: `IReviewService`, `ReviewService`
- **Controller**: `ReviewsController` (Updated)
- **DTOs**: `ReviewDto`, `CreateReviewDto`, `UpdateReviewDto`
- **Stored Procedures**:
  - `sp_reviews_get_all`
  - `sp_reviews_get_by_id`
  - `sp_reviews_insert`
  - `sp_reviews_update`
  - `sp_reviews_delete`
  - `sp_reviews_exists`
  - `sp_reviews_get_by_player_id`
  - `sp_reviews_get_by_scout_id`
- **Additional Endpoints**:
  - `GET /api/reviews/player/{playerId}` - Get reviews by player
  - `GET /api/reviews/scout/{scoutId}` - Get reviews by scout

## API Endpoints

### Standard CRUD Endpoints Pattern

Each resource follows this standard pattern:

```
GET    /api/{resource}              - Get all records
GET    /api/{resource}/{id}         - Get by ID
POST   /api/{resource}              - Create new record
PUT    /api/{resource}/{id}         - Update record
DELETE /api/{resource}/{id}         - Delete record
```

### Example: Players API

```bash
# Get all players
GET /api/players

# Get specific player
GET /api/players/1

# Create new player
POST /api/players
{
  "fullName": "John Doe",
  "dateOfBirth": "2000-01-15",
  "nationality": "Spain",
  "position": "Midfielder",
  "preferredFoot": "Left",
  "heightCm": 182,
  "weightKg": 75,
  "currentClub": "FC Barcelona",
  "agentName": "Agent Smith"
}

# Update player
PUT /api/players/1
{
  "fullName": "John Doe",
  "dateOfBirth": "2000-01-15",
  "nationality": "Spain",
  "position": "Forward"
}

# Delete player
DELETE /api/players/1
```

## Best Practices Implemented

### 1. **Stored Procedure Usage**
- All database operations use stored procedures
- Parameters passed via `SqlParameter` for SQL injection prevention
- Proper null handling with `DBNull.Value`
- Connection and command management with `await using` pattern

### 2. **Async/Await Pattern**
- All repository and service methods are async
- No blocking calls in data access layer
- Efficient resource utilization with Task-based API

### 3. **Entity Framework Integration**
- `FromSqlRaw()` for stored procedures without parameters
- `FromSqlInterpolated()` for parameterized queries
- `AsNoTracking()` for read operations (performance optimization)
- Proper connection management

### 4. **Error Handling**
- Duplicate validation with specific exceptions
- Null checks at service level
- Proper HTTP status codes (200, 201, 204, 400, 404, 409)
- Error messages in response bodies

### 5. **Data Validation**
- Duplicate name/email checking in services
- Unique constraint validation before insert/update
- Proper DTO separation for create/update operations

### 6. **Dependency Injection**
- All services and repositories registered in `Program.cs`
- Constructor injection in controllers
- Scoped lifetime for repositories and services
- Automatic disposal of resources

### 7. **Database Parameter Handling**
```csharp
// Proper null handling
new SqlParameter("@optional_field", (object?)value ?? DBNull.Value),

// Type safety
new SqlParameter("@id", id),
new SqlParameter("@name", entity.Name),
new SqlParameter("@email", entity.Email),
new SqlParameter("@status", entity.Status ?? true),
```

### 8. **NoTracking for Performance**
```csharp
// Read operations don't need tracking
.FromSqlRaw("EXEC [schema].[procedure]")
.AsNoTracking()
.ToListAsync();
```

### 9. **Consistent Response Pattern**
```csharp
// Success responses
return Ok(dto);                    // 200 OK
return CreatedAtAction(...);       // 201 Created
return NoContent();                // 204 No Content

// Error responses
return NotFound(new { message = "..." });    // 404
return Conflict(new { message = "..." });    // 409
```

### 10. **DTO Mapping**
- Clean separation of concerns
- Private mapping methods
- Type-safe DTO conversion
- Reduced API surface area

## Performance Optimizations

1. **Stored Procedures**: Execute on SQL Server for better performance
2. **Connection Pooling**: Automatic connection management
3. **AsNoTracking()**: Eliminates EF Core tracking overhead for read operations
4. **Parameter Binding**: Prevents query parsing on each execution
5. **Async Operations**: Non-blocking I/O operations
6. **Proper Indexing**: Support for database indexes on frequently queried columns

## Dependency Injection Configuration

```csharp
// Repositories
builder.Services.AddScoped<IClubRepository, ClubRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IScoutRepository, ScoutRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

// Services
builder.Services.AddScoped<IClubService, ClubService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IScoutService, ScoutService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
```

## Database Requirements

Each model requires the following stored procedures in your SQL Server database:

### Naming Convention
- `sp_{entity}_get_all` - Get all records
- `sp_{entity}_get_by_id` - Get specific record
- `sp_{entity}_insert` - Insert new record
- `sp_{entity}_update` - Update record
- `sp_{entity}_delete` - Delete record
- `sp_{entity}_exists` - Check existence
- `sp_{entity}_{field}_exists` - Check field uniqueness (optional)

### Example Stored Procedure Structure

```sql
CREATE PROCEDURE [schema].[sp_players_get_all]
AS
BEGIN
    SELECT * FROM [dbo].[players]
END

CREATE PROCEDURE [schema].[sp_players_get_by_id]
    @Id BIGINT
AS
BEGIN
    SELECT * FROM [dbo].[players] WHERE [id] = @Id
END

CREATE PROCEDURE [schema].[sp_players_insert]
    @full_name NVARCHAR(200),
    @nationality NVARCHAR(100),
    @position NVARCHAR(50),
    @created_at DATETIME
AS
BEGIN
    INSERT INTO [dbo].[players] ([full_name], [nationality], [position], [created_at])
    VALUES (@full_name, @nationality, @position, @created_at)
    SELECT SCOPE_IDENTITY()
END
```

## Testing the API

### Using Swagger
1. Navigate to `https://localhost:5001/swagger`
2. Expand any endpoint to see request/response schemas
3. Try executing requests directly from Swagger UI

### Using cURL
```bash
# Get all players
curl -X GET "https://localhost:5001/api/players"

# Create player
curl -X POST "https://localhost:5001/api/players" \
  -H "Content-Type: application/json" \
  -d '{"fullName":"John Doe","nationality":"Spain"}'
```

### Using Postman
1. Import the API endpoints
2. Set up environment variables for base URL
3. Create collections for each resource

## Future Enhancements

1. **Pagination**: Implement limit/offset pagination
2. **Filtering**: Add dynamic filtering capabilities
3. **Sorting**: Support sorting on list endpoints
4. **Caching**: Implement Redis caching for frequently accessed data
5. **Authentication**: Add JWT authentication
6. **Authorization**: Add role-based access control
7. **Logging**: Implement structured logging
8. **API Versioning**: Support multiple API versions
9. **Rate Limiting**: Implement rate limiting
10. **Soft Delete**: Add soft delete support instead of hard delete

## Troubleshooting

### Build Errors
- Ensure all namespaces are correct
- Verify stored procedure names match implementation
- Check database connection string in appsettings.json

### Runtime Errors
- Verify stored procedures exist in database
- Check parameter names match procedure definitions
- Ensure proper null handling with DBNull.Value

### Performance Issues
- Add database indexes on frequently queried columns
- Use AsNoTracking() for read operations
- Monitor stored procedure execution plans
- Consider pagination for large result sets
