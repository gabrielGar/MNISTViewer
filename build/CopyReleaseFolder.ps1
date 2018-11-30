[CmdletBinding()]
param(  
    [Parameter(Mandatory)][string] $containerName,
    [Parameter(Mandatory)][string] $destinationFolder,
    [Parameter(Mandatory)][string] $connectionString
)

$storage_account = New-AzureStorageContext -ConnectionString $connectionString
$blobs = Get-AzureStorageBlob -Container $containerName -Context $storage_account
Write-Output  "Found $($blobs.count) blob(s)"

New-Item -ItemType Directory -Force -Path $destinationFolder

Write-Output  "Downloading to $destinationFolder"


$blobs = Get-AzureStorageBlob -Container $containerName -Context $storage_account

if($blobs.Length -gt 0) {
    $releasesFile = Get-AzureStorageBlob `
                    -Container $containerName `
                    -Context $storage_account `
                    -Blob "RELEASES"

    # Download RELEASES file
    $blobContent = Get-AzureStorageBlobContent `
            -Container $containerName `
            -Blob $releasesFile.Name `
            -Destination $destinationFolder `
            -Context $storage_account `
            -Force

    Write-Output $releasesFile.Name

    # Download latest *-full.nupkg
    $nupkgs = Get-AzureStorageBlob `
                -Container $containerName `
                -Context $storage_account `
                -Blob "*-full.nupkg" | Sort-Object LastModified -descending

    if($nupkgs.Count -gt 0) {
        $blobContent = Get-AzureStorageBlobContent `
            -Container $containerName `
            -Blob $nupkgs[0].Name `
            -Destination $destinationFolder `
            -Context $storage_account `
            -Force

        Write-Output $nupkgs[0].Name
    }
}

# write out final stuffl
$items = Get-ChildItem -Path $destinationFolder
Write-Output "Final Contents:"

foreach ($item in $items) {
    Write-Output $item.FullName
}