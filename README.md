# USBChecker
Application which detects new USB mass storage devices on the system and auto eject them if they dont fulfill certain criteria.

The code is based in two different open source projects:
- USBEject  - https://github.com/mthiffau/usbeject
- Detecting USB drive removal http://www.codeproject.com/Articles/18062/Detecting-USB-Drive-Removal-in-a-C-Program

After finding some issues when 2 USB were plugged at the same time and only one was properly ejected, I decided to move to the use of a timer.

Basically, every 5 seconds the application will check the USB connected to the system. It will check the validity of the USB mass storage based on the defined criteria.
The initial criteria by default is the existence of a file in the root folder, it must be called "STAMP.kiosk". If this file does not exist, the USB will be automatically ejected.
Different criteria should be added to the code according with your needs.
