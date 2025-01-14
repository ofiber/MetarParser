# METAR Parser

METAR Parser is a C# WPF application that 

## Installation

Download the .zip file and extract the .zip to a folder

Open Visual Studio, select 'open a project or solution'

Navigate to the new folder and select MetarAppWPF.sln

## Usage
On running the application, you will see the main menu

![readmeImg1](https://github.com/ofiber/MetarParser/assets/61956298/4f4958d4-879e-413c-ad0b-f116d7d1cc9c)

Enter the ICAO of the airport you want to parse the METAR for

The results of the request will be displayed on the next page

![readmeImg2](https://github.com/ofiber/MetarParser/assets/61956298/6f312521-a697-4ef5-b2d6-548f68b8ef95)

The original encoded METAR is displayed on top

Below, the decoded METAR is displayed

## Notes
Time is always displayed in UTC

Wind speeds are always in knots

Temperatures are always celsius

Altitude pressure are always displayed in the units used by the given airport

Distance and altitude are always displayed in the measurements used by the given airport
