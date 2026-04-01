# Action Plan - Execute Stored Procedures

## ?? IMMEDIATE NEXT STEPS

Your application needs all stored procedures to be created in the SQL Server database. Here's exactly what to do:

---

## ? STEP 1: Verify SQL Files Exist

All SQL script files have been created in: `Database/StoredProcedures/`

**Files created:**
- ? `clubs.sql` - 7 procedures
- ? `emails.sql` - 6 procedures  
- ? `club_contacts.sql` - 7 procedures
- ? `documents.sql` - 6 procedures
- ? `review_ratings.sql` - 6 procedures
- ? `tasks.sql` - 6 procedures
- ? `players_stf.sql` - 6 procedures
- ? `review_skill_details.sql` - 7 procedures

**Total: 51 stored procedures across 8 SQL files**

---

## ? STEP 2: Configure Your Connection String

Open `appsettings.json` and verify the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
  }
}
```

Replace:
- `YOUR_SERVER` - Your SQL Server instance (e.g., `localhost`, `(local)\SQLEXPRESS`)
- `YOUR_DATABASE` - Your database name (e.g., `FootballDashboard`)
- `YOUR_USER` - SQL Server login user
- `YOUR_PASSWORD` - SQL Server login password

---

## ? STEP 3: Verify Database User Permissions

The database user needs these permissions:
- ? `CREATE PROCEDURE`
- ? `ALTER PROCEDURE`
- ? `SELECT`, `INSERT`, `UPDATE`, `DELETE` on all tables in `[stf]` schema

**To grant permissions in SQL Server:**
```sql
-- If using SQL Server authentication
USE [YOUR_DATABASE];
GRANT CREATE PROCEDURE TO [YOUR_USER];
GRANT ALTER ANY PROCEDURE TO [YOUR_USER];
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::[stf] TO [YOUR_USER];
```

---

## ? STEP 4: Choose Execution Method

### **Option A: AUTOMATIC EXECUTION (RECOMMENDED)** ?

The easiest way! Your `Program.cs` is already configured to auto-execute all SQL files at startup.

**Simply run the application:**
```bash
dotnet run
```

**What happens automatically:**
1. Application loads
2. Detects all `.sql` files in `Database/StoredProcedures/`
3. Executes them in alphabetical order
4. Creates all 51 stored procedures
5. Application starts normally

? **This is the recommended approach** - No manual work needed!

---

### **Option B: MANUAL EXECUTION via SQL Server Management Studio**

If automatic execution doesn't work, do this manually:

1. **Open SQL Server Management Studio (SSMS)**

2. **Connect to your database**

3. **Execute SQL files in this order:**
   - File ? Open ? Select `clubs.sql` ? Execute (F5)
   - File ? Open ? Select `emails.sql` ? Execute
   - File ? Open ? Select `club_contacts.sql` ? Execute
   - File ? Open ? Select `documents.sql` ? Execute
   - File ? Open ? Select `review_ratings.sql` ? Execute
   - File ? Open ? Select `tasks.sql` ? Execute
   - File ? Open ? Select `players_stf.sql` ? Execute
   - File ? Open ? Select `review_skill_details.sql` ? Execute

4. **Verify all executed without errors**

---

### **Option C: COMMAND LINE EXECUTION**

Open PowerShell and run:

```powershell
cd E:\Football\FootballDashboardAPI\FootballDashboardAPI

# Execute each SQL file
sqlcmd -S (local) -d FootballDashboard -E -i "Database\StoredProcedures\clubs.sql"
sqlcmd -S (local) -d FootballDashboard -E -i "Database\StoredProcedures\emails.sql"
sqlcmd -S (local) -d FootballDashboard -E -i "Database\StoredProcedures\club_contacts.sql"
sqlcmd -S (local) -d FootballDashboard -E -i "Database\StoredProcedures\documents.sql"
sqlcmd -S (local) -d FootballDashboard -E -i "Database\StoredProcedures\review_ratings.sql"
sqlcmd -S (local) -d FootballDashboard -E -i "Database\StoredProcedures\tasks.sql"
sqlcmd -S (local) -d FootballDashboard -E -i "Database\StoredProcedures\players_stf.sql"
sqlcmd -S (local) -d FootballDashboard -E -i "Database\StoredProcedures\review_skill_details.sql"
```

Replace `(local)` with your server name and `FootballDashboard` with your database name.

---

## ? STEP 5: Verify Procedures Were Created

Run this SQL query to verify all procedures exist:

```sql
-- Check if procedures were created
SELECT COUNT(*) AS [TotalProcedures]
FROM sys.procedures 
WHERE schema_id = SCHEMA_ID('stf')

-- Should show: 51

-- List all procedures
SELECT name AS [ProcedureName]
FROM sys.procedures 
WHERE schema_id = SCHEMA_ID('stf')
ORDER BY name

-- Check specific procedure
SELECT OBJECT_ID('[stf].[sp_clubs_get_all]')  -- Should NOT be NULL
```

---

## ? STEP 6: Run Your Application

```bash
dotnet run
```

**Expected behavior:**
- ? No "Could not find stored procedure" errors
- ? API starts successfully
- ? Swagger UI accessible at `https://localhost:5001/swagger`
- ? All endpoints working

---

## ?? VERIFICATION CHECKLIST

After following the steps above, verify:

- [ ] Connection string is correct in `appsettings.json`
- [ ] SQL Server user has CREATE PROCEDURE permission
- [ ] All 8 SQL files are in `Database/StoredProcedures/` directory
- [ ] Procedures were created (run SQL query to verify)
- [ ] Application starts without "stored procedure not found" errors
- [ ] Can call API endpoints via Swagger UI

---

## ?? TROUBLESHOOTING

### **Error: "Could not find stored procedure 'stf.sp_clubs_get_all'"**

**Causes & Solutions:**

1. **Procedures not created**
   - Solution: Execute the SQL files manually using Method A or B above

2. **Wrong database connection**
   - Solution: Verify `DefaultConnection` in `appsettings.json` points to correct database

3. **User doesn't have CREATE PROCEDURE permission**
   - Solution: Grant permissions (see Step 3 above)

4. **Procedures created in wrong schema**
   - Solution: Verify procedures exist in `[stf]` schema, not `[dbo]`
   - Check: `SELECT * FROM sys.procedures WHERE name LIKE 'sp_clubs%'`

---

### **Error: "Login failed for user..."**

**Solution:**
1. Verify username/password in connection string
2. Verify user exists in SQL Server
3. Verify user has database access permissions

---

### **Error: "Database ... does not exist"**

**Solution:**
1. Verify database name in connection string
2. Verify database exists in SQL Server
3. Create database if needed:
   ```sql
   CREATE DATABASE FootballDashboard;
   ```

---

## ?? SUMMARY

| Step | Action | Status |
|------|--------|--------|
| 1 | Create SQL files | ? Done |
| 2 | Update connection string | ? You do this |
| 3 | Verify permissions | ? You do this |
| 4 | Execute SQL files | ? You do this |
| 5 | Verify procedures created | ? You do this |
| 6 | Run application | ? You do this |

---

## ?? ESTIMATED TIME

- **Automatic execution (Option A)**: 1-2 minutes
- **Manual execution (Option B)**: 5-10 minutes
- **Command line execution (Option C)**: 5-10 minutes

---

## ?? NEED HELP?

If you encounter issues:

1. **Check the logs** - Application will show exact error message
2. **Verify connection** - Test connection string in SSMS
3. **Run SQL manually** - Execute SQL files directly in SSMS to see exact errors
4. **Check permissions** - Ensure user has required permissions
5. **Restart application** - Sometimes helps with connection caching

---

## ? WHAT'S NEXT

Once all stored procedures are created and verified:

1. ? Test API endpoints in Swagger UI
2. ? Verify all CRUD operations work
3. ? Connect your React frontend
4. ? Monitor application performance
5. ? Deploy to production

---

**Current Status**: ?? Ready for Stored Procedure Execution
**Total Procedures Ready**: 51 across 8 SQL files
**Last Updated**: Current session
