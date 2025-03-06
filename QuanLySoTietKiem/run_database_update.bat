@echo off
echo Running database update scripts...

REM Get the connection string from appsettings.json
REM This is a simplified approach - in a real scenario, you might want to use a more robust JSON parser
for /f "tokens=*" %%a in ('findstr "HieuNghiaConnection" appsettings.json') do (
    set connectionLine=%%a
)

REM Extract the connection string
set connectionLine=%connectionLine:"HieuNghiaConnection": "=%
set connectionString=%connectionLine:",=%
set connectionString=%connectionString:"=%

echo Using connection string: %connectionString%

REM Extract server name, database name, user ID, and password from the connection string
for /f "tokens=1-4 delims=;" %%a in ("%connectionString%") do (
    set server=%%a
    set database=%%b
    set uid=%%c
    set pwd=%%d
)

set server=%server:Data Source==%
set database=%database:Initial Catalog==%
set uid=%uid:User ID==%
set pwd=%pwd:Password==%

echo Server: %server%
echo Database: %database%
echo User ID: %uid%
echo Password: [HIDDEN]

REM Run the SQL scripts using sqlcmd
echo Running update_giaodich_table.sql...
sqlcmd -S %server% -d %database% -U %uid% -P %pwd% -i update_giaodich_table.sql -o update_giaodich_output.log

echo Running update_migration_history.sql...
sqlcmd -S %server% -d %database% -U %uid% -P %pwd% -i update_migration_history.sql -o update_migration_output.log

echo Database update completed. Check the log files for details.
pause 