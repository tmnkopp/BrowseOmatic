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
        try{
            taskkill /IM "BOM.exe" /F
        }catch{
            Write-Host 'BOM not killable'
        }
        
        cd 'C:\Users\Tim\source\repos\BrowseOmatic';  
        dotnet build --configuration Debug;
        dotnet build --configuration Release;
        dotnet publish BrowseOmatic -p:PublishProfile=FolderProfile   
        Copy-Item -Path C:\Users\Tim\source\repos\BrowseOmatic\BrowseOmatic\bin\publish\BOM.exe -Destination c:\bom\BOM.exe -Force 
        Copy-Item -Path C:\Users\Tim\source\repos\BrowseOmatic\bominstaller.ps1 -Destination c:\bom\bominstaller.ps1 -Force 
        Copy-Item -Path C:\Users\Tim\source\repos\BrowseOmatic\bominstaller.ps1 -Destination C:\Users\Tim\source\repos\BrowseOmatic\BrowseOmatic\bin\publish\installer.ps1 -Force 
        Remove-Item -Path C:\Users\Tim\source\repos\BrowseOmatic\BrowseOmatic\bin\publish\*.pdb -Force
        
    }
    if ($with -match ' commit ' ){
        $m = -join ((65..90) + (97..122) | Get-Random -Count 2 | % {[char]$_ +''+ $_ })
        $message = 'resolves #9' #$message + $m   
        cd 'C:\Users\Tim\source\repos\BrowseOmatic';  
        git add .; git commit -m $message; git push;
        #Write-Host 'foo'
    } 
    
    explorer.exe C:\Users\Tim\source\repos\BrowseOmatic\BrowseOmatic\bin\publish\
    # explorer.exe C:\BOM commit
} 
Invoke-BOM-Workflow -with " release  " 

