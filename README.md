# BrowseOmatic

## BOM automates browsers. Built on Selenium WebDriver technology, BOM simplifies automated testing by using YAML configuration files to configure test scripts. BOM is C# extendable; however, coding is not required to build and run automated test scripts.   

## Get Started

1. Create a bom environment variable (name = bom, path = x:\path\to\bom.exe)
2. Open command line
3. %bom% cmd -t unitest

## More

Use appsettings.json to configure desired settings that involve passwords

Use config.yaml to configure test automation steps

## Even More

### config.yaml 

- **task** defines a test automation task

- **context** associates the task to the appropriate browser driver context defined in the appsettings.json

- **steps** defines a list of test automation steps

### config yaml commands

- Click: one argument - element to be clicked
- Key: 2 arguments - element and keys to send
- Url: one argument - the URL
- SetOption: 2 arguments - select element and index to select 
- NaiveFormFill: no arguments - fills a form
- OpenTab: one argument - URL to open
