param (
    [Parameter(Mandatory)]
    $key,
    [Parameter(Mandatory)]
    [array]$filesToHash,
    $azFunctionPrefix = 'https://armstate20191214083323.azurewebsites.net/api',
    $azFunctionApiKey = 'S/qifih5rm6Wea8VzggQLU0Ys8ibpsIlfW1OSPiCo44siF1rnAFkSQ=='
)
$ErrorActionPreference = 'stop'
if ($filesToHash.Count -eq 0) {
    Write-Error 'List of files to hash is empty.'
}

$inputValue = $filesToHash | ForEach-Object { Get-Content $_ } | Out-String
$uri = "$azFunctionPrefix/UpdateState?key=$key&code=$azFunctionApiKey"
Invoke-RestMethod -Method Post -Uri $uri -Body $inputValue -ErrorAction SilentlyContinue
if (-not($?)) {
    Write-Error 'Could not update ARM template state.'
}