<# Read-only inventory. Never writes SQL, schedules jobs, or returns connection strings. #>
[CmdletBinding()]
param(
    [string]$Server = '.\SQL2025',
    [Parameter(Mandatory = $true)][ValidatePattern('^[A-Za-z][A-Za-z0-9_]{0,127}$')][string]$Database,
    [uri]$BaseUri = 'http://127.0.0.1:5080',
    [switch]$RequireTrustedCertificate,
    [string]$ReportPath
)
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Data
function Query-Inventory([string]$Sql) {
    $builder = New-Object System.Data.SqlClient.SqlConnectionStringBuilder
    $builder['Data Source'] = $Server
    $builder['Initial Catalog'] = 'master'
    $builder['Integrated Security'] = $true
    $builder['Encrypt'] = $true
    $builder['TrustServerCertificate'] = -not $RequireTrustedCertificate
    $builder['Pooling'] = $false
    $builder['Connect Timeout'] = 15
    $builder['Application Name'] = 'HPUNI read-only operations inventory'
    $connection = New-Object System.Data.SqlClient.SqlConnection($builder.ConnectionString)
    try {
        $connection.Open()
        $command = $connection.CreateCommand()
        $command.CommandText = 'SET LOCK_TIMEOUT 5000; ' + $Sql
        $command.CommandTimeout = 30
        [void]$command.Parameters.AddWithValue('@database', $Database)
        $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($command)
        $table = New-Object System.Data.DataTable
        [void]$adapter.Fill($table)
        $rows = @()
        foreach ($row in $table.Rows) {
            $item = [ordered]@{}
            foreach ($column in $table.Columns) {
                $value = $row[$column.ColumnName]
                $item[$column.ColumnName] = if ($value -is [DBNull]) { $null } else { $value }
            }
            $rows += $item
        }
        return $rows
    } finally { $connection.Dispose() }
}
if ($BaseUri.Scheme -notin @('http', 'https') -or $BaseUri.UserInfo -or $BaseUri.Query -or $BaseUri.Fragment -or $BaseUri.AbsolutePath -ne '/') { throw 'BaseUri must be an HTTP(S) origin without credentials, path, query or fragment.' }
if ($ReportPath -and (Test-Path -LiteralPath $ReportPath)) { throw 'Report path already exists; no overwrite is permitted.' }
$configuration = [ordered]@{}
foreach ($name in @('ConnectionStrings__Hpuni', 'DataProtection__Path', 'AttachmentScan__Host', 'AttachmentScan__Mode')) {
    $configuration[$name + '_PresentInThisConsole'] = -not [string]::IsNullOrWhiteSpace([Environment]::GetEnvironmentVariable($name, 'Process'))
}
$report = [ordered]@{
    ObservedUtc = [DateTime]::UtcNow.ToString('o')
    Scope = 'Read-only SQL metadata/history and unauthenticated HTTP. Console environment presence is not the running service configuration.'
    Server = $Server
    Database = $Database
    ConsoleConfigurationPresence = $configuration
    SqlServer = @(Query-Inventory "SELECT CONVERT(nvarchar(128),SERVERPROPERTY('ProductVersion')) AS ProductVersion,CONVERT(nvarchar(128),SERVERPROPERTY('Edition')) AS Edition,CONVERT(int,SERVERPROPERTY('EngineEdition')) AS EngineEdition")
    DatabaseState = @(Query-Inventory 'SELECT name,state_desc,recovery_model_desc,log_reuse_wait_desc FROM sys.databases WHERE name=@database')
    BackupHistory = @(Query-Inventory "SELECT MAX(CASE WHEN type='D' AND is_copy_only=0 THEN backup_finish_date END) AS LastRegularFullServerTime,MAX(CASE WHEN type='D' AND is_copy_only=1 THEN backup_finish_date END) AS LastCopyOnlyFullServerTime,MAX(CASE WHEN type='I' THEN backup_finish_date END) AS LastDifferentialServerTime,MAX(CASE WHEN type='L' THEN backup_finish_date END) AS LastLogServerTime FROM msdb.dbo.backupset WHERE database_name=@database")
    SourceVolumeSpace = @(Query-Inventory 'SELECT DISTINCT v.volume_mount_point,v.total_bytes,v.available_bytes FROM sys.master_files f CROSS APPLY sys.dm_os_volume_stats(f.database_id,f.file_id) v WHERE f.database_id=DB_ID(@database)')
    BackupSchedule = 'Not inspected or configured; backup history does not prove a scheduled policy.'
}
if ($report.DatabaseState.Count -ne 1) { throw 'Database was not found or metadata access is unavailable.' }
$health = @()
foreach ($path in @('/health/live', '/health/ready')) {
    $uri = [uri]::new($BaseUri, $path)
    try {
        $response = Invoke-WebRequest -Uri $uri -Method Get -UseBasicParsing -TimeoutSec 10 -MaximumRedirection 0
        $health += [ordered]@{ Path = $path; Status = [int]$response.StatusCode; Nosniff = [string]$response.Headers['X-Content-Type-Options']; HstsPresent = -not [string]::IsNullOrWhiteSpace([string]$response.Headers['Strict-Transport-Security']) }
    } catch {
        $status = $null
        if ($_.Exception.Response) { $status = [int]$_.Exception.Response.StatusCode }
        $health += [ordered]@{ Path = $path; Status = $status; ReachableWithoutRedirect = $false }
    }
}
$report.HttpOrigin = $BaseUri.GetLeftPart([UriPartial]::Authority)
$report.Health = $health
$json = ConvertTo-Json -InputObject $report -Depth 10
if ($ReportPath) {
    $stream = [IO.File]::Open([IO.Path]::GetFullPath($ReportPath), [IO.FileMode]::CreateNew, [IO.FileAccess]::Write, [IO.FileShare]::None)
    try { $bytes = [Text.UTF8Encoding]::new($false).GetBytes($json + [Environment]::NewLine); $stream.Write($bytes, 0, $bytes.Length) } finally { $stream.Dispose() }
}
Write-Output $json
