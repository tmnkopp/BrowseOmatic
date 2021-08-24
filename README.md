 
###### Browse-O-Matic Readme 
![alt text](https://the80port.com/cdn/logos/som75-4.png "bom") 
# Browse-O-Matic 
### BOM automates browsers. Built on Selenium WebDriver technology, BOM simplifies automated testing by using a combination of YAML configuration and extendable C# to configure test scripts. No code/low code supported - coding is not required to build and run automated test scripts. Extendable libraries support custom coded execution routines. Support for both naive form auto-fillers and direct access to form elements allows for flexible automation procedures. Multi-environment support (Production, Stage, Dev), naive form value detection, and more are configurable using JSON.  

***

#### Browse-O-matic is a member of the "O-Matics" family, a suite of coding projects actively maintained by a busy but dedicated coder determined to unsuck sucky coding tasks.  

![alt text](https://the80port.com/cdn/logos/som.png "som") [Sledge-O-Matic](https://github.com/tmnkopp/sledgeomatic) (takes the suck out of code generation) 

![alt text](https://the80port.com/cdn/logos/som4.png "som") [Pie-O-Matic](https://github.com/tmnkopp/PieOMatic) (Sledge-O-Matic for Python) 

***
## Get Started

1. [Get Latest Release](https://github.com/tmnkopp/BrowseOmatic/releases)
2. Create a bom environment variable (name = bom, path = c:\bom\bom.exe); Add PATH c:\bom\ (optional)
3. bom run -t unittest

### Or use Powershell

1. Run installer.ps1
bominstaller will install bom dependencies and keep your bom up-to-date with the latest release.


***
## Configuration

[Get Config Starter Files](https://github.com/tmnkopp/BrowseOmatic/releases)

Use `appsettings.json` to configure bom settings. Here you can configure different automation environments (dev, stage, prod), browser drivers (chrome, firefox), automation task file locations (c:\My-Staging-Test-Procedures.yaml, c:\Fill-My-Timesheets.yaml, c:\Update-My-JIRA-Stuff.yaml), and more. 

Use `appsettings.json` to configure login routines that involve passwords and connection information.  

Use `config.yaml` files to configure test automation steps.

### Config.YAML 


`task` defines a test automation task 

`context` associates the task to the appropriate context defined in the appsettings.json 

`taskSteps` defines a list of test automation steps
 
 
### Browser Driver Commands (just a few)

- `Click`: one argument - element to be clicked
- `ClickByContent`: element container, content/regex pattern, true/false (is regex)
- `Key`: 2 arguments - element and keys to send
- `Url`: one argument - the URL
- `Script`: one argument - Javascript
- `SetOption`: 2 arguments - select element and index to select 
- `NaiveFormFill`: one argument - container
- `OpenTab`: one argument - URL to open
- `Pause`: one argument - int Milliseconds  
- `SetWait`: one argument - int Pause between next steps


## YAML CONFIG

``` yaml

tasks:
  - task: unittest
    context: bomdriver
    taskSteps:   
    - { cmd: OpenTab, args: [ 'http://automationpractice.com/index.php?controller=authentication&back=identity'  ]}
    - { cmd: Key, args: [ 'email_create', 'TestyMcTestFace@Domain.com'  ]}
    - { cmd: Click, args: [ 'SubmitCreate'  ]}
    - { cmd: Pause, args: [ 2000 ]}
    - { cmd: SetWait, args: [ '50'   ]}
    - { cmd: NaiveFormFill, args: [ ''    ]}
    - { cmd: OpenTab, args: [ 'https://github.com/tmnkopp/BrowseOmatic#browse-o-matic-readme']}
    - { cmd: Script, args: [  alert('Success\n\nUnit Test Complete'); ]}

```


``` yaml
  - task: how_fluent_selectors_work
    context: bomdriver
    taskSteps:    
    - { cmd: Key, args: [ 'PartialID_Or_PartialClass_Or_PartialName', 'My Input Text' ]}
    - { cmd: Click, args: [ 'Element_Selector_Is_Fluent_Will_Search_Attributes_For_Best_Match' ]}   
    - { cmd: SetOption, args: ['dropdown_id', 2]}
    - { cmd: Script, args: [  "alert(' javascript goes here ');" ]} 
```
***


## Appsettings.JSON CONFIG

Customize naive form auto-fill using regular expressions

``` json

  "NaiveInputDefaults": {
    "(email)": "UseyMcUseFace@Domain.com",
    "(phone)": "555-123-4567",
    "(address)": "123 Anywhere USA",
    "(city)": "Washington",
    "(state)": "DC",
    "(zip|postal)": "20000",
    "(firstname|fname|name)": "Testy",
    "(lastname|lname|name)": "McTestFace",
    "(search|query)": "SEARCH TERM",
    "(ipaddress)": "255.255.255.255"
  }

```
### Multi-Enviornment Configuration

``` json

    {
      "name": "dev",
      "conn": "driver:BOM.CORE.SessionDriver, BOM.CORE;https://DEV_Enviornment.com/;s:UserName,MyLogin;s:Password,MyLogin;c:LoginButton;"
    },
    {
      "name": "stage",
      "conn": "driver:BOM.CORE.SessionDriver, BOM.CORE;https://DEV_STAGE.com/;s:UserName,MyLogin;s:Password,MyLogin;c:LoginButton;"
    },
    {
      "name": "prod",
      "conn": "driver:BOM.CORE.SessionDriver, BOM.CORE;https://DEV_PRODUCTION.com/;s:UserName,MyLogin;s:Password,MyLogin;c:LoginButton;"
    },

```
***
###   Extending ICommand to expose ISessionContext for access to browser driver commands

#### 
#### 
``` csharp
namespace BOM.CORE
{
    public interface ICommand
    {
        public void Execute(ISessionContext SessionContext);
    }
    public class MyCustomAutomator : ICommand
    {
        private string MyConfigArg = "";
        private int MyConfiArg2 = 0;
        public MyCustomAutomator(string MyConfigArg, int MyConfiArg2)
        {
            this.MyConfigArg = MyConfigArg;
            this.MyConfiArg2 = MyConfiArg2;
        }
        public void Execute(ISessionContext SessionContext)
        {
            var ctx = SessionContext;
            ctx.SessionDriver
                .GetUrl($"http://domainname.com/page-number/{MyConfiArg2.ToString()}")
                .Click("Element")
                .SendKeys("Element", "Type This") 
                .Click("Element")
                .SendKeys($"{MyConfigArg}", $"Type This"); 
        }
    }
}
```


***

Like Browse-O-matic? 

Check out [Sledge-O-Matic](https://github.com/tmnkopp/sledgeomatic)


 
