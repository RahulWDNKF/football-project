# ?? CLUB CREATE 409 ERROR - COMPREHENSIVE TROUBLESHOOTING

## The Error You're Getting

```
Status Code: 409 Conflict
Message: "The ConnectionString property has not been initialized."
```

## Root Cause Analysis

This error typically occurs when:

1. **Database Connection Issues**
   - Connection string is not properly initialized
   - Database server is not running or unreachable
   - Database doesn't exist
   - User doesn't have permissions

2. **Entity Framework Context Issues**
   - DbContext is not properly registered in DI container
   - Connection string is null or empty
   - Database migrations not applied

3. **Stored Procedure Issues**
   - Stored procedure doesn't exist
   - Stored procedure has different parameter names
   - Stored procedure failed to execute

---

## Step-by-Step Troubleshooting

### Step 1: Verify Database Connection

**Check your connection string in `appsettings.json`:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TORUS-S4\\SQLEXPRESS;Database=Football;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Verify:**
- ? Server name is correct: `TORUS-S4\SQLEXPRESS`
- ? Database name is correct: `Football`
- ? Authentication is correct: `Trusted_Connection=True`

### Step 2: Verify SQL Server is Running

**On your machine:**

```bash
# Check if SQL Server service is running
Get-Service | Where-Object {$_.Name -like "*SQL*"}

# Should show MSSQL$SQLEXPRESS or similar as Running
```

If not running, start it from SQL Server Configuration Manager.

### Step 3: Verify Database Exists

**In SQL Server Management Studio (SSMS):**

```sql
-- Check if Football database exists
SELECT name FROM sys.databases WHERE name = 'Football'

-- Should return: Football
```

If not, create it:

```sql
CREATE DATABASE Football;
```

### Step 4: Verify Stored Procedure Exists

**In SSMS:**

```sql
-- Check if stored procedure exists
SELECT * FROM sys.procedures 
WHERE name = 'sp_clubs_insert' AND schema_id = SCHEMA_ID('stf')

-- Should return one row
```

### Step 5: Create the [stf] Schema if Missing

**In SSMS:**

```sql
-- Check if schema exists
SELECT * FROM sys.schemas WHERE name = 'stf'

-- If not, create it:
CREATE SCHEMA [stf];
```

### Step 6: Create the clubs Table

**In SSMS:**

```sql
-- Create the clubs table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'clubs' AND schema_id = SCHEMA_ID('stf'))
BEGIN
    CREATE TABLE [stf].[clubs]
    (
        [club_id] NVARCHAR(50) PRIMARY KEY,
        [club_name] NVARCHAR(150) NOT NULL,
        [country] NVARCHAR(100) NOT NULL,
        [address_line] NVARCHAR(300) NULL,
        [logo_url] NVARCHAR(500) NULL,
        [created_at] DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
```

### Step 7: Execute All Stored Procedures

**In SSMS, execute this file:**

```
Database/StoredProcedures/clubs.sql
```

This will create all club-related stored procedures.

### Step 8: Restart the Application

```bash
# Stop current run (Ctrl+C)
# Then restart:
dotnet clean
dotnet build
dotnet run
```

---

## Testing the Fix

### Test 1: Simple Health Check

```bash
# Get all clubs (should work)
curl GET https://localhost:7001/api/clubs
```

Expected response: `200 OK` with empty array `[]`

### Test 2: Create a Club

```bash
# Create a new club
curl -X POST https://localhost:7001/api/clubs \
  -H "Content-Type: application/json" \
  -d '{
    "clubName": "FC Barcelona",
    "country": "Spain",
    "addressLine": "Camp Nou"
  }'
```

Expected response: `201 Created` with the new club object

### Test 3: Get Specific Club

```bash
# Get the created club (replace {club-id} with actual ID)
curl GET https://localhost:7001/api/clubs/{club-id}
```

Expected response: `200 OK` with club details

---

## Common Errors and Solutions

### Error 1: "Connection string 'DefaultConnection' not found"
**Solution:**
- Verify `appsettings.json` has the connection string
- Check file name spelling (case-sensitive in Linux)

### Error 2: "Cannot open database 'Football'"
**Solution:**
```sql
CREATE DATABASE Football;
```

### Error 3: "Schema 'stf' does not exist"
**Solution:**
```sql
CREATE SCHEMA [stf];
```

### Error 4: "Procedure 'sp_clubs_insert' not found"
**Solution:**
- Execute the stored procedure creation script from `Database/StoredProcedures/clubs.sql`

### Error 5: "Duplicate key value violates unique constraint"
**Solution:**
- The club name already exists
- Use a different club name or delete the duplicate record

---

## Complete Database Setup Script

Run this in SSMS to set up everything from scratch:

```sql
-- Create database
CREATE DATABASE Football;
GO

USE Football;
GO

-- Create schema
CREATE SCHEMA [stf];
GO

-- Create clubs table
CREATE TABLE [stf].[clubs]
(
    [club_id] NVARCHAR(50) PRIMARY KEY,
    [club_name] NVARCHAR(150) NOT NULL UNIQUE,
    [country] NVARCHAR(100) NOT NULL,
    [address_line] NVARCHAR(300) NULL,
    [logo_url] NVARCHAR(500) NULL,
    [created_at] DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME()
);

-- Create sp_clubs_insert procedure
CREATE PROCEDURE [stf].[sp_clubs_insert]
    @club_id NVARCHAR(50),
    @club_name NVARCHAR(150),
    @country NVARCHAR(100),
    @address_line NVARCHAR(300) = NULL,
    @logo_url NVARCHAR(500) = NULL,
    @created_at DATETIME2(0)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [stf].[clubs]
    (
        [club_id],
        [club_name],
        [country],
        [address_line],
        [logo_url],
        [created_at]
    )
    VALUES
    (
        @club_id,
        @club_name,
        @country,
        @address_line,
        @logo_url,
        @created_at
    );
END
GO

-- Add other stored procedures...
-- (Execute the rest from Database/StoredProcedures/clubs.sql)
```

---

## Enhanced Error Handling

The code has been updated to provide better error messages. When you create a club now, you'll see:

? **Detailed error messages** if something fails
? **Stack trace** for debugging
? **Inner exception** details
? **HTTP status codes** that match the error type

---

## Checklist

- [ ] SQL Server is running
- [ ] `Football` database exists
- [ ] `[stf]` schema exists
- [ ] `[stf].[clubs]` table exists
- [ ] All stored procedures are created from `clubs.sql`
- [ ] Connection string in `appsettings.json` is correct
- [ ] Application is restarted after any changes
- [ ] No conflicting club names in database

---

## Still Having Issues?

1. **Check the error message in the response** - It now includes full details
2. **Look at the console output** - The app logs detailed error information
3. **Check SSMS** - Verify the database, schema, and table exist
4. **Run the setup script** - Execute the complete database setup above

---

**Status**: ? Enhanced Error Handling Applied
**Next Step**: Follow the troubleshooting steps above
