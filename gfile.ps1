[cmdletbinding()]
param(
    [Parameter(Position=0)]
    [ValidateSet('Release','Debug')]
    [string]$configuration = 'Release',

    [Parameter(Position=1)]
    [System.IO.DirectoryInfo]$outputRoot
)

function InternalGet-ScriptDirectory{
    split-path (((Get-Variable MyInvocation -Scope 1).Value).MyCommand.Path)
}
$scriptDir = ((InternalGet-ScriptDirectory) + "\")

[System.IO.FileInfo]$slnPath = (Join-Path $scriptDir vsfolders.sln)

if($outputRoot -eq $null){
    $outputRoot = (Join-Path $scriptDir 'OutputRoot')
}

#[System.IO.DirectoryInfo]$outputRoot = (

task default -dependsOn build

task init {
    requires 'https://raw.githubusercontent.com/ligershark/psbuild/master/src/GetPSBuild.ps1' -condition (-not (Get-Command -Module psbuild -Name Invoke-MSBuild -ErrorAction SilentlyContinue) )
    requires 'https://raw.githubusercontent.com/ligershark/nuget-powershell/master/get-nugetps.ps1' -condition (-not (Get-Command -Module nuget-powershell -Name Get-NuGetPackage -ErrorAction SilentlyContinue) )

    # ensure output root exists
    if(-not (Test-Path $outputRoot)){
        'Creating outputroot folder at [{0}]' -f $outputRoot | Write-Verbose
        New-Item -ItemType Directory -Path $outputRoot
    }
}

task build {
    Invoke-MSBuild -projectsToBuild $slnPath -configuration $configuration -outputPath $outputRoot
} -dependsOn restorepackages

task restorepackages{
    Push-Location
    try{
        Set-Location ($slnPath.Directory.FullName)
        &(Get-Nuget) restore
    }
    finally{
        Pop-Location
    }
}

task clean{
    if($outputRoot -ne $null -and (-not ([string]::IsNullOrWhiteSpace( $outputRoot.FullName ))) ) {
        Remove-Item -Path $outputRoot -Recurse
    }
}