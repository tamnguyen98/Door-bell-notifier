# Doorbell Notifier

A hardware and software project that alerts your Windows PC when someone rang your doorbell. This is good if your room is far from the bell and you often listen to music through headphones.

## Requirements
- Windows (with .NET framework 4.5+)
- ESP8266 with Wifi
  -  ```ESP8266WiFi``` ```WiFiClient``` ```ESP8266WebServer``` ```ESP8266mDNS``` and ```ESP8266WiFiMulti``` library.
- Some fundamental electrical knowledge.

## Installation

### Setting Up the ESP8266

Open the Arduino code (.ino) and change the following wifi variables' value to match your wifi login.
```C
char* wifiName = "WIFI NAME";
char* wifiPW = "WIFI PASS";
```
This allows the microcontroller to connect to your wifi and act as a server. Additionally, you may change the ```inputPin``` value to the pin that you want to receive the electrical signal from your doorbell (make sure it does not exceed 3.3V and 12mA!).

Then compile the code to ensure there is no error and upload the code to the 8266. Head over to the 8266's IP address in your browsers (log into your router and look at the device's IP in the 'connected devices' section) and see if it shows either a 0 or a 1. If it neither, then either you have the wrong IP or your device is not connected to the wifi.

Once you get the device working, wire it up to your doorbell (all doorbell are different, so look up your bell's schematics) and recheck the address.

#### Note
If you can't compile/upload due to library issues, make sure you've installed ```ESP8266WiFi``` ```WiFiClient``` ```ESP8266WebServer``` ```ESP8266mDNS``` and ```ESP8266WiFiMulti``` library.

### Running the software

Run the final release build, and it'll prompt you for a server URL. Enter the 8266's IP in the input field and click update. If it connects successfully, it will tell you (within 3 seconds). You can minimize the application, and it will go into your tray icons (bottom right) but still run in the background. Additionally, you can check if the software works by wiring up the 8266 to a button on a breadboard to act as a bell.

## Usage

### Running the software

Run the final release build, and it'll prompt you for a server URL. Enter the 8266's IP in the input field and click update. If it connects successfully, it will tell you (within 3 seconds). You can minimize the application, and it will go into your tray icons (bottom right) but still run in the background. Additionally, you can check if the software works by wiring up the 8266 to a button on a breadboard to act as a bell.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update the tests as appropriate.