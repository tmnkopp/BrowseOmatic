param ( 
    [Parameter(Mandatory = $false, Position = 0)] 
    [string] $ConfigurationName ,
    [Parameter(Mandatory = $false, Position = 0)] 
    [string] $m = 'refactor build '  
)
Write-Host $ConfigurationName

if ($ConfigurationName -match 'Release' ){
    cd 'C:\Users\Tim\source\repos\BrowseOmatic\BrowseOmatic';    
    dotnet publish -p:PublishProfile=FolderProfile   
    Copy-Item -Path C:\Users\Tim\source\repos\BrowseOmatic\BrowseOmatic\bin\publish\BOM.exe -Destination c:\bom\BOM.exe -Force 
    Copy-Item -Path C:\Users\Tim\source\repos\BrowseOmatic\bominstaller.ps1 -Destination c:\bom\bominstaller.ps1 -Force 
     
}  