<#
.SYNOPSIS
Sao lưu COPY_ONLY và diễn tập khôi phục sang database mới; không xóa/ghi đè.
.DESCRIPTION
Chạy trên máy SQL Server Windows với Windows Authentication. Đường dẫn tệp là
đường dẫn trên máy SQL Server, không phải máy client. Không dùng cho PITR.
#>
[CmdletBinding()]
param(
    [string]$Server = '.\SQL2025',
    [Parameter(Mandatory = $true)][ValidatePattern('^[A-Za-z][A-Za-z0-9_]{0,127}$')][string]$SourceDatabase,
    [ValidateSet('Backup', 'Restore', 'Drill')][string]$Mode = 'Drill',
    [string]$BackupPath,
    [ValidatePattern('^HpuniRestore_[A-Za-z0-9_]{1,110}$')][string]$TargetDatabase,
    [ValidateRange(30, 86400)][int]$CommandTimeoutSeconds = 600,
    [switch]$RequireTrustedCertificate,
    [string]$ReportPath
)
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Data

function Sql-Literal([string]$Value) { return "N'" + $Value.Replace("'", "''") + "'" }
function Sql-Identifier([string]$Value) { return '[' + $Value.Replace(']', ']]') + ']' }
function Read-Sql([string]$Sql, [string]$Database = 'master') {
    $builder = New-Object System.Data.SqlClient.SqlConnectionStringBuilder
    $builder['Data Source'] = $Server
    $builder['Initial Catalog'] = $Database
    $builder['Integrated Security'] = $true
    $builder['Encrypt'] = $true
    $builder['TrustServerCertificate'] = -not $RequireTrustedCertificate # local development default
    $builder['Application Name'] = 'HPUNI backup restore drill'
    $builder['Connect Timeout'] = 15
    $builder['Pooling'] = $false
    $connection = New-Object System.Data.SqlClient.SqlConnection($builder.ConnectionString)
    try {
        $connection.Open()
        $command = $connection.CreateCommand()
        $command.CommandText = 'SET LOCK_TIMEOUT 5000; ' + $Sql
        $command.CommandTimeout = $CommandTimeoutSeconds
        $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($command)
        $table = New-Object System.Data.DataTable
        [void]$adapter.Fill($table)
        return ,$table
    } finally { $connection.Dispose() }
}
function Scalar-Sql([string]$Sql) {
    $table = Read-Sql $Sql
    if ($table.Rows.Count -ne 1 -or $table.Columns.Count -ne 1) { throw 'Expected a scalar SQL result.' }
    return $table.Rows[0][0]
}
function Server-FileExists([string]$Path) {
    $table = Read-Sql ('EXEC master.dbo.xp_fileexist ' + (Sql-Literal $Path))
    return [int]$table.Rows[0][0] -eq 1
}
function Assert-NewServerPath([string]$Path) {
    $table = Read-Sql ('EXEC master.dbo.xp_fileexist ' + (Sql-Literal $Path))
    $registered = [int](Scalar-Sql ('SELECT COUNT(*) FROM sys.master_files WHERE physical_name = ' + (Sql-Literal $Path)))
    if ([int]$table.Rows[0][0] -ne 0 -or [int]$table.Rows[0][1] -ne 0 -or [int]$table.Rows[0][2] -ne 1 -or $registered -ne 0) { throw 'Server destination exists, is registered, or has no parent directory; no overwrite is permitted.' }
}
function Database-Exists([string]$Name) {
    return [int](Scalar-Sql ('SELECT COUNT(*) FROM sys.databases WHERE name = ' + (Sql-Literal $Name))) -gt 0
}
function Get-Snapshot([string]$Database) {
    $tables = Read-Sql "SELECT s.name AS SchemaName,t.name AS TableName FROM sys.tables t JOIN sys.schemas s ON t.schema_id=s.schema_id WHERE t.is_ms_shipped=0 ORDER BY s.name,t.name" $Database
    $counts = [ordered]@{}
    foreach ($row in $tables.Rows) {
        $qualified = (Sql-Identifier $row.SchemaName) + '.' + (Sql-Identifier $row.TableName)
        $count = Read-Sql ('SELECT COUNT_BIG(*) AS Total FROM ' + $qualified) $Database
        $counts[$row.SchemaName + '.' + $row.TableName] = [long]$count.Rows[0].Total
    }
    $history = Read-Sql "SELECT s.name AS SchemaName,t.name AS TableName FROM sys.tables t JOIN sys.schemas s ON s.schema_id=t.schema_id WHERE t.name IN (N'LichSuCapNhatCauTruc',N'__EFMigrationsHistory')" $Database
    if ($history.Rows.Count -ne 1) { throw 'Expected exactly one migration history table.' }
    $migrations = Read-Sql ('SELECT MigrationId FROM ' + (Sql-Identifier ([string]$history.Rows[0].SchemaName)) + '.' + (Sql-Identifier ([string]$history.Rows[0].TableName)) + ' ORDER BY MigrationId') $Database
    # Compute SHA on SQL Server; only IDs, lengths, digests and a mismatch count leave SQL.
    $attachmentNames = Read-Sql "SELECT s.name AS SchemaName,t.name AS TableName FROM sys.tables t JOIN sys.schemas s ON s.schema_id=t.schema_id WHERE t.name IN (N'TepDinhKemChungTu',N'VoucherAttachments')" $Database
    $contentNames = Read-Sql "SELECT s.name AS SchemaName,t.name AS TableName FROM sys.tables t JOIN sys.schemas s ON s.schema_id=t.schema_id WHERE t.name IN (N'NoiDungTepChungTu',N'VoucherAttachmentData')" $Database
    if ($attachmentNames.Rows.Count -ne 1 -or $contentNames.Rows.Count -ne 1) { throw 'Attachment table mapping is ambiguous.' }
    $attachmentTable = (Sql-Identifier ([string]$attachmentNames.Rows[0].SchemaName)) + '.' + (Sql-Identifier ([string]$attachmentNames.Rows[0].TableName))
    $contentTable = (Sql-Identifier ([string]$contentNames.Rows[0].SchemaName)) + '.' + (Sql-Identifier ([string]$contentNames.Rows[0].TableName))
    $attachments = Read-Sql ("SELECT a.Id,a.Size,a.Sha256,LOWER(CONVERT(varchar(64),HASHBYTES('SHA2_256',d.Content),2)) AS ActualSha256,DATALENGTH(d.Content) AS ActualSize FROM " + $attachmentTable + ' a LEFT JOIN ' + $contentTable + ' d ON d.AttachmentId=a.Id ORDER BY a.Id') $Database
    $files = @()
    foreach ($row in $attachments.Rows) {
        if ($row.ActualSha256 -is [DBNull] -or $row.ActualSha256 -ne $row.Sha256.ToLowerInvariant() -or [long]$row.Size -ne [long]$row.ActualSize) { throw 'Voucher attachment checksum/size mismatch; database retained for investigation.' }
        $files += [ordered]@{ Id = [string]$row.Id; Size = [long]$row.Size; Sha256 = [string]$row.ActualSha256 }
    }
    return [ordered]@{ Counts = $counts; Migrations = @($migrations.Rows | ForEach-Object { [string]$_.MigrationId }); Attachments = $files }
}

if ($ReportPath) {
    $resolvedReportPath = [IO.Path]::GetFullPath($ReportPath)
    if ([IO.File]::Exists($resolvedReportPath) -or [IO.Directory]::Exists($resolvedReportPath)) { throw 'Report path already exists; no overwrite is permitted.' }
    if (-not [IO.Directory]::Exists([IO.Path]::GetDirectoryName($resolvedReportPath))) { throw 'Report parent directory does not exist.' }
}
if ($SourceDatabase -in @('master', 'model', 'msdb', 'tempdb')) { throw 'System databases are not supported.' }
if ($Mode -ne 'Restore' -and -not (Database-Exists $SourceDatabase)) { throw 'Source database does not exist.' }
if ($Mode -ne 'Restore' -and [int](Scalar-Sql ('SELECT COUNT(*) FROM sys.databases WHERE state_desc = N''ONLINE'' AND name = ' + (Sql-Literal $SourceDatabase))) -ne 1) { throw 'Source database must be ONLINE.' }
if ($Mode -eq 'Restore' -and [string]::IsNullOrWhiteSpace($BackupPath)) { throw 'Restore requires BackupPath on the SQL Server machine.' }
if ($Mode -ne 'Restore' -and $BackupPath) { throw 'BackupPath is accepted only in Restore mode; backup always creates a unique file.' }
$stamp = [DateTime]::UtcNow.ToString('yyyyMMdd_HHmmss')
$nonce = [Guid]::NewGuid().ToString('N')
if (-not $TargetDatabase) { $TargetDatabase = 'HpuniRestore_' + $stamp + '_' + $nonce.Substring(0, 12) }
if ($Mode -ne 'Backup' -and (Database-Exists $TargetDatabase)) { throw 'Target database already exists; no overwrite is permitted.' }
if ($SourceDatabase -eq $TargetDatabase) { throw 'Source and target must differ.' }
$paths = Read-Sql "SELECT CONVERT(nvarchar(4000),SERVERPROPERTY('InstanceDefaultBackupPath')) AS BackupPath,CONVERT(nvarchar(4000),SERVERPROPERTY('InstanceDefaultDataPath')) AS DataPath,CONVERT(nvarchar(4000),SERVERPROPERTY('InstanceDefaultLogPath')) AS LogPath"
$compression = if ([int](Scalar-Sql "SELECT CONVERT(int,SERVERPROPERTY('EngineEdition'))") -eq 4) { 'NO_COMPRESSION' } else { 'COMPRESSION' }
foreach ($column in @('BackupPath', 'DataPath', 'LogPath')) {
    if ($paths.Rows[0][$column] -is [DBNull] -or [string]::IsNullOrWhiteSpace([string]$paths.Rows[0][$column])) { throw ('SQL Server default path is missing: ' + $column) }
}
if ($Mode -ne 'Restore') {
    $BackupPath = [IO.Path]::Combine([string]$paths.Rows[0].BackupPath, $SourceDatabase + '_' + $stamp + '_' + $nonce + '.bak')
    Assert-NewServerPath $BackupPath
} elseif (-not (Server-FileExists $BackupPath)) { throw 'Backup file does not exist on the SQL Server machine.' }
$before = if ($Mode -eq 'Drill') { Get-Snapshot $SourceDatabase } else { $null }
$started = [DateTime]::UtcNow
if ($Mode -ne 'Restore') {
    [void](Read-Sql ('BACKUP DATABASE ' + (Sql-Identifier $SourceDatabase) + ' TO DISK = ' + (Sql-Literal $BackupPath) + ' WITH COPY_ONLY, CHECKSUM, ' + $compression + ', STOP_ON_ERROR'))
}
[void](Read-Sql ('RESTORE VERIFYONLY FROM DISK = ' + (Sql-Literal $BackupPath) + ' WITH CHECKSUM, STOP_ON_ERROR'))
$header = Read-Sql ('RESTORE HEADERONLY FROM DISK = ' + (Sql-Literal $BackupPath))
if ($header.Rows.Count -ne 1 -or [int]$header.Rows[0].BackupType -ne 1 -or [string]$header.Rows[0].DatabaseName -ne $SourceDatabase -or -not [bool]$header.Rows[0].HasBackupChecksums -or -not [bool]$header.Rows[0].IsCopyOnly) { throw 'Backup header verification failed.' }
$report = [ordered]@{ StartedUtc = $started.ToString('o'); Server = $Server; SourceDatabase = $SourceDatabase; Mode = $Mode; BackupPath = $BackupPath; CopyOnly = $true; BackupChecksumVerified = $true }
if ($Mode -ne 'Backup') {
    $fileList = Read-Sql ('RESTORE FILELISTONLY FROM DISK = ' + (Sql-Literal $BackupPath))
    $moves = @()
    $index = 0
    foreach ($row in $fileList.Rows) {
        if ([string]$row.Type -notin @('D', 'L')) { throw 'Only ordinary SQL data/log files are supported; backup retained.' }
        $index++
        $folder = if ([string]$row.Type -eq 'L') { [string]$paths.Rows[0].LogPath } else { [string]$paths.Rows[0].DataPath }
        $extension = if ([string]$row.Type -eq 'L') { '.ldf' } else { '.ndf' }
        $destination = [IO.Path]::Combine($folder, $TargetDatabase + '_' + $nonce + '_' + $index + $extension)
        Assert-NewServerPath $destination
        $moves += 'MOVE ' + (Sql-Literal ([string]$row.LogicalName)) + ' TO ' + (Sql-Literal $destination)
    }
    if ($moves.Count -eq 0) { throw 'Backup contains no data files.' }
    # Repeat immediately before RESTORE; RESTORE without REPLACE also refuses an existing foreign DB.
    if (Database-Exists $TargetDatabase) { throw 'Target database now exists; restore cancelled.' }
    $restoreGate = 'DECLARE @gate int; EXEC @gate = sys.sp_getapplock @Resource = ' + (Sql-Literal ('HpuniRestoreGate:' + $TargetDatabase.ToUpperInvariant())) + ', @LockMode = N''Exclusive'', @LockOwner = N''Session'', @LockTimeout = 10000; IF @gate < 0 THROW 51000, ''Cannot acquire restore gate.'', 1; IF DB_ID(' + (Sql-Literal $TargetDatabase) + ') IS NOT NULL THROW 51001, ''Target database already exists; restore cancelled.'', 1; '
    [void](Read-Sql ($restoreGate + 'RESTORE DATABASE ' + (Sql-Identifier $TargetDatabase) + ' FROM DISK = ' + (Sql-Literal $BackupPath) + ' WITH ' + ($moves -join ', ') + ', CHECKSUM, STOP_ON_ERROR, RECOVERY'))
    $integrity = Read-Sql ('DBCC CHECKDB (' + (Sql-Literal $TargetDatabase) + ') WITH NO_INFOMSGS, ALL_ERRORMSGS, TABLERESULTS')
    if ($integrity.Rows.Count -gt 0) { throw 'DBCC CHECKDB returned findings; restored database retained.' }
    $restored = Get-Snapshot $TargetDatabase
    if ($Mode -eq 'Drill') {
        $after = Get-Snapshot $SourceDatabase
        $baselineJson = ConvertTo-Json -InputObject $before -Depth 8 -Compress
        if ($baselineJson -ne (ConvertTo-Json -InputObject $after -Depth 8 -Compress)) { throw 'Source changed during the drill; backup and restored DB retained. Repeat comparison in a quiet window.' }
        if ($baselineJson -ne (ConvertTo-Json -InputObject $restored -Depth 8 -Compress)) { throw 'Restored snapshot differs from source; both databases retained.' }
        $report.SourceStableDuringDrill = $true
        $report.RowCountsMigrationsAttachmentsMatch = $true
    }
    $report.TargetDatabase = $TargetDatabase
    $report.CheckDbPassed = $true
    $report.Snapshot = $restored
}
$report.CompletedUtc = [DateTime]::UtcNow.ToString('o')
$report.DurationSeconds = [math]::Round(([DateTime]::UtcNow - $started).TotalSeconds, 3)
$json = ConvertTo-Json -InputObject $report -Depth 10
if ($ReportPath) {
    $resolvedReportPath = [IO.Path]::GetFullPath($ReportPath)
    $stream = New-Object IO.FileStream($resolvedReportPath, [IO.FileMode]::CreateNew, [IO.FileAccess]::Write, [IO.FileShare]::None)
    try { $bytes = [Text.UTF8Encoding]::new($false).GetBytes($json + [Environment]::NewLine); $stream.Write($bytes, 0, $bytes.Length) } finally { $stream.Dispose() }
}
Write-Output $json
