# METAR Parser

METAR Parser is a C# WPF application that 

## Installation

Download the .zip file and extract the .zip to a folder

Open Visual Studio, select 'open a project or solution'

Navigate to the new folder and select MetarAppWPF.sln

## Usage
On running the application, you will see the main menu of the application

--img here

Enter the ICAO of the airport you want to parse the METAR for

The results of the request will be displayed on the next page

--img

The original encoded METAR is displayed on top

Below, the decoded METAR is displayed

## Notes
Time is always displayed in UTC

Wind speed is always in knots

Temperature is always celsius

Altitude pressure will be displayed in the units used by the given airport

Distance and altitude will be displayed in the measurements used by the given airport
