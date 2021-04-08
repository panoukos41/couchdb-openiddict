[CmdletBinding()]
param (
    [Parameter(Mandatory)]
    [string]
    $url
)

curl -X POST -H "Content-Type: application/json" -d "@ddoc.json" $url