param(
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $root "KeyTracker.sln"
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

Write-Host "==> Running tests with coverage"
dotnet test $solution --collect:"XPlat Code Coverage"
if ($LASTEXITCODE -ne 0) {
    throw "dotnet test failed with exit code $LASTEXITCODE"
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
