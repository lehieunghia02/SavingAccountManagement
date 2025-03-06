# PowerShell script to execute SQL scripts for database update

Write-Host "Running database update scripts..." -ForegroundColor Green

# Read the appsettings.json file
$appSettings = Get-Content -Path "appsettings.json" -Raw | ConvertFrom-Json

# Get the connection string
$connectionString = $appSettings.ConnectionStrings.HieuNghiaConnection

Write-Host "Using connection string from appsettings.json" -ForegroundColor Cyan

# Parse the connection string
$connectionParts = @{}
$connectionString.Split(';') | ForEach-Object {
    $key, $value = $_.Split('=')
    if ($key -and $value) {
        $connectionParts[$key.Trim()] = $value.Trim()
    }
}

$server = $connectionParts['Data Source']
$database = $connectionParts['Initial Catalog']
$userId = $connectionParts['User ID']
$password = $connectionParts['Password']

Write-Host "Server: $server" -ForegroundColor Cyan
Write-Host "Database: $database" -ForegroundColor Cyan
Write-Host "User ID: $userId" -ForegroundColor Cyan
Write-Host "Password: [HIDDEN]" -ForegroundColor Cyan

# Check if sqlcmd is available
try {
    $sqlcmdVersion = Invoke-Expression "sqlcmd -?"
    Write-Host "sqlcmd is available" -ForegroundColor Green
}
catch {
    Write-Host "sqlcmd is not available. Please install SQL Server Command Line Utilities." -ForegroundColor Red
    Write-Host "You can download it from: https://docs.microsoft.com/en-us/sql/tools/sqlcmd-utility" -ForegroundColor Red
    exit
}

# Run the SQL scripts
Write-Host "Running update_giaodich_table.sql..." -ForegroundColor Yellow
$giaoDichOutput = Invoke-Expression "sqlcmd -S `"$server`" -d `"$database`" -U `"$userId`" -P `"$password`" -i update_giaodich_table.sql"
$giaoDichOutput | Out-File -FilePath "update_giaodich_output.log"

Write-Host "Running update_migration_history.sql..." -ForegroundColor Yellow
$migrationOutput = Invoke-Expression "sqlcmd -S `"$server`" -d `"$database`" -U `"$userId`" -P `"$password`" -i update_migration_history.sql"
$migrationOutput | Out-File -FilePath "update_migration_output.log"

Write-Host "Database update completed. Check the log files for details." -ForegroundColor Green

# Alternative method using Invoke-Sqlcmd if available
Write-Host "`nAlternative method (if you have the SqlServer module installed):" -ForegroundColor Magenta
Write-Host "You can also run the scripts using Invoke-Sqlcmd if you have the SqlServer PowerShell module installed:" -ForegroundColor Magenta
Write-Host "Install-Module -Name SqlServer" -ForegroundColor Gray
Write-Host "Invoke-Sqlcmd -ServerInstance `"$server`" -Database `"$database`" -Username `"$userId`" -Password `"$password`" -InputFile `"update_giaodich_table.sql`"" -ForegroundColor Gray
Write-Host "Invoke-Sqlcmd -ServerInstance `"$server`" -Database `"$database`" -Username `"$userId`" -Password `"$password`" -InputFile `"update_migration_history.sql`"" -ForegroundColor Gray

# Pause to see the output
Write-Host "`nPress any key to continue..." -ForegroundColor Green
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") 