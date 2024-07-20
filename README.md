# Exchange Rates Updater
Exchange Rates Updater is a web application built as a Windows service. It automatically downloads files containing the current currency exchange rates, based on which it updates data on the web page. Everything is fully configurable, allowing easy customization of update intervals, execution times, target servers, and the data source from which the application fetches data.

***

# Features
-Fully Autonomous: CBS Exchange Rates Updater is fully self-contained. Simply install and run the application, and it will handle authorization tokens required for server authorization and automatically update data according to the configured settings.
-Automated Data Updates: The application automatically fetches and updates currency exchange rate data based on configured settings.
-Configurability: Exchange Rates Updater is highly configurable, allowing users to adjust update intervals, execution times, target servers, and data sources.
-Flexible Deployment: Being a Windows service, the application can be deployed easily and run in the background without user intervention, or initialized as console app.

***

# Getting Started
To get started with Exchange Rate Updater, follow these steps:
1) Open Windows PowerShell as Administrator
2) Change directory to:
	\YourDirectoryPath\CBSExchangeRatesUpdater\bin\Release\net8.0
3) Run this command:
	.\CBSExchangeRatesUpdater.exe install
	App will be installed as windows service named ExchangeRatesUpdater
4) To run the service, run this command:
	.\CBSExchangeRatesUpdater.exe start
	or start it manually in Services.

To change configuration, go to: \Path\to\the\direcoty\Autorestart\bin\Release\net8.0\ and open the CONFIG file: CBSExchangeRatesUpdater.dll.config, when change is made and file is saved, Windows Service will restart itself automatically.
- Logs are saved here:  \YourDirectoryPath\CBSExchangeRatesUpdater\logs\Log_[COUNTRY NAME]_[dd.MM.yyyy]_[HH-mm].txt
- Currency configuration is located here: \YourDirectoryPath\CBSExchangeRatesUpdater\Resources\CurrencyProperties.csv
- Newly downloaded data from central bank are located here: \YourDirectoryPath\CBSExchangeRatesUpdater\Resources\exchangeRates\exchange_rates_[COUNTRY NAME]_[dd.MM.yyyy]_[HH-mm].csv


***

# Application Flow
1) Application will schedule it's launch time based on setting in Application.dll.config and calculate wait time between scheduled tasks. 
2) Application sends request for authorization token from CBS, that is required to upload data later in the process.
3) Application starts downloading/extracting data from configured URL and saves them to: \YourDirectoryPath\CBSExchangeRatesUpdater\Resources\exchangeRates\exchange_rates_[COUNTRY NAME]_[dd.MM.yyyy]_[HH-mm].csv"
4) Application reads downloaded data and if needed, convert them to proper format (CurrencyName,ValidityDate,Rate,Multiplier)
5) Application parses values from downloaded file and compare them with configurated settings. If the value meet the conditions, application filter values to currencies with and 
   without records, otherwise the currency is set as invalid currency.
6) Application uploads values to CBS one by one to configured URL using authorization token and returns status of this action.
7) Application returns statistics of successfull and unsuccessfull updates.
8) Application is waiting until scheduled repetetion, or restart.

***

# Technologies Used
C# .NET
Topshelf
Selenium
HtmlAgilityPack
TinCsvParser
CsvHelper
Serilog
Newtonsoft.Json
Task.Scheduler