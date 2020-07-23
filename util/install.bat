REM USAGE: Install.bat <Debug/Release> <VSProjectFolderPath> <UUID/AssemblyName>
REM Example: Install.bat DEBUG C:\Projects\SuperMacro com.barraider.SuperMacro
setlocal

REM *** MAKE SURE THE FOLLOWING VARIABLES ARE CORRECT ***
REM Distribution tool be downloaded from: https://developer.elgato.com/documentation/stream-deck/sdk/exporting-your-plugin/
REM Place Distribution tool in the same directory as this batch file
SET STREAM_DECK_DISTRIBUTION_TOOL=%~dp0DistributionTool.exe
SET STREAM_DECK_APP="C:\Program Files\Elgato\StreamDeck\StreamDeck.exe"
SET STREAM_DECK_LOAD_TIMEOUT_SECONDS=4


REM Setup build variables
SET BUILD_MODE=%1
SET VSProjectFolder=%2
SET PROJECT_UUID=%3
SET BUILD_DIR=%2\bin\%1\%3.sdPlugin
SET STREAM_DECK_PLUGIN_FILE=%BUILD_DIR%\%PROJECT_UUID%.streamDeckPlugin

REM Install plugin
taskkill /f /im streamdeck.exe
taskkill /f /im %PROJECT_UUID%.exe
timeout /t 2
del /q %BUILD_DIR%\%PROJECT_UUID%.streamDeckPlugin 2>nul
%STREAM_DECK_DISTRIBUTION_TOOL% -b -i %BUILD_DIR% -o %BUILD_DIR%
rmdir %APPDATA%\Elgato\StreamDeck\Plugins\%PROJECT_UUID%.sdPlugin /s /q
START "" %STREAM_DECK_APP%
timeout /t %STREAM_DECK_LOAD_TIMEOUT_SECONDS%
call %STREAM_DECK_PLUGIN_FILE%
EXIT 0