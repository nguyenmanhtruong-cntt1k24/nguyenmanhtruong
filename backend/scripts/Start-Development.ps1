$ErrorActionPreference = 'Stop'
$previousEnvironment = $env:ASPNETCORE_ENVIRONMENT
Push-Location (Split-Path $PSScriptRoot -Parent)
try {
    $env:ASPNETCORE_ENVIRONMENT = 'Development'
    dotnet run --project src/Hpuni.Api --urls http://127.0.0.1:5080
    if ($LASTEXITCODE -ne 0) { throw 'Backend stopped with an error.' }
}
finally {
    $env:ASPNETCORE_ENVIRONMENT = $previousEnvironment
    Pop-Location
}
