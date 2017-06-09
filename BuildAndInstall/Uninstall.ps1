param($siteUrl= $(Read-Host "Enter your SharePoint site"))
$name = "onlyoffice.wsp"
$featureId = "bb60a219-eb5d-47ff-b023-353a3be7b8ee"

if(!(Get-PSSnapin Microsoft.SharePoint.Powershell -ErrorAction:SilentlyContinue))
{
    Add-PSSnapin Microsoft.SharePoint.Powershell
}
if((Get-SPFeature $featureId -Web $siteUrl -ErrorAction:SilentlyContinue)) #or -Site, -WebApplication, -Farm
{
   Uninstall-SPFeature $featureId -Confirm:$false -force
}

$solution = Get-SPSolution $name -ErrorAction:SilentlyContinue
if($solution.Deployed -eq $true)
{
    Uninstall-SPSolution -Identity $solution -Confirm:$false
    while($solution.Deployed -eq $true)
    {
    Write-Host '.' -NoNewline
        sleep -s 1
    }
    while($solution.JobExists)
    {
    Write-Host '.' -NoNewline
        sleep -s 1
    }
    Remove-SPSolution -Identity $solution -Confirm:$false -force
    Write-Host ''
}

