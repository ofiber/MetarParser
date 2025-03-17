# METAR Parser

METAR Parser is a C# WPF application that 

## Installation

Download the .zip file and extract the .zip to a folder

Open Visual Studio, select 'open a project or solution'

Navigate to the new folder and select MetarAppWPF.sln

## Usage

### Enter an ICAO code
On running the application, you will see the main menu

![Screenshot1](https://github.com/user-attachments/assets/dedc001f-76ed-442b-8141-30cf29c841fa)

Enter the ICAO of the airport you want to parse the METAR for

The results of the request will be displayed on the next page

KALB - Albany International Airport
![Screenshot2](https://github.com/user-attachments/assets/ae00c798-6c9e-4c76-8ba9-260e23073653)

EGLL - Heathrow Airport
![Screenshot3](https://github.com/user-attachments/assets/cd6fe5e9-57b4-4511-9942-ef0800d6450e)

The original encoded METAR is displayed on top

Below, the decoded METAR is displayed

<br/>

### Enter your own METAR

You can also enter your own METAR in the second text box

![Screenshot4](https://github.com/user-attachments/assets/b8d5fbc3-144d-4094-ab7a-4534c4c7f5d7)

The results will be displayed on the next page

This is the METAR that the program parsed for us
METAR KALB 162251Z 25015G25KT 10SM OVC025 05/M01 A2999 RMK AO2

![Screenshot5](https://github.com/user-attachments/assets/3d9f2f2f-4f34-413e-a10c-90bcabad309b)

<br/>

Entering an invalid ICAO or METAR will display an error message

## Notes
Time is always displayed in UTC

Wind speeds are always in knots

Temperatures are always celsius

Altitude pressure are always displayed in the units used by the given airport

Distance and altitude are always displayed in the measurements used by the given airport
