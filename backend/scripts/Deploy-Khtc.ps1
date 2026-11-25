$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$statePath=Join-Path $root 'docs/vercel-tunnel-state.json'
$state=Get-Content -Raw -Encoding UTF8 -LiteralPath $statePath|ConvertFrom-Json
$artifact=Join-Path $root 'artifacts/khtc-live-20261125'
$current=Get-CimInstance Win32_Process -Filter "ProcessId=$($state.ApiPid)"
if(!$current -or $current.Name -ne 'dotnet.exe' -or $current.CommandLine -notmatch 'Hpuni.Api.dll.*5083'){throw 'API process does not match deployment state.'}
if(!(Test-Path -LiteralPath (Join-Path $artifact 'Hpuni.Api.dll'))){throw 'Missing KHTC artifact.'}
$stamp=Get-Date -Format 'yyyyMMdd-HHmmss'
Copy-Item -LiteralPath $statePath -Destination (Join-Path $root "docs/vercel-tunnel-state-before-khtc-$stamp.json")
$env:ASPNETCORE_ENVIRONMENT='Production'
$env:ConnectionStrings__Hpuni="Server=.\SQL2025;Database=$($state.Database);Integrated Security=True;Encrypt=True;TrustServerCertificate=True"
$env:DataProtection__Path=Join-Path $root 'runtime/tunnel-keys'
$env:ReverseProxy__Enabled='true'
$env:ReverseProxy__KnownProxies__0='127.0.0.1'
$env:ReverseProxy__ForwardLimit='1'
$env:AllowedHosts='localhost;127.0.0.1'
$env:HTTPS_PORT='443'
$env:Logging__EventLog__LogLevel__Default='None'
& dotnet (Join-Path $artifact 'Hpuni.Api.dll') --ensure-khtc *> (Join-Path $root "runtime/khtc-role-$stamp.log")
if($LASTEXITCODE -ne 0){throw 'KHTC role provisioning failed; current API unchanged.'}
Stop-Process -Id $state.ApiPid
Wait-Process -Id $state.ApiPid -Timeout 15 -ErrorAction SilentlyContinue
$log=Join-Path $root "runtime/khtc-api-$stamp.log"
$process=Start-Process dotnet -ArgumentList 'Hpuni.Api.dll --urls http://127.0.0.1:5083' -WorkingDirectory $artifact -WindowStyle Hidden -PassThru -RedirectStandardOutput $log -RedirectStandardError (Join-Path $root "runtime/khtc-api-$stamp.err.log")
$state.ApiPid=$process.Id;$state.Artifact='backend/artifacts/khtc-live-20261125';$state.ApiLog=$log
$state|ConvertTo-Json|Set-Content -Encoding UTF8 -LiteralPath $statePath
Write-Output "KHTC API PID: $($process.Id); gateway/tunnel unchanged."
