param (
    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string]
    $OutputPath = '.\bin\v2rayN'
)

Write-Host 'Building'

dotnet publish `
    .\v2rayN\v2rayN.csproj `
    -c Release `
    -r win-x64 `
    --self-contained false `
    -p:PublishReadyToRun=false `
    -p:PublishSingleFile=true `
    -o "$OutputPath\win-x64"

dotnet publish `
    .\v2rayN.Desktop\v2rayN.Desktop.csproj `
    -c Release `
    -r linux-x64 `
    --self-contained true `
    -p:PublishReadyToRun=false `
    -p:PublishSingleFile=true `
    -o "$OutputPath\linux-x64"

if (-Not $?) {
    exit $lastExitCode
}

if (Test-Path -Path .\bin\v2rayN) {
    rm -Force "$OutputPath\win-x64\*.pdb"
    rm -Force "$OutputPath\linux-x64\*.pdb"
}

Write-Host 'Build done'

# Создаем два отдельных архива
7z a "$OutputPath\v2rayN-windows-64.zip" "$OutputPath\win-x64\*"
7z a "$OutputPath\v2rayN-linux-64.zip" "$OutputPath\linux-x64\*"

exit 0