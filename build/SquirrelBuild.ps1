[CmdletBinding()]
param(  
    [Parameter(Mandatory)][string] $nugetPackage,
    [Parameter(Mandatory)][string] $releaseFolder
)

# Find squirrel.exe
$squirrel = Get-ChildItem -Recurse -Filter *.exe | ?{ !$_.PSIsContainer -and [System.IO.Path]::GetFileNameWithoutExtension($_.Name) -eq "Squirrel"} | % { $_.FullName }
if(!$squirrel)  
{
    Write-Error "Unable to find Squirrel.exe"
    exit 1
}
$package = Resolve-Path $nugetPackage

Write-Output "Found Squirrel Installation. Using $squirrel"
Write-Host "Starting Squirrel releasify on" $package "release directory:" $releaseFolder

& $squirrel --releasify $package --releaseDir $releaseFolder | Write-Output

Write-Output "Complete!"