function Invoke-BOM-Workflow
{ 
    [CmdletBinding()]
        param ( 
        [Parameter(Mandatory = $false, Position = 0)] 
        [string] $with ,
        [Parameter(Mandatory = $false, Position = 0)] 
        [string] $message = 'refactor build '  
    )
    cd 'C:\Users\Tim\source\repos\BrowseOmatic';   
    if ($with -match ' release ' ){
        dotnet build --configuration Debug;
        dotnet build --configuration Release;
        dotnet publish -p:PublishProfile=FolderProfile   
        Copy-Item -Path C:\Users\Tim\source\repos\BrowseOmatic\BrowseOmatic\bin\publish\BOM.exe -Destination c:\bom\BOM.exe -Force 
    }
    if ($with -match ' release | commit ' ){
        cd 'C:\Users\Tim\source\repos\BrowseOmatic'; 
        $m = -join ((65..90) + (97..122) | Get-Random -Count 2 | % {[char]$_ +''+ $_ })
        $message = $message + $m  
        git add .; git commit -m $message; git push;
        #Write-Host 'foo'
    } 
} 
Invoke-BOM-Workflow -with " commit release " 

