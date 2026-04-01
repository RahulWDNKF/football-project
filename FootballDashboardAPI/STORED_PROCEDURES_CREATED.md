# Complete Stored Procedures Documentation

This document lists all the stored procedures created for the Football Dashboard API, organized by entity and SQL file.

## ?? Stored Procedures by Entity

### 1. Clubs (`stf` schema) - `clubs.sql`
- ? `[stf].[sp_clubs_get_all]` - Get all clubs
- ? `[stf].[sp_clubs_get_by_id]` - Get club by ID
- ? `[stf].[sp_clubs_insert]` - Insert new club
- ? `[stf].[sp_clubs_update]` - Update club
- ? `[stf].[sp_clubs_delete]` - Delete club
- ? `[stf].[sp_clubs_exists]` - Check club exists
- ? `[stf].[sp_clubs_name_exists]` - Check club name uniqueness

### 2. Emails (`stf` schema) - `emails.sql`
- ? `[stf].[sp_emails_get_all]` - Get all emails
- ? `[stf].[sp_emails_get_by_id]` - Get email by ID
- ? `[stf].[sp_emails_insert]` - Insert new email
- ? `[stf].[sp_emails_update]` - Update email
- ? `[stf].[sp_emails_delete]` - Delete email
- ? `[stf].[sp_emails_exists]` - Check email exists

### 3. Club Contacts (`stf` schema) - `club_contacts.sql`
- ? `[stf].[sp_club_contacts_get_all]` - Get all club contacts
- ? `[stf].[sp_club_contacts_get_by_id]` - Get club contact by ID
- ? `[stf].[sp_club_contacts_get_by_club_id]` - Get contacts by club ID
- ? `[stf].[sp_club_contacts_insert]` - Insert new club contact
- ? `[stf].[sp_club_contacts_update]` - Update club contact
- ? `[stf].[sp_club_contacts_delete]` - Delete club contact
- ? `[stf].[sp_club_contacts_exists]` - Check club contact exists

### 4. Documents (`stf` schema) - `documents.sql`
- ? `[stf].[sp_documents_get_all]` - Get all documents
- ? `[stf].[sp_documents_get_by_id]` - Get document by ID
- ? `[stf].[sp_documents_insert]` - Insert new document
- ? `[stf].[sp_documents_update]` - Update document
- ? `[stf].[sp_documents_delete]` - Delete document
- ? `[stf].[sp_documents_exists]` - Check document exists

### 5. Review Ratings (`stf` schema) - `review_ratings.sql`
- ? `[stf].[sp_review_ratings_get_all]` - Get all review ratings
- ? `[stf].[sp_review_ratings_get_by_id]` - Get review rating by ID
- ? `[stf].[sp_review_ratings_insert]` - Insert new review rating
- ? `[stf].[sp_review_ratings_update]` - Update review rating
- ? `[stf].[sp_review_ratings_delete]` - Delete review rating
- ? `[stf].[sp_review_ratings_exists]` - Check review rating exists

### 6. Tasks (`stf` schema) - `tasks.sql`
- ? `[stf].[sp_tasks_get_all]` - Get all tasks
- ? `[stf].[sp_tasks_get_by_id]` - Get task by ID
- ? `[stf].[sp_tasks_insert]` - Insert new task
- ? `[stf].[sp_tasks_update]` - Update task
- ? `[stf].[sp_tasks_delete]` - Delete task
- ? `[stf].[sp_tasks_exists]` - Check task exists

### 7. Players (`stf` schema) - `players_stf.sql`
**Note:** This is for the [stf].[players] table (Player1 model), separate from [dbo].[players]
- ? `[stf].[sp_players_get_all]` - Get all players
- ? `[stf].[sp_players_get_by_id]` - Get player by ID
- ? `[stf].[sp_players_insert]` - Insert new player
- ? `[stf].[sp_players_update]` - Update player
- ? `[stf].[sp_players_delete]` - Delete player
- ? `[stf].[sp_players_exists]` - Check player exists

### 8. Review Skill Details (`stf` schema) - `review_skill_details.sql`
**Note:** This table has a composite primary key (review_id, skill_key)
- ? `[stf].[sp_review_skill_details_get_all]` - Get all review skill details
- ? `[stf].[sp_review_skill_details_get_by_id]` - Get skill detail by review_id and skill_key
- ? `[stf].[sp_review_skill_details_get_by_review_id]` - Get all skills for a review
- ? `[stf].[sp_review_skill_details_insert]` - Insert new skill detail
- ? `[stf].[sp_review_skill_details_update]` - Update skill detail
- ? `[stf].[sp_review_skill_details_delete]` - Delete skill detail
- ? `[stf].[sp_review_skill_details_exists]` - Check skill detail exists

---

## ?? SQL Files Location

All SQL files are located in: `Database/StoredProcedures/`

```
Database/StoredProcedures/
??? clubs.sql                     ? 7 procedures
??? emails.sql                    ? 6 procedures
??? club_contacts.sql             ? 7 procedures
??? documents.sql                 ? 6 procedures
??? review_ratings.sql            ? 6 procedures
??? tasks.sql                     ? 6 procedures
??? players_stf.sql               ? 6 procedures
??? review_skill_details.sql      ? 7 procedures
??? STORED_PROCEDURES_TEMPLATES.sql  (old - reference only)
??? README.md
```

---

## ?? How to Execute These Procedures

### Option 1: Automatic Execution (Recommended)
The `Program.cs` is configured to automatically execute all `.sql` files in the `Database/StoredProcedures/` directory at application startup.

Simply ensure:
1. All SQL files are in `Database/StoredProcedures/` folder
2. Database user has `CREATE PROCEDURE` permission
3. Connection string is correct in `appsettings.json`

### Option 2: Manual Execution
1. Open SQL Server Management Studio (SSMS)
2. Connect to your database
3. Open each SQL file
4. Execute the scripts in this order:
   - `clubs.sql`
   - `emails.sql`
   - `club_contacts.sql`
   - `documents.sql`
   - `tasks.sql`
   - `players_stf.sql`
   - `review_ratings.sql`
   - `review_skill_details.sql`

### Option 3: Execute via Command Line
```powershell
sqlcmd -S YOUR_SERVER -d YOUR_DATABASE -U YOUR_USER -P YOUR_PASSWORD -i "Database\StoredProcedures\clubs.sql"
sqlcmd -S YOUR_SERVER -d YOUR_DATABASE -U YOUR_USER -P YOUR_PASSWORD -i "Database\StoredProcedures\emails.sql"
# ... repeat for other files
```

---

## ? Total Procedures Created

- **Clubs**: 7 procedures
- **Emails**: 6 procedures
- **Club Contacts**: 7 procedures
- **Documents**: 6 procedures
- **Review Ratings**: 6 procedures
- **Tasks**: 6 procedures
- **Players (STF)**: 6 procedures
- **Review Skill Details**: 7 procedures

**Total: 51 stored procedures**

---

## ?? Standard Procedure Pattern

Each entity typically has these procedures:

### Standard CRUD Operations
```sql
[schema].[sp_{entity}_get_all]      -- Returns all records
[schema].[sp_{entity}_get_by_id]    -- Returns single record by ID
[schema].[sp_{entity}_insert]       -- Creates new record
[schema].[sp_{entity}_update]       -- Updates existing record
[schema].[sp_{entity}_delete]       -- Deletes record by ID
[schema].[sp_{entity}_exists]       -- Returns 1 if exists, 0 otherwise
```

### Additional Procedures (as needed)
```sql
[schema].[sp_{entity}_{field}_exists]      -- Check field uniqueness
[schema].[sp_{entity}_get_by_{relation}]   -- Filter by related entity
```

---

## ?? Parameters Convention

### Insert/Update Parameters
Parameters follow the **column name** convention from the database:
```sql
@column_name1 DATATYPE,
@column_name2 DATATYPE = NULL,
...
```

### Get by ID Parameters
Standard parameter naming:
```sql
@Id      -- For single column primary keys
@review_id + @skill_key   -- For composite keys
```

### Filter Parameters
Descriptive names:
```sql
@ClubId
@PlayerId
@ReviewId
@ScoutId
```

---

## ?? Important Notes

### Schemas
- **`[stf]` schema**: Scoping Football Tables (Scout Football Training)
  - Clubs, Emails, Club Contacts, Documents, Tasks
  - Players (stf.players), Review Ratings, Review Skill Details
  
- **`[dbo]` schema**: Default schema (for other applications or future use)

### Composite Keys
- **Review Skill Details**: Uses composite key `(review_id, skill_key)`
  - Get by ID requires both parameters: `@review_id` and `@skill_key`
  - Delete and Update also require both parameters

### NULL Handling
- Optional fields use `= NULL` default parameters
- When inserting NULL values from C#, use `DBNull.Value`

### Return Values
- **Non-query operations** (INSERT, UPDATE, DELETE): Return `@@ROWCOUNT`
- **Query operations** (SELECT): Return result set
- **Exists procedures**: Return `COUNT(1)` (1 if exists, 0 if not)

---

## ?? Verification Checklist

After executing all procedures, verify they exist:

```sql
-- Check stored procedures in [stf] schema
SELECT name FROM sys.procedures WHERE schema_id = SCHEMA_ID('stf')

-- Count should be ~51 procedures
SELECT COUNT(*) FROM sys.procedures WHERE schema_id = SCHEMA_ID('stf')

-- Check specific procedure
SELECT OBJECT_ID('[stf].[sp_clubs_get_all]')  -- Should not be NULL
```

---

## ?? Troubleshooting

### "Could not find stored procedure"
**Cause**: Stored procedure not created or wrong schema
**Solution**: 
1. Run the SQL script again
2. Verify procedure name matches exactly (case-insensitive but schema is case-sensitive)
3. Check database user has CREATE PROCEDURE permission

### "Invalid column name"
**Cause**: Column name mismatch between procedure and table
**Solution**: 
1. Verify column names in procedure match actual table columns
2. Check for typos in column names

### "Incorrect syntax near..."
**Cause**: SQL syntax error in procedure definition
**Solution**: 
1. Run procedure creation SQL manually in SSMS
2. Check for unclosed parentheses or quotes

### Database user permissions
**Required permissions**:
- CREATE PROCEDURE
- ALTER PROCEDURE
- SELECT, INSERT, UPDATE, DELETE on affected tables

---

## ?? Related Files

- **Program.cs** - Loads and executes all `.sql` files at startup
- **EntityCrudController.cs** - Generic controller that calls these procedures
- **Models/** - Entity definitions that map to these procedures
- **API_IMPLEMENTATION_GUIDE.md** - Detailed implementation guide

---

**Status**: ? All 51 stored procedures created and ready for execution
**Last Updated**: Current session
**Total SQL Files**: 8
