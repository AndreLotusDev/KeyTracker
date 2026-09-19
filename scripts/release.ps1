param(
    [Parameter(Mandatory = $true)]
    [string]$Version,
    [switch]$SkipE2E
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$unitTestProject = Join-Path $root "tests\KeyTracker.Tests\KeyTracker.Tests.csproj"
$e2eTestProject = Join-Path $root "tests\KeyTracker.E2E\KeyTracker.E2E.csproj"
$appProject = Join-Path $root "src\KeyTracker.App\KeyTracker.App.csproj"
$installerScript = Join-Path $root "installer\setup.iss"
$distDir = Join-Path $root "dist\$Version"
$publishDir = Join-Path $root "publish-temp"

function Find-InnoSetupCompiler {
    $candidates = @(
        "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
        "${env:ProgramFiles}\Inno Setup 6\ISCC.exe",
        "${env:LocalAppData}\Programs\Inno Setup 6\ISCC.exe"
    )
    foreach ($candidate in $candidates) {
        if (Test-Path $candidate) { return $candidate }
    }
    $onPath = Get-Command "ISCC.exe" -ErrorAction SilentlyContinue
    if ($onPath) { return $onPath.Source }
    throw "Inno Setup compiler (ISCC.exe) not found. Install Inno Setup 6: https://jrsoftware.org/isdl.php"
}

Write-Host "==> Running unit tests with coverage"
dotnet test $unitTestProject --collect:"XPlat Code Coverage"
if ($LASTEXITCODE -ne 0) {
    throw "dotnet test failed with exit code $LASTEXITCODE"
}

if ($SkipE2E) {
    Write-Warning "Skipping e2e tests (-SkipE2E)."
} else {
    Write-Host "==> Running e2e tests"
    dotnet test $e2eTestProject
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet test (e2e) failed with exit code $LASTEXITCODE"
    }
}

Write-Host "==> Publishing $Version"
if (Test-Path $publishDir) { Remove-Item $publishDir -Recurse -Force }
dotnet publish $appProject -c Release -r win-x64 --self-contained true `
    -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:Version=$Version -o $publishDir
if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed with exit code $LASTEXITCODE"
}

New-Item -ItemType Directory -Force -Path $distDir | Out-Null

$publishedExe = Join-Path $publishDir "KeyTracker.exe"
$versionedExe = Join-Path $distDir "KeyTracker-$Version.exe"
Copy-Item $publishedExe $versionedExe -Force

Write-Host "==> Compiling installer"
$iscc = Find-InnoSetupCompiler
& $iscc "/DAppVersion=$Version" "/DSourceExe=$versionedExe" $installerScript
if ($LASTEXITCODE -ne 0) {
    throw "Inno Setup compilation failed with exit code $LASTEXITCODE"
}

Remove-Item $publishDir -Recurse -Force

Write-Host "==> Done: dist\$Version"
Get-ChildItem $distDir | ForEach-Object { Write-Host " - $($_.Name)" }
