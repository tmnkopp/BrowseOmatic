 
![alt text](https://the80port.com/cdn/logos/som75-4.png "som")
# Browse-O-matic
## BOM is a browser automator. Built on Selenium WebDriver technology, BOM simplifies automated testing by using YAML configuration files to configure test scripts. BOM is C# extendable; however, coding is not required to build and run automated test scripts.   

## Get Started

1. Create a bom environment variable (name = bom, path = x:\path\to\bom.exe)
2. Open command line
3. %bom% cmd -t unittest

## Configuration

Use `appsettings.json` to configure desired settings that involve passwords and connection information

Use `config.yaml` to configure test automation steps

## Even More

### Config.YAML 

`task` defines a test automation task

`context` associates the task to the appropriate browser driver context defined in the appsettings.json

`steps` defines a list of test automation steps

### YAML config commands

- `Click`: one argument - element to be clicked
- `Key`: 2 arguments - element and keys to send
- `Url`: one argument - the URL
- `SetOption`: 2 arguments - select element and index to select 
- `NaiveFormFill`: no arguments - fills a form
- `OpenTab`: one argument - URL to open


## YAML CONFIG
``` yaml
  - task: unittest
    context: bomdriver
    steps:
    - OpenTab: ['https://github.com/tmnkopp/BrowseOmatic']  
    - Click: ['body']
    - Key: ['SearchInput_PartialID_Or_PartialClass', 'My Search Test']
    - Click: ['SearchButton_PartialID_Or_PartialClass']
    - Click: ['AnotherButton_PartialID_Or_PartialClass']
    - Key: ['AnotherInput_PartialID_Or_PartialClass', 'My Test']
    - SetOption: ['Select_PartialID_Or_PartialClass', index]
    - Key: ['AnotherInput_PartialID_Or_PartialClass', 'My Test']
    - Key: ['AnotherInput_PartialID_Or_PartialClass', 'My Test']
    - Click: ['SaveButton_PartialID_Or_PartialClass']
```

