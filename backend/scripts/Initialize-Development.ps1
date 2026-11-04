param([switch]$Bootstrap)
$ErrorActionPreference = 'Stop'
$previousEnvironment = $env:ASPNETCORE_ENVIRONMENT
$previousPassword = $env:Bootstrap__Password
Push-Location (Split-Path $PSScriptRoot -Parent)
try {
    $env:ASPNETCORE_ENVIRONMENT = 'Development'
    dotnet restore Hpuni.sln --locked-mode
    if ($LASTEXITCODE -ne 0) { throw 'Restore failed.' }
    dotnet build Hpuni.sln --no-restore
    if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
    dotnet run --project src/Hpuni.Api --no-build -- --migrate
    if ($LASTEXITCODE -ne 0) { throw 'Migration failed.' }
    if ($Bootstrap) {
        if ([string]::IsNullOrWhiteSpace($env:Bootstrap__Password)) {
            $secure = Read-Host 'Mat khau admin (12-128 ky tu, co chu va so)' -AsSecureString
            $pointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secure)
            try { $env:Bootstrap__Password = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($pointer) }
            finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($pointer) }
        }
        dotnet run --project src/Hpuni.Api --no-build -- --bootstrap
        if ($LASTEXITCODE -ne 0) { throw 'Bootstrap failed; existing data was not reset.' }
        Write-Host 'Da tao admin.hpuni. Khong co mat khau mac dinh.'
    }
}
finally {
    $env:Bootstrap__Password = $previousPassword
    $env:ASPNETCORE_ENVIRONMENT = $previousEnvironment
    Pop-Location
}
