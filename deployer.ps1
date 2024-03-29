﻿function Invoke-BOM-Workflow
{ 
    [CmdletBinding()]
        param ( 
        [Parameter(Mandatory = $false, Position = 0)] 
        [string] $with ,
        [Parameter(Mandatory = $false, Position = 1)] 
        [string] $message = 'refactor build '  
    )
    cd 'C:\Users\timko\source\repos\BrowseOmatic';   
    if ($with -match ' release ' ){
        try{
            taskkill /IM "BOM.exe" /F
        }catch{
            Write-Host 'BOM not killable'
        } 
        cd 'C:\Users\timko\source\repos\BrowseOmatic';  
        dotnet build --configuration Debug;
        dotnet build --configuration Release;
        dotnet publish BrowseOmatic -p:PublishProfile=FolderProfile   
        Copy-Item -Path C:\Users\timko\source\repos\BrowseOmatic\BrowseOmatic\bin\Release\netcoreapp3.1\BOM.exe -Destination c:\bom\BOM.exe -Force 
     
        Remove-Item -Path C:\Users\timko\source\repos\BrowseOmatic\BrowseOmatic\bin\Release\netcoreapp3.1\*.pdb -Force 
    }
    if ($with -match ' commit ' ){
        $m = -join ((65..90) + (97..122) | Get-Random -Count 2 | % {[char]$_ +''+ $_ })
        $message = 'update alert accept' + $m   
        cd 'C:\Users\timko\source\repos\BrowseOmatic';  
        git add .; git commit -m $message; git push; 
    }  
    # explorer.exe d:\dev\CyberScope\CyberScopeBranch\CSwebdev\test\bom
    explorer.exe C:\BOM   
} 
Invoke-BOM-Workflow -with " release commit  " 
cls; bom config;