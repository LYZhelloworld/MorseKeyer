# Morse Keyer
Morse Keyer is a tool created by BD4VQK (Helloworld), to translate text into morse code, with playback function.

Download: https://github.com/LYZhelloworld/MorseKeyer/releases/latest/

## Project Configuration
Most of the projects can be loaded normally.

The project under `setup` folder in Solution Explorer requires [Microsoft Visual Studio 2022 Installer Projects 2022](https://marketplace.visualstudio.com/items?itemName=VisualStudioClient.MicrosoftVisualStudio2022InstallerProjects) to be installed. This is a project to create MSI installer packages and is not related to the implementation of the main program.

## Features
- Adjusting speed (WPM), tone (frequency), and volume (gain) of the signal.
- Editable message template with callsigns.

## Libraries Used
- [NAudio](https://github.com/naudio/NAudio) 2.1.0: for producing wave signal.
- [VocaDb.ResXFileCodeGenerator](https://github.com/VocaDB/ResXFileCodeGenerator) 3.2.0: for generating i18n resource files.
