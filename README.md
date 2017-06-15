# PlacePod Client API

The PlacePod cloud offers an asynchronous MQTT API for notifications. 
Additionally, a <a href="https://api.pnicloud.com">REST API</a> is available for syncronous based web services.

 

## Rest Web API
The Rest API may be invoked at any time retrieve sensor status, and history values for parking spaces and lots. Also, downlink communication to the sensor is done via the REST Api.

 - Login into your PNI cloud account with admin permission. 
 - Navigate to  *Settings > Rest API*
 - Click the **Generate API Key** button.
 - Every request must include the api key header.
 - See the <a href="https://api.pnicloud.com">Swagger Rest API docs</a> for more details.
 - Example: <code><pre>curl -X GET \
  --header 'Accept: application/json' \ 
  --header 'X-API-KEY: YOUR-API-KEY' \ 
  'https://api.pnicloud.com/api/parking-lots'  
</pre></code>
   

## MQTT Notification API
 Whenever a Placepod communicates, an event is published to the MQTT defined in your account.

 - Login into your PNI cloud account with admin permission. 
 - Navigate to  *Settings > MQTT API*
 - Click the **Generate Credentials** button.
 - Copy Username, password and topic into MQTT Client.
 - MQTT Connection is on port 8883 and requires TLS configuration.
 - Client library may require trusted CA certificate.
 	- Download [Starfield Services Root Certificate Authority - G2](https://www.amazontrust.com/repository/SFSRootCAG2.pem) . 
	- For example, use the MQTTfx client to test connection.
		 - Check "Enable TLS" under SSL/TLS flag
		 - Enter username/password under "User credentials"
		 - Click Generate Client ID		 
	 -  See <a href="http://www.hivemq.com/blog/mqtt-security-fundamentals-tls-ssl">Security Fundamentals </a> for more details.
  - MQTT topic is hierarchical and can include the wildcards +, &#35; and ?
 	 - Example Topics
	 	 - **&#35;** 
		 	 - recieve all notifications from ALL sensors.
	 	 - **placepod/uplink/+/0080000004000675**
		 	 - Single sensor		 	 
	 - See <a href="http://www.hivemq.com/blog/mqtt-essentials-part-5-mqtt-topics-best-practices">MQTT Topics</a> for more information.
 - Note: To issue commands (Recalibrate, BISTest, SetWakeupInterval) please use the REST API.   

### MQTT Packet format

Mqtt packet format is a JSON object defined as follows:

- `sensorId:` 16 Digit Hex ID. Ex`: '0080000004000675'
- `parkingSpace:` Space PlacePod is assigned to. Ex`: 'Space #54'
- `network`: Network this sensor is asigned to. Ex`: 'PNI' or  'SENET'
- `location`: Not used.
- `installationDate`: Not used
- `lat`: Location of Sensor Ex`: 38.42074876336795
- `lon`: Location of Sensor Ex:-122.75509041539759
- `createdAt`: Date of Sensor Creation. Ex`: '2017-05-03T21:12:52.974Z'
- `hostFirmware`: Host Firmware Revision. Ex`: '0.3.39'
- `sensorFirmware`: Sentral Firmware Revision. Ex`: '1.3.0.0.284'
- `parkingName`: Name of Parking Lot. Ex`: 'Electric Vehicle Parking Lot'
- `validationInProcess`: Internal QA Validation Flag.    
- `GatewayTime`: Event Timestamp from Gateway. Subject to clock drift and misconfigured gateways. Ex`: '2017-06-06T17:23:57.834Z'
- `SENtralTime`: Internal 36 bit timestamp in ticks. Ex`: 309467131
- `ServerTime`: Canonical Event Timestemp from Server. '2017-06-06T17:17:08.977Z'
- `CarPresence`: State of sensor 0 = Initialization/startup (never published) 1 = No Car Present 2 = Car Entering Space 3 = Parked Car Detected 4 = Car Leaving Space 
- `Confidence`: Not Used.
- `Temperature`: Temperature in celcius. Ex`: 23
- `Battery`: Battery Voltage  ex`: 3.6482174396514893
- `status`: Human readable Placepod status`: occupied, vacant, car entering, car leaving

