param($siteUrl= $(Read-Host "Enter your SharePoint site"))

$dir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$name = "onlyoffice.wsp"
$path = "$dir\$name"
$featureId = "bb60a219-eb5d-47ff-b023-353a3be7b8ee"

if(!(Get-PSSnapin Microsoft.SharePoint.Powershell -ErrorAction:SilentlyContinue))
{
    Add-PSSnapin Microsoft.SharePoint.Powershell
}

$solution = Get-SPSolution $name -ErrorAction:SilentlyContinue
if(!$solution)
{
    $solution = Add-SPSolution -LiteralPath $path -Confirm:$false
}

if($solution.Deployed -eq $false)
{
    Install-SPSolution $solution -GACDeployment -Confirm:$false
    while($solution.Deployed -eq $false)
    {
    Write-Host '.' -NoNewline
        sleep -s 1
    }
}
if(!(Get-SPFeature $featureId -Web $siteUrl -ErrorAction:SilentlyContinue)) #or -Site, -WebApplication, -Farm
{
    Enable-SPFeature $featureId -Url $siteUrl
}
Write-Host ''