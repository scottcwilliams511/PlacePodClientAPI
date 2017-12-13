# -------------------------------------------------#
# Python MQTT API client                           #
# File: MQTT.py                                    #
# Author: Scott Williams swilliams@pnicorp.com     #
# Date: June 8th, 2017                             #
# Last updated: December 13th, 2017                #
# Developed in: PyCharm Community Edition 2016.3.2 #
# Project interpreter: 3.5.0                       #
# Tests the MQTT API for PlacePod. Will run until  #
# user terminated.                                 #
#--------------------------------------------------#

# Used to subscribe to the api
import paho.mqtt.client as mqtt

# Used to deserialize JSON obtained from the api
import json

# MQTT API connection values

# To get these values: 1) Login to PNI cloud account at https://parking.pnicloud.com
#                      2) Click on settings > MQTT API
#                      3) Click "Re-Generate MQTT Credentials"
#                      4) Click Show Password"
#                      5) Copy all three values into the respective fields below
#                      6) Select the topic you want to listen to. There are currently 2 options:
#                           Car Presence Topic
#                           Parking Lot Count Topic
#                      7) Paste the listed topic into the topic field below.
# For serverURL, don't include "mqtts://" or ":8883", otherwise the connection will not work


serverURL = ""
username = ""
password = ""

topic = ""

# Port used
port = 8883

# Dynamic dictionary object used to publish results
class Payload(object):
    def __init__(self, j):
        self.__dict__ = json.loads(j)

# Old non dynamic code for reference
'''
# Api value fields for a sensor as an object
class SensorInfo(object):
    def __init__(self, sensorId, parkingSpace, network, location, installationDate,
                 lat, lon, createdAt, hostFirmware, sensorFirmware, parkingName,
                 validationInProcess, GatewayTime, SENtralTime, ServerTime,
                 CarPresence, Confidence, Temperature, Battery, status):
        self.sensorId = sensorId
        self.parkingSpace = parkingSpace
        self.network = network
        self.location = location
        self.installationDate = installationDate
        self.lat = lat
        self.lon = lon
        self.createdAt = createdAt
        self.hostFirmware = hostFirmware
        self.sensorFirmware = sensorFirmware
        self.parkingName = parkingName
        self.validationInProcess = validationInProcess
        self.GatewayTime = GatewayTime
        self.SENtralTime = SENtralTime
        self.ServerTime = ServerTime
        self.CarPresence = CarPresence
        self.Confidence = Confidence
        self.Temperature = Temperature
        self.Battery = Battery
        self.status = status


# Create a SensorInfo object using the dictionary grabbed from the JSON
def createSensorInfoPayload(dct):

    # This was going to set missing packet fields to NULL so the program wouldn't crash
    if not 'network' in dct:
        network = ""
    else:
        network = dct['network']

    if not 'hostFirmware' in dct:
        hostFirmware = ""
    else:
        hostFirmware = dct['hostFirmware']  # Switched to dynamic object use at this point

    return SensorInfo(dct['sensorId'], dct['parkingSpace'], network, dct['location'],
                      dct['installationDate'],dct['lat'], dct['lon'], dct['createdAt'], hostFirmware,
                      dct['sensorFirmware'], dct['parkingName'], dct['validationInProcess'], dct['GatewayTime'],
                      dct['SENtralTime'], dct['ServerTime'], dct['CarPresence'], dct['Confidence'], dct['Temperature'],
                      dct['Battery'], dct['status'])
'''

# The callback for when the client receives a CONNACK response from the server.
def on_connect(client, userdata, rc):

    # Connection code 0 works
    print("Connected with result code " + str(rc))

    if rc == 5:
        print("Error: Invalid username or password!")
        exit(1)

    # The other possible connection codes are not documented, so warn the user that something may have happened
    if rc != 0:
        print("Warning: Connected with code other than 0, errors may occur...")

    # Subscribing in on_connect() means that if we lose the connection and
    # reconnect then subscriptions will be renewed.
    print("\nWaiting on publish...\n")
    client.subscribe(topic)


# The callback for when a PUBLISH message is received from the server.
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

    # Old non dynamic call create objects, again just left for reference
    #payload = json.loads(result, object_hook= createSensorInfoPayload)

    # Output each value from the packet, if the field is there
    # Having there checks ensures that the script won't crash should a packet
    # be missing a field it could have
    print("Packet contents")

    # Fields for a 'Car Presence' packet
    if hasattr(payload, 'sensorId'):
        print("sensorId: " + payload.sensorId)
    if hasattr(payload, 'parkingSpace'):
        print("parkingSpace: " + payload.parkingSpace)
    if hasattr(payload, 'network'):
        print("network: " + payload.network)
    if hasattr(payload, 'location'):
        print("location: " + payload.location)
    if hasattr(payload, 'installationDate'):
        print("installationDate: " + str(payload.installationDate))
    if hasattr(payload, 'lat'):
        print("lat: " + str(payload.lat))
    if hasattr(payload, 'lon'):
        print("lon: " + str(payload.lon))
    if hasattr(payload, 'createdAt'):
        print("createdAt: " + payload.createdAt)
    if hasattr(payload, 'hostFirmware'):
        print("hostFirmware: " + payload.hostFirmware)
    if hasattr(payload, 'sensorFirmware'):
        print("sensorFirmware: " + payload.sensorFirmware)
    if hasattr(payload, 'parkingName'):
        print("parkingName: " + payload.parkingName)
    if hasattr(payload, 'validationInProcess'):
        print("validationInProcess: " + str(payload.validationInProcess))
    if hasattr(payload, 'GatewayTime'):
        print("GatewayTime: " + payload.GatewayTime)
    if hasattr(payload, 'SENtralTime'):
        print("SENtralTime: " + str(payload.SENtralTime))
    if hasattr(payload, 'ServerTime'):
        print("ServerTime: " + payload.ServerTime)
    if hasattr(payload, 'CarPresence'):
        print("CarPresence: " + str(payload.CarPresence))
    if hasattr(payload, 'Confidence'):
        print("Confidence: " + str(payload.Confidence))
    if hasattr(payload, 'Temperature'):
        print("Temperature: " + str(payload.Temperature))
    if hasattr(payload, 'Battery'):
        print("Battery: " + str(payload.Battery))
    if hasattr(payload, 'status'):
        print("status: " + payload.status)

    # Fields for a 'Parking Lot Count' packet
    if hasattr(payload, 'totalNumberOfSpaces'):
        print("totalNumberOfSpaces: " + str(payload.totalNumberOfSpaces))
    if hasattr(payload, 'availableSpaces'):
        print("availableSpaces: " + str(payload.availableSpaces))
    if hasattr(payload, 'adjustedAvailableSpaces'):
        print("adjustedAvailableSpaces: " + str(payload.adjustedAvailableSpaces))
    if hasattr(payload, 'parkingLotId'):
        print("parkingLotId: " + payload.parkingLotId)
    if hasattr(payload, 'parkingLotClosed'):
        print("parkingLotClosed: " + str(payload.parkingLotClosed))

    print("")
    print("Waiting on publish...")


def main():
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
    client.tls_set(0)
    print("TLS successfully set!")

    # Establish the connection to the broker
    print("Attempting to connect...")
    try:
        client.connect(serverURL, port, 60)
    except:
        print("Couldn't connect to serverURL address.")
        return

    # Blocking call that processes network traffic, dispatches callbacks and
    # handles reconnecting.
    # Other loop*() functions are available that give a threaded interface and a
    # manual interface.
    client.loop_forever()
main()