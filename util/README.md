# How to use build installer

1. Download and place the DistributionTool.exe in the same folder as the *install.bat*
    * File be downloaded from: https://developer.elgato.com/documentation/stream-deck/sdk/exporting-your-plugin/
2. Build the project in Visual Studio
3. Run the shortcut *run_install.bat* to install this project's plugin
    * You can also run *install.bat* directly with the build arguments to install your plugin (useful for automated build pipelines)

## Params for Install.bat
* Param 1: **Debug/Release** which build config to use
* Param 2: **VSProjectFolderPath** the path to the plugin project
* Param 3: **UUID/AssemblyName** the UUID or Assembly name of the plugin project
