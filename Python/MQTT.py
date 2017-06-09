# -------------------------------------------------#
# Python REST API client                           #
# File: MQTT.py                                    #
# Author: Scott Williams swilliams@pnicorp.com     #
# Date: June 8th, 2017                             #
# Developed in: PyCharm Community Edition 2016.3.2 #
# Project interpreter: 3.5.0                       #
# Tests the MQTT API for Placepod. Will run until  #
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
#                      5) Copy all four values into the respective fields below
# For serverURL, don't include "mqtts://" or ":8883", otherwise the connection will not work
serverURL = ""
topic = ""
username = ""
password = ""

# Port used
port = 8883


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
    return SensorInfo(dct['sensorId'], dct['parkingSpace'], dct['network'], dct['location'],
                      dct['installationDate'],dct['lat'], dct['lon'], dct['createdAt'], dct['hostFirmware'],
                      dct['sensorFirmware'], dct['parkingName'], dct['validationInProcess'], dct['GatewayTime'],
                      dct['SENtralTime'], dct['ServerTime'], dct['CarPresence'], dct['Confidence'], dct['Temperature'],
                      dct['Battery'], dct['status'])


# The callback for when the client receives a CONNACK response from the server.
def on_connect(client, userdata, rc):
    print("Connected with result code " +str(rc))

    # Subscribing in on_connect() means that if we lose the connection and
    # reconnect then subscriptions will be renewed.
    client.subscribe(topic)


# The callback for when a PUBLISH message is received from the server.
def on_message(client, userdata, msg):

    result = msg.payload

    # MQTT returns a "byte string" which needs to be decoded for use with json
    result = result.decode('utf_8')

    # Breaks the json into a list of Sensor objects
    payload = json.loads(result, object_hook= createSensorInfoPayload)

    # Output each value from the packet
    print("Packet contents")
    print("sensorId: " + payload.sensorId)
    print("parkingSpace: " + payload.parkingSpace)
    print("network: " + payload.network)
    print("location: " + payload.location)
    print("installationDate: " + str(payload.installationDate))
    print("lat: " + str(payload.lat))
    print("lon: " + str(payload.lon))
    print("createdAt: " + payload.createdAt)
    print("hostFirmware: " + payload.hostFirmware)
    print("sensorFirmware: " + payload.sensorFirmware)
    print("parkingName: " + payload.parkingName)
    print("validationInProcess: " + str(payload.validationInProcess))
    print("GatewayTime: " + payload.GatewayTime)
    print("SENtralTime: " + str(payload.SENtralTime))
    print("ServerTime: " + payload.ServerTime)
    print("CarPresence: " + str(payload.CarPresence))
    print("Confidence: " + str(payload.Confidence))
    print("Temperature: " + str(payload.Temperature))
    print("Battery: " + str(payload.Battery))
    print("status: " + payload.status)
    print("")


def main():

    # Create a MQTT client
    client = mqtt.Client()

    # Specify functions to use for connection and publish messages
    client.on_connect = on_connect
    client.on_message = on_message

    # Set the username and password for the broker
    client.username_pw_set(username,password)

    # Enables TLS
    client.tls_set(0)

    # Establish the connection to the broker
    client.connect(serverURL, port, 60)

    # Blocking call that processes network traffic, dispatches callbacks and
    # handles reconnecting.
    # Other loop*() functions are available that give a threaded interface and a
    # manual interface.
    client.loop_forever()

main()