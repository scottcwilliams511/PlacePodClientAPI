# --------------------------------------------------#
# Python Mag Data Test Client Application           #
# File: REST.py                                     #
# Author: Scott Williams swilliams@pnicorp.com      #
# Date: January 22nd, 2018                          #
# Last updated: January 24th, 2018                  #
# Developed in: PyCharm Community Edition 2016.3.2  #
# Project interpreter: 3.5.0                        #
# This program will ask for a sensorID. Given it, a #
# request will be sent down to the REST API that    #
# will request that the sensor sends back some      #
# mag data. The program will then connect to the    #
# MQTT API and will let us know when the sensor     #
# next wakes up. Once it has woken up, the program  #
# disconnects from the MQTT API. Next another API   #
# call will be made so that the data can be pulled  #
# from the database. The results are written to a   #
# new text file.                                    #
#-------------------------------------------------- #

# Used for timestamps and to make the app sleep
from datetime import datetime
import time

# Used to make get requests to the api
import requests

# Used to deserialize JSON obtained from the get call
import json

# Used to subscribe to the MQTT api
import paho.mqtt.client as mqtt

''' REST API connection Values

To get these values: 1) Login to PNI cloud account at https://parking.pnicloud.com
                     2) Click on settings > REST API
                     3) If there is no key, click "Re-Generate API Key"
                     4) Copy the API URL and the API Key into the below values '''

API_SERVER = ""
API_KEY = ""


''' MQTT API connection values

To get these values: 1) Login to PNI cloud account at https://parking.pnicloud.com
                     2) Click on settings > MQTT API
                     3) If there are no credentials, click "Re-Generate MQTT Credentials"
                     4) Click Show Password"
                     5) Copy all three values into the respective fields below
                     6) Select the topic you want to listen to. There are currently 2 options:
                        Car Presence Topic
                        Parking Lot Count Topic
                     7) Paste the listed topic into the topic field below.
For serverURL, don't include "mqtts://" or ":8883", otherwise the connection will not work '''

serverURL = ""
username = ""
password = ""

topic = ""

# Port used
port = 8883


# Interval to wait for sensor to send back data in seconds
WAKEUP_INTERVAL = 45


'''
@function main
This application is meant to only work with one sensorId, and is strictly a proof of concept
for attempting to grab mag data. '''
def main():
    if API_SERVER == "":
        print("API_SERVER url is not set.")
        return
    if API_KEY == "":
        print("API_KEY is not set.")
        return
    if WAKEUP_INTERVAL == "":
        print("WAKEUP_INTERVAL is not set.")
        return
    if serverURL == '':
        print("serverURL not set!")
        return
    if username == '':
        print("username not set!")
        return
    if password == '':
        print("password not set!")
        return
    if topic == '':
        print("topic not set!")
        return


    # Create a MQTT client
    client = mqtt.Client()

    # Specify functions to use for connection and publish messages
    client.on_connect = on_connect
    client.on_message = on_message

    # Set the username and password for the broker
    # Client ID must be unique otherwise if multiple users are using the same ID it will knock them off
    # Since a client ID is not provided, paho randomly generates one for us giving a high probability of uniqueness
    client.username_pw_set(username, password)
    print("Username and password successfully set!")

    # Enable TLS
    # On macOS it is sufficient to leave the parameter as a "0"
    # If you are on another system or this does not work, then do the following:
    # 1) Login to PNI cloud account at https://parking.pnicloud.com
    # 2) Click on settings > MQTT API
    # 3) Where it says "* SSL/TLS required", click on the Certificate Authority URL
    # 4) Under "Root CAs" look for the certificate that has G2 in its name
    # 5) On the right hand side, right click on PEM to save the certificate.
    # 6) Put the .PEM file in the same directory as this script
    # 7) Post the file name as an argument below
    #    example:
    #    client.tls_set("./SFSRootCAG2.pem")
    #    or
    #    client.tls_set(0)
    client.tls_set("./SFSRootCAG2.pem")
    print("TLS successfully set!")

    # MQTT is now ready to subscribe to a topic, but we don't do that yet
    userInput = input("Run mag data test (y/n)? ")
    if userInput != 'y' and userInput != 'Y':
        return

    global sensorId
    sensorId = input("Enter sensor ID: ")
    sensorId = str(sensorId)

    # Establish the connection to the broker
    print("Attempting to connect...")
    try:
        client.connect(serverURL, port, 60)
    except:
        print("Couldn't connect to serverURL address.")
        return

    global timeSent
    timeSent = initializeMagDataTest(sensorId)
    if timeSent is None:
        return

    print("Request was sent at " + timeSent)

    # We now wait for the sensor to wake up
    client.loop_forever()
#---end main


'''
@function initializeMagDataTest
Sends down an API request that will queue a bunch of ParamIO downlinks
and a timed mag data downlink. The cloud will see these the next time
the sensor wakes up. It takes 10 - 30 seconds for the sensor to send
all of the requested data to the cloud.

@param {String} sensorId: Id of the sensor to send the request to

@returns {String}: ISO Date string. '''
def initializeMagDataTest(sensorId):
    print("Sending request using sensorId: " + sensorId + "...")

    # Make the request JSON
    params =  "{"
    params +=   " 'sensorId': '" + sensorId + "' "
    params += "}"

    # Get the ISO date-time before making the request
    d = datetime.utcnow()
    timeStr = str(d).replace(" ", "T").split(".")[0] + ".000"

    # Make the post call
    result = post("/api/sensor/initialize-mag-data-test", params)

    # Stop running if an error occurs during the post call
    if result == "Exiting...":
        return

    # If an error didn't occur, then the call was successful
    print("Initialization request successfully sent")
    return timeStr
#---end


'''
@function magDataTestResponse
Sends down an API call that will query for mag data created and 
the ParamIOs retrieved from the initializeMagDataTest call

@param {String} sensorId: sensorId: Id of the sensor to send the request to

@param {String} timeSent: ISO Date string when initializeMagDataTest was called

@returns {String} Either a JSON string or an error string. '''
def magDataTestResponse(sensorId, timeSent):
    # Make the get call
    result = get("/api/sensor/mag-data-test-response/" + sensorId + "/" + timeSent)

    # Stop running if an error occurs during the get call
    if result == "Exiting...":
        return
    result = result.decode('utf-8')
    print("Mag Data Test response recieved!")

    payload = json.dumps(result)
    payload = json.loads(payload)
    print(payload)
    return payload
#---end


'''
@function post
Makes a post API call using the supplied url, optional filters and key

@param {String} urlPath: API route to call

@param {String} data: JSON string to send as a parameter 

@returns {String} An empty string, response JSON, or error message. '''
def post(urlPath, data):
    # Concatenate the base server address with the url path
    url = API_SERVER + urlPath

    # Include the api key into the header
    # Must set content-type or else a 415 response error will occur
    headers = {'X-API-KEY': API_KEY, 'content-type': 'application/json; charset=utf-8'}
    response = requests.post(url, data=data, headers=headers)

    # If the status isn't 200, then an error occurred!
    if response.status_code != 200:
        return errorHandler(response)

    print("Successful connection...")
    return response.content
#---end


'''
@function get
Makes a get API call using the supplied url and key

@param {String} urlPath : API route with parameters to call

@returns {String} An empty string, response JSON, or error message. '''
def get(urlPath):
    # Concatenate the base server address with the url path
    url = API_SERVER + urlPath

    # Include the api key into the header
    headers = {'X-API-KEY': API_KEY}
    response = requests.get(url, headers=headers)

    # If the status isn't 200, then an error occurred!
    if response.status_code != 200:
        return errorHandler(response)

    print("Successful connection...")
    return response.content
#---end


'''
@function on_connect
The callback for when the client receives a CONNACK response from the server.
Once we are connected we can subscribe to a topic

@param {Object} client: the client instance for this callback

@param {Object} userdata: the private user data as set in Client() or user_data_set()

@param {Integer} rc: the connection result [0-5]. '''
def on_connect(client, userdata, rc):
    # Connection code 0 works
    print("Connected with result code " + str(rc))

    # If rc isn't 0, then the connection was refused
    if rc != 0:
        if rc == 1:
            print("Connection refused - incorrect protocol version")
        elif rc == 2:
            print("Connection refused - invalid client identifier")
        elif rc == 3:
            print("Connection refused - server unavailable")
        elif rc == 4:
            print("Connection refused - bad username or password")
        elif rc == 5:
            print("Connection refused - not authorised")
        exit(1)

    # Add the sensorId to the topic instead of '#'
    subscriptionTopic = topic.replace("#", sensorId)

    # Subscribing in on_connect() means that if we lose the connection and
    # reconnect then subscriptions will be renewed.
    print("\nSubscribing to " + subscriptionTopic + "\n")
    client.subscribe(subscriptionTopic)

    print("Waiting for sensor to wakeup...")
#---end


'''
@function on_message
The callback for when a PUBLISH message is received from the server.

@param {Object} client: the client instance for this callback

@param {Object} userdata: the private user data as set in Client() or user_data_set()

@param {Object} msg: an instance of MQTTMessage. This is a class with members topic, 
payload, qos, retain. '''
def on_message(client, userdata, msg):
    result = msg.payload

    # MQTT returns a "byte string" which needs to be decoded for use with json
    result = result.decode('utf_8')

    # Don't populate fields if JSON is empty
    if result == '':
        print("Error: Published returned empty data")
        return

    # Breaks the json into a list of Sensor objects
    payload = Payload(result)

    # Check for server time field
    timeStamp = payload.ServerTime.replace("Z", "")
    packetTime = datetime.strptime(timeStamp, '%Y-%m-%dT%H:%M:%S.%f')
    requestTime = datetime.strptime(timeSent, '%Y-%m-%dT%H:%M:%S.%f')

    # If this is true, then a carPresence event has been seen since
    # the first API request was made
    if packetTime > requestTime:
        print("Sensor was woken up!")

        # Disconnect from MQTT, we know the sensor has woken up now
        client.disconnect()
        print("Waiting " + str(WAKEUP_INTERVAL) + " for sensor to send data...")
        time.sleep(WAKEUP_INTERVAL)

        responseJson = magDataTestResponse(sensorId, timeStamp)
        if responseJson is None:
            return

        # Formatting filenames can be weird...
        timeStamp = timeSent.replace(":", ".")
        filename = sensorId + "_" + timeStamp
        file = open("%s.txt" % filename, "w+")  # "w+" = write, create if doesn't exist
        file.write(responseJson)
        file.close()
        print("File " + "%s.txt" % filename + " created!")
#---end


'''
@function errorHandler
Looks at error code and prints the appropriate error string. If 500, the error
message is grabbed from the API response.

@param {Object} error: Error object thrown from the API

@returns {string} indicating the app should stop. '''
def errorHandler(error):
    if error.status_code == 401:
        print("Error: \"HTTP Error 401\" - Unauthorized: Access is denied due to invalid credentials.")
    elif error.status_code == 404:
        print("Error: \"HTTP Error 404\" - Not Found.")
    elif error.status_code == 415:
        print("Error: \"HTTP Error 415\" - Unsupported media type")
    else:
        message = error.content.decode('utf_8')
        errorMsg = json.loads(message, object_hook=createErrorPayload)
        print("Error: \"HTTP ERROR " + str(error.status_code) + "\" - " + errorMsg.code + ": " + errorMsg.message)
    return "Exiting..."
#---end


def createErrorPayload(dct):
    return Error(dct['Code'], dct['Message'])
#---end


class Error(object):
    def __init__(self, code, message):
        self.code = code
        self.message = message
#---end


# Dynamic dictionary object used to publish results
class Payload(object):
    def __init__(self, j):
        self.__dict__ = json.loads(j)
#---end


# Once all functions have been defined, it is safe to execute main
main()