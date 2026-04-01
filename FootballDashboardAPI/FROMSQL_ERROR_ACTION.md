# ?? FROMSQL ERROR FIX - ACTION REQUIRED

## The Error You Got

```
GET https://localhost:7001/api/clubs/c1

Error: InvalidOperationException: 'FromSql' or 'SqlQuery' was called with non-composable SQL 
and with a query composing over it.
```

## What Was Fixed

Fixed 7 repositories where `.FromSqlInterpolated()` was being composed with `.FirstOrDefaultAsync()`:

| Repository | Method | Status |
|------------|--------|--------|
| ClubRepository | GetByIdAsync() | ? FIXED |
| PlayerRepository | GetByIdAsync() | ? FIXED |
| ScoutRepository | GetByIdAsync() | ? FIXED |
| UserRepository | GetByIdAsync(), CreateAsync() | ? FIXED |
| TemplateRepository | GetByIdAsync() | ? FIXED |
| NoteRepository | GetByIdAsync() | ? FIXED |
| ReviewRepository | GetByIdAsync() | ? FIXED |

## The Fix

Changed from:
```csharp
// ? WRONG
.FromSqlInterpolated($"EXEC [stf].[sp_clubs_get_by_id] @Id={id}")
.AsNoTracking()
.FirstOrDefaultAsync()
```

Changed to:
```csharp
// ? CORRECT
var result = await _context.Clubs
    .FromSqlInterpolated($"EXEC [stf].[sp_clubs_get_by_id] @Id={id}")
    .AsNoTracking()
    .ToListAsync();

return result.FirstOrDefault();
```

## Next Steps

1. **Build the solution:**
   ```bash
   dotnet clean
   dotnet build
   ```

2. **Run the application:**
   ```bash
   dotnet run
   ```

3. **Test the fixed endpoint:**
   ```bash
   curl GET https://localhost:7001/api/clubs/c1
   ```

## Expected Result

? No more error
? Data returns correctly
? All GET by ID endpoints working

---

?? See: `FROMSQL_COMPOSITION_FIX.md` for detailed information
