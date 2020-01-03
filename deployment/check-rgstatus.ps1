param (
    [Parameter(Mandatory)]
    $key,
    [Parameter(Mandatory)]
    [array]$filesToHash,
    [Parameter(Mandatory=$false)]
    [string]$additionalInputs,
    $azFunctionPrefix = 'https://armstate20191214083323.azurewebsites.net/api',
    $azFunctionApiKey = 'S/qifih5rm6Wea8VzggQLU0Ys8ibpsIlfW1OSPiCo44siF1rnAFkSQ=='
)
$ErrorActionPreference = 'stop'
if ($filesToHash.Count -eq 0) {
    Write-Error 'List of files to hash is empty.'
}

$inputValue = "$($filesToHash | ForEach-Object { Get-Content $_ } | Out-String)$additionalInputs"
$uri = "$azFunctionPrefix/CheckState?key=$key&code=$azFunctionApiKey"
$statusCheck = Invoke-RestMethod -Method Post -Uri $uri -Body $inputValue -ErrorAction SilentlyContinue 

if ($statusCheck.state -eq 'unchanged') {
    Write-host 'ARM template and parameters file are unchanged. Setting noprovision variable.'
    Write-output "##vso[task.setvariable variable=noprovision]true"
} else {
    Write-host 'Detected change in ARM template. Do nothing.'
}
