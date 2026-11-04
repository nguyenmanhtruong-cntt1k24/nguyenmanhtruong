param([string]$BaseUrl = 'http://127.0.0.1:5080')
$ErrorActionPreference = 'Stop'
$results = [System.Collections.Generic.List[object]]::new()
function Check-Status([string]$Path, [int]$Expected, [string]$Method = 'GET', [string]$Body = '') {
    $params = @{ Uri = $BaseUrl.TrimEnd('/') + $Path; Method = $Method; UseBasicParsing = $true }
    if ($Body) { $params.Body = $Body; $params.ContentType = 'application/json' }
    try { $status = [int](Invoke-WebRequest @params).StatusCode }
    catch {
        if (-not $_.Exception.Response) { throw }
        $status = [int]$_.Exception.Response.StatusCode
    }
    $results.Add([pscustomobject]@{ method = $Method; path = $Path; expected = $Expected; actual = $status; passed = $status -eq $Expected })
    if ($status -ne $Expected) { throw "HTTP check failed: $Method $Path expected=$Expected actual=$status" }
}
Check-Status '/health/live' 200
Check-Status '/health/ready' 200
Check-Status '/openapi/v1.json' 200
Check-Status '/api/v1/auth/csrf' 200
Check-Status '/api/v1/auth/me' 401
Check-Status '/api/v1/audit-events' 401
Check-Status '/api/v1/auth/login' 400 'POST' '{"userName":"nonexistent.test","password":"NotRealAccount123"}'
$schema = Invoke-RestMethod -Uri ($BaseUrl.TrimEnd('/') + '/openapi/v1.json')
$report = [pscustomobject]@{
    at = [DateTimeOffset]::UtcNow.ToString('o')
    baseUrl = $BaseUrl
    checks = $results
    openapiPathCount = @($schema.paths.PSObject.Properties).Count
}
$file = Join-Path (Split-Path $PSScriptRoot -Parent) 'docs/smoke-http.json'
[IO.File]::WriteAllText($file, ($report | ConvertTo-Json -Depth 5), [Text.UTF8Encoding]::new($false))
Write-Output ($report | ConvertTo-Json -Depth 5)
