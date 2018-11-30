[CmdletBinding()]
param(  
    [Parameter(Mandatory)][string] $containerName,
    [Parameter(Mandatory)][string] $sourceFolder,
    [Parameter(Mandatory)][string] $connectionString
)

$storage_account = New-AzureStorageContext -ConnectionString $connectionString

foreach ($localFile in Get-ChildItem -Path $sourceFolder)  {
    Write-Output "$($localFile.FullName)"
    Set-AzureStorageBlobContent `
        -File $localFile.FullName `
        -Container $containerName `
        -Blob $localFile.Name `
        -Context $storage_account `
        -Force
}
