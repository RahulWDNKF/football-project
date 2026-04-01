# ? QUICK FIX - Missing Stored Procedures

## The Problem
```
Error: Could not find stored procedure 'stf.sp_club_contacts_get_all'
Error: Could not find stored procedure 'stf.sp_documents_get_all'
Error: Could not find stored procedure 'stf.sp_review_ratings_get_all'
```

## The Solution ?

### FASTEST METHOD: Let the App Do It (Auto Execution)

1. **Update `appsettings.json`** with correct database connection:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
  }
}
```

2. **Run the app:**
```bash
dotnet run
```

3. **That's it!** ? 
   - The app automatically creates all 51 stored procedures
   - No manual SQL execution needed

---

### IF Auto Execution Doesn't Work: Manual Method

Open **SQL Server Management Studio** and run these 8 SQL files in order:

```
1. Database/StoredProcedures/clubs.sql
2. Database/StoredProcedures/emails.sql
3. Database/StoredProcedures/club_contacts.sql
4. Database/StoredProcedures/documents.sql
5. Database/StoredProcedures/tasks.sql
6. Database/StoredProcedures/review_ratings.sql
7. Database/StoredProcedures/players_stf.sql
8. Database/StoredProcedures/review_skill_details.sql
```

---

## What Was Created

? **8 SQL Files** = **51 Stored Procedures**

| Entity | File | Procedures |
|--------|------|-----------|
| Clubs | `clubs.sql` | 7 |
| Emails | `emails.sql` | 6 |
| Club Contacts | `club_contacts.sql` | 7 |
| Documents | `documents.sql` | 6 |
| Tasks | `tasks.sql` | 6 |
| Review Ratings | `review_ratings.sql` | 6 |
| Players (STF) | `players_stf.sql` | 6 |
| Skill Details | `review_skill_details.sql` | 7 |

---

## Verify It Worked

Run this SQL query:

```sql
SELECT COUNT(*) FROM sys.procedures 
WHERE schema_id = SCHEMA_ID('stf')
-- Should show: 51
```

---

## Done! ??

All errors should be gone. Your API is ready!

---

?? For detailed instructions, see: `ACTION_PLAN_EXECUTE_PROCEDURES.md`
