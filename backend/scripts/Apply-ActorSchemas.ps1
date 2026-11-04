[CmdletBinding()]
param([string]$Workspace='C:\PYTHON\thực tập')
Set-StrictMode -Version Latest
$ErrorActionPreference='Stop'
$docs=Join-Path $Workspace 'backend/docs'
$backup=Get-Content -LiteralPath (Join-Path $docs 'actor-schemas-before-backup-20261104.json') -Raw | ConvertFrom-Json
if(-not $backup.BackupChecksumVerified -or $backup.SourceDatabase -cne 'HpuniEquipmentUi_20261104_0950'){throw 'Verified backup required.'}
$source=Get-Content -LiteralPath (Join-Path $Workspace 'backend/src/Hpuni.Infrastructure/ActorTableSchemas.cs') -Raw
$map=@{}
foreach($match in [regex]::Matches($source,'\["([A-Za-z]+)"\]\s*=\s*"([A-Za-z]+)"')){$map[$match.Groups[1].Value]=$match.Groups[2].Value}
if($map.Count -ne 40){throw 'Unexpected mapping count.'}
$sql=Get-Content -LiteralPath (Join-Path $docs 'actor-schemas-migration.sql') -Raw
if($sql -match '(?i)\b(DROP\s+TABLE|DROP\s+COLUMN|DELETE\s+FROM|TRUNCATE\s+TABLE)\b'){throw 'Destructive SQL rejected.'}
$sql=[regex]::Replace($sql,'(?im)^\s*GO\s*$','')
Add-Type -AssemblyName System.Data
$connection=New-Object System.Data.SqlClient.SqlConnection('Server=.\SQL2025;Database=HpuniEquipmentUi_20261104_0950;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;Pooling=False')
$transaction=$null
function Query([string]$text){
    $cmd=$connection.CreateCommand();$cmd.CommandText=$text;$cmd.CommandTimeout=120
    if($null -ne $transaction){$cmd.Transaction=$transaction}
    $table=New-Object System.Data.DataTable;$adapter=New-Object System.Data.SqlClient.SqlDataAdapter($cmd)
    try{[void]$adapter.Fill($table);return ,$table}finally{$adapter.Dispose();$cmd.Dispose()}
}
$report=[ordered]@{Backup=$backup.BackupPath;Tables=@();Committed=$false}
try{
    $connection.Open();$transaction=$connection.BeginTransaction([System.Data.IsolationLevel]::Serializable)
    $tables=Query 'SELECT t.name,t.object_id,s.name AS SchemaName FROM sys.tables t JOIN sys.schemas s ON s.schema_id=t.schema_id WHERE t.is_ms_shipped=0'
    if($tables.Rows.Count -ne 40){throw 'Unexpected source tables.'}
    foreach($row in $tables.Rows){
        $name=[string]$row.name
        if(-not $map.ContainsKey($name) -or $row.SchemaName -ne 'dbo'){throw 'Unexpected source mapping.'}
        $data=(Query "SELECT COUNT_BIG(*) AS Total,CHECKSUM_AGG(BINARY_CHECKSUM(*)) AS Digest FROM dbo.[$name] WITH (TABLOCKX,HOLDLOCK)").Rows[0]
        $report.Tables+=,[ordered]@{Table=$name;Schema=$map[$name];ObjectId=[int]$row.object_id;BeforeRows=[long]$data.Total;BeforeDigest=[string]$data.Digest}
    }
    [void](Query "CREATE SCHEMA QuanTriVien AUTHORIZATION dbo")
    [void](Query 'ALTER SCHEMA QuanTriVien TRANSFER dbo.LichSuCapNhatCauTruc')
    [void](Query $sql)
    foreach($entry in $report.Tables){
        $full='['+$entry.Schema+'].['+$entry.Table+']'
        $id=(Query ("SELECT OBJECT_ID(N'"+$full+"')")).Rows[0][0]
        if([int]$id -ne $entry.ObjectId){throw 'Object identity changed.'}
        $data=(Query "SELECT COUNT_BIG(*) AS Total,CHECKSUM_AGG(BINARY_CHECKSUM(*)) AS Digest FROM $full").Rows[0]
        $entry['AfterRows']=[long]$data.Total;$entry['AfterDigest']=[string]$data.Digest
        if($entry.Table -eq 'LichSuCapNhatCauTruc'){
            if($entry.AfterRows -ne $entry.BeforeRows+1){throw 'History row count mismatch.'}
        }elseif($entry.BeforeRows -ne $entry.AfterRows -or $entry.BeforeDigest -cne $entry.AfterDigest){throw 'Data mismatch.'}
    }
    $fk=Query 'SELECT name FROM sys.foreign_keys WHERE is_disabled=1 OR is_not_trusted=1'
    if($fk.Rows.Count -gt 0){throw 'Invalid foreign keys.'}
    $transaction.Commit();$report.Committed=$true
}catch{if($null -ne $transaction){try{$transaction.Rollback()}catch{}};$report['Error']=$_.Exception.Message;throw}
finally{$connection.Dispose();$report | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $docs 'actor-schemas-data-check-20261104.json') -Encoding UTF8}
