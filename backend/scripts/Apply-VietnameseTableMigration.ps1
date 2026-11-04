[CmdletBinding()]
param([string]$Workspace = 'C:\PYTHON\thực tập', [string]$Server = '.\SQL2025',
    [string]$Database = 'HpuniEquipmentUi_20261104_0950')
Set-StrictMode -Version Latest
$ErrorActionPreference='Stop'
$docs=Join-Path $Workspace 'backend/docs'
$backup=Get-Content -LiteralPath (Join-Path $docs 'vietnamese-tables-before-backup-20261104.json') -Raw | ConvertFrom-Json
if (-not $backup.BackupChecksumVerified -or $backup.SourceDatabase -cne $Database -or $backup.Server -cne $Server) { throw 'Verified source backup required.' }
$source=Get-Content -LiteralPath (Join-Path $Workspace 'backend/src/Hpuni.Infrastructure/VietnameseTableNames.cs') -Raw
$mapping=[ordered]@{}
foreach($match in [regex]::Matches($source,'\["([A-Za-z0-9_]+)"\]\s*=\s*"([A-Za-z0-9_]+)"')) { $mapping[$match.Groups[1].Value]=$match.Groups[2].Value }
if ($mapping.Count -ne 39) { throw 'Unexpected mapping size.' }
$mapping['__EFMigrationsHistory']='LichSuCapNhatCauTruc'
$migration=Get-Content -LiteralPath (Join-Path $docs 'vietnamese-table-migration.sql') -Raw
if ($migration -match '(?i)\b(DROP\s+TABLE|DROP\s+COLUMN|DELETE\s+FROM|TRUNCATE\s+TABLE)\b') { throw 'Data-destructive SQL rejected.' }
$migration=[regex]::Replace($migration,'(?im)^\s*GO\s*$','')
Add-Type -AssemblyName System.Data
$cs=New-Object System.Data.SqlClient.SqlConnectionStringBuilder
$cs['Data Source']=$Server; $cs['Initial Catalog']=$Database; $cs['Integrated Security']=$true
$cs['Encrypt']=$true; $cs['TrustServerCertificate']=$true; $cs['Pooling']=$false
$connection=New-Object System.Data.SqlClient.SqlConnection($cs.ConnectionString)
$transaction=$null
function Query([string]$sql) {
    $command=$connection.CreateCommand(); $command.CommandText=$sql; $command.CommandTimeout=120
    if($null -ne $transaction){$command.Transaction=$transaction}
    $table=New-Object System.Data.DataTable; $adapter=New-Object System.Data.SqlClient.SqlDataAdapter($command)
    try { [void]$adapter.Fill($table); return ,$table } finally { $adapter.Dispose(); $command.Dispose() }
}
$report=[ordered]@{Database=$Database;BackupPath=$backup.BackupPath;Tables=@();Committed=$false}
try {
    $connection.Open(); $transaction=$connection.BeginTransaction([System.Data.IsolationLevel]::Serializable)
    $tables=Query 'SELECT name FROM sys.tables WHERE is_ms_shipped=0'
    if($tables.Rows.Count -ne $mapping.Count){throw 'Unexpected source table count.'}
    foreach($row in $tables.Rows){if(-not $mapping.Contains([string]$row.name)){throw 'Unmapped source table.'}}
    foreach($old in $mapping.Keys){
        $row=(Query "SELECT COUNT_BIG(*) AS Rows, CHECKSUM_AGG(BINARY_CHECKSUM(*)) AS Digest FROM dbo.[$old] WITH (TABLOCKX,HOLDLOCK)").Rows[0]
        $report.Tables+=,[ordered]@{Old=$old;New=$mapping[$old];BeforeRows=[long]$row.Rows;BeforeDigest=[string]$row.Digest}
    }
    [void](Query "EXEC sys.sp_rename N'dbo.__EFMigrationsHistory', N'LichSuCapNhatCauTruc', N'OBJECT'")
    [void](Query $migration)
    foreach($entry in $report.Tables){
        $row=(Query ("SELECT COUNT_BIG(*) AS Rows, CHECKSUM_AGG(BINARY_CHECKSUM(*)) AS Digest FROM dbo.["+$entry.New+']')).Rows[0]
        $entry['AfterRows']=[long]$row.Rows; $entry['AfterDigest']=[string]$row.Digest
        if($entry.Old -eq '__EFMigrationsHistory'){
            if($entry.AfterRows -ne $entry.BeforeRows+1){throw 'Expected one new migration history row.'}
        } elseif($entry.BeforeRows -ne $entry.AfterRows -or $entry.BeforeDigest -cne $entry.AfterDigest){throw "Data verification failed: $($entry.Old)"}
    }
    $bad=Query 'SELECT name FROM sys.foreign_keys WHERE is_disabled=1 OR is_not_trusted=1'
    if($bad.Rows.Count -gt 0){throw 'Untrusted or disabled foreign key.'}
    $transaction.Commit(); $report.Committed=$true
} catch { if($null -ne $transaction){try{$transaction.Rollback()}catch{}}; $report['Error']=$_.Exception.Message; throw }
finally { $connection.Dispose(); $report | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $docs 'vietnamese-tables-data-verification-20261104.json') -Encoding UTF8 }
