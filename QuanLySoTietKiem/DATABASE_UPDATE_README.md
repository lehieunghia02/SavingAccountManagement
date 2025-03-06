# Database Update Instructions

This folder contains scripts to manually update the database schema and migration history to resolve the migration conflict.

## Problem Description

The application is encountering a database migration conflict. The migration system is attempting to create tables that already exist in the database, specifically when trying to apply the `AddAvatarUrlToUser` migration.

## Solution

Instead of using Entity Framework migrations directly, we've created SQL scripts to:

1. Add the missing columns to the `GiaoDichs` table
2. Update the migration history table to mark all pending migrations as applied

## Files Included

- `update_giaodich_table.sql`: SQL script to add the missing columns to the `GiaoDichs` table
- `update_migration_history.sql`: SQL script to update the migration history table
- `run_database_update.bat`: Batch file to execute the SQL scripts (Windows Command Prompt)
- `run_database_update.ps1`: PowerShell script to execute the SQL scripts (Windows PowerShell)

## Prerequisites

- SQL Server Command Line Utilities (sqlcmd) must be installed
- Access to the database with appropriate permissions

## How to Use

### Option 1: Using the Batch File (Command Prompt)

1. Open Command Prompt
2. Navigate to the directory containing these scripts
3. Run the batch file:
   ```
   run_database_update.bat
   ```

### Option 2: Using the PowerShell Script

1. Open PowerShell
2. Navigate to the directory containing these scripts
3. Run the PowerShell script:
   ```
   .\run_database_update.ps1
   ```

### Option 3: Manual Execution

If the automated scripts don't work, you can manually execute the SQL scripts using SQL Server Management Studio (SSMS) or any other SQL client:

1. Open SQL Server Management Studio
2. Connect to your database server
3. Open and execute `update_giaodich_table.sql`
4. Open and execute `update_migration_history.sql`

## Verification

After running the scripts, you should:

1. Check the log files (`update_giaodich_output.log` and `update_migration_output.log`) for any errors
2. Verify that the `GiaoDichs` table has the new columns:
   - `LoaiGiaoDich`
   - `MoTa`
   - `TrangThai`
   - `UserId`
3. Verify that the `__EFMigrationsHistory` table contains entries for all three migrations:
   - `20250225013528_AddAvatarUrlToUser`
   - `20250225013801_AddAvatarUrlColumn`
   - `20250304190536_AddMissingColumnsToGiaoDich`

## After Running the Scripts

Once the scripts have been successfully executed, the application should start without migration errors. The database schema will be updated with the necessary columns, and the migration history will be updated to reflect that all migrations have been applied.

## Troubleshooting

If you encounter any issues:

1. Check the connection string in `appsettings.json` to ensure it's correct
2. Ensure you have the necessary permissions to modify the database
3. Check if sqlcmd is installed and available in your PATH
4. Review the log files for specific error messages

For additional help, please contact the development team. 