[CmdletBinding()]
param([string]$Server = '.\SQL2025', [switch]$ExecuteApprovedCleanup,
    [string]$ReportDirectory = 'C:\PYTHON\thực tập\backend\docs')
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
if (-not $ExecuteApprovedCleanup) { throw 'Requires explicit approved cleanup switch.' }
Add-Type -AssemblyName System.Data
$cs = New-Object System.Data.SqlClient.SqlConnectionStringBuilder
$cs['Data Source']=$Server; $cs['Initial Catalog']='master'; $cs['Integrated Security']=$true
$cs['Encrypt']=$true; $cs['TrustServerCertificate']=$true; $cs['Pooling']=$false
$connection = New-Object System.Data.SqlClient.SqlConnection($cs.ConnectionString)
function Query([string]$sql) {
    $command=$connection.CreateCommand(); $command.CommandTimeout=600; $command.CommandText=$sql
    $table=New-Object System.Data.DataTable
    $adapter=New-Object System.Data.SqlClient.SqlDataAdapter($command)
    try { [void]$adapter.Fill($table); return ,$table } finally { $adapter.Dispose(); $command.Dispose() }
}
function Lit([string]$value) { return "N'"+$value.Replace("'","''")+"'" }
function Ident([string]$value) { return '['+$value.Replace(']',']]')+']' }
$stamp=[DateTime]::UtcNow.ToString('yyyyMMdd_HHmmss')+'_'+[Guid]::NewGuid().ToString('N').Substring(0,8)
$reportPath=Join-Path $ReportDirectory "sql-cleanup-$stamp.json"
$report=[ordered]@{ Server=$Server; StartedUtc=[DateTime]::UtcNow.ToString('o'); Authorization='User approved backup, verification, then deletion of HpuniTests_* and HpuniRestore_* only'; Databases=@(); Completed=$false }
function Save-Report { $report | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $reportPath -Encoding UTF8 }
try {
    $connection.Open()
    $all=Query 'SELECT name,database_id,create_date,state_desc FROM sys.databases WHERE database_id>4 ORDER BY name'
    $report['Before']=@($all.Rows | ForEach-Object { [string]$_.name })
    $targets=@($all.Rows | Where-Object { $_.name -cmatch '^Hpuni(Tests|Restore)_[A-Za-z0-9_]+$' })
    $folder=[string](Query "SELECT CONVERT(nvarchar(4000),SERVERPROPERTY('InstanceDefaultBackupPath'))").Rows[0][0]
    if ([string]::IsNullOrWhiteSpace($folder)) { throw 'Missing server backup folder.' }
    foreach ($db in $targets) {
        if ($db.state_desc -ne 'ONLINE') { throw "Database not online: $($db.name)" }
        $entry=[ordered]@{ Name=[string]$db.name; DatabaseId=[int]$db.database_id; Created=$db.create_date.ToString('o'); BackupPath=[IO.Path]::Combine($folder,"approved-cleanup-$stamp-$($db.name).bak"); Verified=$false; Dropped=$false }
        $report.Databases+=,$entry
    }
    Save-Report
    foreach ($entry in $report.Databases) {
        $path=Lit $entry.BackupPath; $name=Ident $entry.Name
        $exists=Query "EXEC master.dbo.xp_fileexist $path"
        if ([int]$exists.Rows[0][0] -ne 0) { throw 'Backup destination exists.' }
        [void](Query "BACKUP DATABASE $name TO DISK=$path WITH COPY_ONLY,CHECKSUM,COMPRESSION,STOP_ON_ERROR")
        [void](Query "RESTORE VERIFYONLY FROM DISK=$path WITH CHECKSUM,STOP_ON_ERROR")
        $header=Query "RESTORE HEADERONLY FROM DISK=$path"
        if ($header.Rows.Count -ne 1 -or $header.Rows[0].DatabaseName -cne $entry.Name -or [int]$header.Rows[0].BackupType -ne 1 -or -not [bool]$header.Rows[0].HasBackupChecksums -or -not [bool]$header.Rows[0].IsCopyOnly) { throw 'Backup header mismatch.' }
        $entry.Verified=$true; Save-Report
        Write-Output "VERIFIED $($entry.Name)"
    }
    # All backups must pass before any deletion. Never terminate user sessions.
    foreach ($entry in $report.Databases) {
        if (-not $entry.Verified -or $entry.Name -cnotmatch '^Hpuni(Tests|Restore)_[A-Za-z0-9_]+$') { throw 'Deletion gate rejected.' }
        $literal=Lit $entry.Name
        $current=Query "SELECT database_id,create_date FROM sys.databases WHERE name=$literal"
        if ($current.Rows.Count -ne 1 -or [int]$current.Rows[0].database_id -ne $entry.DatabaseId -or $current.Rows[0].create_date.ToString('o') -ne $entry.Created) { throw 'Database identity changed.' }
        $sessions=Query "SELECT COUNT(*) FROM sys.dm_exec_sessions WHERE is_user_process=1 AND database_id=DB_ID($literal)"
        if ([int]$sessions.Rows[0][0] -gt 0) { throw "Database in use; retained: $($entry.Name)" }
        [void](Query ('DROP DATABASE '+(Ident $entry.Name)))
        $entry.Dropped=$true; Save-Report
        Write-Output "DROPPED $($entry.Name)"
    }
    $remaining=Query 'SELECT name FROM sys.databases WHERE database_id>4 ORDER BY name'
    $report['After']=@($remaining.Rows | ForEach-Object { [string]$_.name })
    $expected=@($report.Before | Where-Object { $_ -cnotmatch '^Hpuni(Tests|Restore)_[A-Za-z0-9_]+$' })
    if (@(Compare-Object $expected $report.After).Count -gt 0) { throw 'Remaining database list differs from expected.' }
    $report.Completed=$true
} catch { $report['Error']=$_.Exception.Message; throw }
finally { $report['FinishedUtc']=[DateTime]::UtcNow.ToString('o'); Save-Report; $connection.Dispose(); Write-Output "REPORT $reportPath" }
