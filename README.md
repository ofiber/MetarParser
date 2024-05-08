# METAR Parser

METAR Parser is a C# WPF application that 

## Installation

Download the .zip file and extract the .zip to a folder

Open Visual Studio, select 'open a project or solution'

Navigate to the new folder and select MetarAppWPF.sln

## Usage
On running the application, you will see the main menu of the application

![MetarAppMain](https://github.com/ofiber/MetarParser/assets/61956298/838cdc78-4a9e-4e3a-9f2a-8a250aa9084c)

Enter the ICAO of the airport you want to parse the METAR for

The results of the request will be displayed on the next page

![MetarAppResults](https://github.com/ofiber/MetarParser/assets/61956298/c2f7ebbf-a104-401b-aa06-2bc19e9304d7)

The original encoded METAR is displayed on top

Below, the decoded METAR is displayed

## Notes
Time is always displayed in UTC

Wind speeds are always in knots

Temperatures are always celsius

Altitude pressure are always displayed in the units used by the given airport

Distance and altitude are always displayed in the measurements used by the given airport
