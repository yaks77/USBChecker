# USBChecker
Application which detects new USB mass storage devices on the system and auto eject them if they dont fulfill certain criteria.

The code is based in two different open source projects:
- USBEject  - https://github.com/mthiffau/usbeject
- Detecting USB drive removal http://www.codeproject.com/Articles/18062/Detecting-USB-Drive-Removal-in-a-C-Program

After finding some issues when 2 USB were plugged at the same time, and only one of them was properly ejected, I decided to use a timer instead. You can find the old code still commented, just in case you find the bug on it. ;)

Using a timer, every 5 seconds the application will check the USB devices connected to the system. For those USB mass storage it will check their validity based on some criteria define in the code. By defautl the initial criteria is simply to check the existence of a file in the root folder, it must be called "STAMP.kiosk". If this file does not exist, the USB will be automatically ejected.
Different criteria should be added to the code according to your needs.
