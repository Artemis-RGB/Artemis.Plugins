# Artemis Plugins
[![Build Status](https://dev.azure.com/artemis-rgb/Artemis/_apis/build/status/Artemis-RGB.Artemis.Plugins?repoName=Artemis-RGB%2FArtemis.Plugins&branchName=master)](https://dev.azure.com/artemis-rgb/Artemis/_build/latest?definitionId=2&repoName=Artemis-RGB%2FArtemis.Plugins&branchName=master)
[![GitHub license](https://img.shields.io/badge/license-noncommercial-blue.svg)](https://github.com/Artemis-RGB/Artemis.Plugins/blob/master/LICENSE)
[![GitHub stars](https://img.shields.io/github/stars/Artemis-RGB/Artemis.Plugins.svg)](https://github.com/Artemis-RGB/Artemis.Plugins/stargazers)
[![Discord](https://img.shields.io/discord/392093058352676874?logo=discord&logoColor=white)](https://discord.gg/S3MVaC9) 
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=VQBAEJYUFLU4J) 

Repository containing official Artemis plugins

### Installation
Note: These plugins are provided in a pre-release state. The installation process will eventually be automated through the workshop
They are also included in the pre-release builds of Artemis found over at [the main repository](https://github.com/Artemis-RGB/Artemis).
1. Find the latest succesful build [here](https://dev.azure.com/artemis-rgb/Artemis/_build/latest?definitionId=2&repoName=Artemis-RGB%2FArtemis.Plugins&branchName=master) and open the build artifact by clicking on ![1 published](https://i.imgur.com/UBu0BBW.png)
2. Download the Artifacts you'd like. Each Artifact contains one plugin
3. Extract the Artifact into its own folder at ```%ProgramData%\Artemis\plugins```
4. If already running, restart Artemis

**If you run into any issues please let us know on Discord.**

## Plugin development
While Artemis 2 is still in development, the plugin API is pretty far along.
To get started developing your plugin, you can use the following resources:
* [Plugin Templates](https://github.com/Artemis-RGB/Artemis.Plugins.Templates)
* [Plugin API Documentation](https://artemis-rgb.com/docs/)

Due to the volatine nature of the project right now, there is no documentation yet. The templates provide some commentary to get you going and feel free to ask for more help in Discord.

#### Want to build? Follow these instructions
1. Create a central folder like ```C:\Repos```
2. Clone Artemis into  ```<central folder>\Artemis```
3. Open ```<central folder>\Artemis\src\Artemis.sln``` and build with the default config
4. Clone Artemis.Plugins into  ```<central folder>\Artemis.Plugins```
5. Open ```<central folder>\Artemis.Plugins\src\Artemis.Plugins.sln``` and build with the default config
