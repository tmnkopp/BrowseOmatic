 
function BOMInstaller {
	[CmdletBinding()]
	param(
		[Parameter()]
		[string] $Version,
		[Parameter()]
		[string] $InstallPath,
		[Parameter()]
		[bool] $WithUnitTest = $false 
	) 
    $v = $PSCmdlet.MyInvocation.BoundParameters["Verbose"].IsPresent

    $BOMURL=''
    if($Version -notmatch '\d{1,3}\.\d{1,3}' ){
        $HTML = Invoke-RestMethod 'https://github.com/tmnkopp/BrowseOmatic/releases/latest'
        $HTML -match '(/tmnkopp/BrowseOmatic/releases/download/.*/BOM.exe)'
        $BOMURL = "https://github.com" + $Matches[0] 
        $HTML -match '(/tmnkopp/BrowseOmatic/releases/download/.*/chromedriver.exe)'
        $CDURL = "https://github.com" + $Matches[0] 
    }else{ 
        $BOMURL="https://github.com" + '/tmnkopp/BrowseOmatic/releases/download/' + $Version + '/BOM.exe' 
    }  
    if($InstallPath -notmatch '\w{1}:.*\\' ){
        $InstallPath = 'c:\bom\' 
    }else{
        md -Force c:\bom\
    }

    if( $v ){
        Write-Host $BOMURL 
        Write-Host $CDURL
        Write-Host $InstallPath
    }

    [System.Environment]::SetEnvironmentVariable('bom', $InstallPath + 'bom.exe',[System.EnvironmentVariableTarget]::User)
    $webClient = [System.Net.WebClient]::new() 
    try {
       $WebClient.DownloadFile( $BOMURL , $InstallPath + "BOM.exe"  )
    }
    catch [System.Net.WebException],[System.IO.IOException] {
        "Unable to download bom.exe"
    }  
    if($WithUnitTest ){ 
        $exe = [System.Environment]::GetEnvironmentVariable('bom', 'User')  
        & $exe  cmd -t unittest -p config.yaml 
    } 

}
BOMInstaller -Version '~' -WithUnitTest $true



