<# Kiểm tra HTTP không đăng nhập và PING ClamAV tùy chọn; không đổi cấu hình/SQL. #>
[CmdletBinding()]
param(
    [uri]$BaseUri = 'http://127.0.0.1:5080',
    [string]$ScannerHost,
    [ValidateRange(1,65535)][int]$ScannerPort = 3310,
    [ValidateRange(1,30)][int]$TimeoutSeconds = 10,
    [string]$ReportPath
)
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
if (!$BaseUri.IsAbsoluteUri -or $BaseUri.Scheme -notin @('http','https') -or
    $BaseUri.UserInfo -or $BaseUri.Query -or $BaseUri.Fragment -or $BaseUri.AbsolutePath -ne '/') {
    throw 'BaseUri phải là origin HTTP(S), không chứa thông tin đăng nhập, path hoặc query.'
}
$checks = @()
foreach ($path in @('/health/live','/health/ready','/login.html','/api/v1/auth/csrf')) {
    try {
        $r = Invoke-WebRequest -UseBasicParsing -Uri ($BaseUri.GetLeftPart([System.UriPartial]::Authority) + $path) -TimeoutSec $TimeoutSeconds -MaximumRedirection 0
        $headerOk = $r.Headers['X-Content-Type-Options'] -eq 'nosniff' -and $r.Headers['Referrer-Policy'] -eq 'same-origin'
        if ($path.StartsWith('/api/')) { $headerOk = $headerOk -and ($r.Headers['Cache-Control'] -match 'no-store') }
        $checks += [pscustomobject]@{name=$path;passed=($r.StatusCode -eq 200 -and $headerOk);status=[int]$r.StatusCode}
    } catch {
        # Không ghi exception thô chứa URL hoặc dữ liệu nhạy cảm vào báo cáo.
        $checks += [pscustomobject]@{name=$path;passed=$false;status='unavailable-or-redirect'}
    }
}
if ($ScannerHost) {
    if ($ScannerHost.Length -gt 253 -or $ScannerHost -match '[\s\x00-\x1f]') { throw 'ScannerHost không hợp lệ.' }
    $tcp = New-Object System.Net.Sockets.TcpClient
    try {
        $connect = $tcp.ConnectAsync($ScannerHost,$ScannerPort)
        if (!$connect.Wait($TimeoutSeconds*1000)) { throw 'Timeout' }
        $connect.GetAwaiter().GetResult()
        $stream = $tcp.GetStream()
        $stream.ReadTimeout = $TimeoutSeconds*1000
        $stream.WriteTimeout = $TimeoutSeconds*1000
        $ping = [System.Text.Encoding]::ASCII.GetBytes("zPING`0")
        $stream.Write($ping,0,$ping.Length)
        $response = New-Object System.Collections.Generic.List[byte]
        do {
            $b = $stream.ReadByte()
            if ($b -lt 0 -or $response.Count -ge 32) { throw 'Invalid scanner response' }
            if ($b -ne 0) { $response.Add([byte]$b) }
        } while ($b -ne 0)
        $checks += [pscustomobject]@{name='clamav-ping';passed=([System.Text.Encoding]::ASCII.GetString($response.ToArray()) -eq 'PONG');status='checked'}
    } catch {
        $checks += [pscustomobject]@{name='clamav-ping';passed=$false;status='unavailable-or-invalid'}
    } finally { $tcp.Dispose() }
}
$report = [pscustomobject]@{at=[DateTime]::UtcNow.ToString('o');checks=$checks;passed=(@($checks | Where-Object {!$_.passed}).Count -eq 0);scannerRequested=[bool]$ScannerHost}
$json = $report | ConvertTo-Json -Depth 5
if ($ReportPath) {
    $stream = [System.IO.File]::Open($ReportPath,[System.IO.FileMode]::CreateNew,[System.IO.FileAccess]::Write)
    try { $bytes = [System.Text.Encoding]::UTF8.GetBytes($json); $stream.Write($bytes,0,$bytes.Length) } finally { $stream.Dispose() }
}
$json
if (!$report.passed) { throw 'Có kiểm tra vận hành chưa đạt; xem báo cáo.' }
