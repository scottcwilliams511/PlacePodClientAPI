# --------------------------------------------------#
# Python WEB API client                             #
# File: Program.py                                  #
# Author: Scott Williams swilliams@pnicorp.com      #
# Date: June 8th, 2017                              #
# Developed in: PyCharm Community Edition 2016.3.2  #
# Project interpreter: 3.5.0                        #
# Tests the Rest Web API for Placepod. This program #
# assumes that the retried data will conform to the #
# below object fields. Program may crash otherwise  #
#-------------------------------------------------- #

# Used to make get requests to the api
import requests

# Used to deserialize JSON obtained from the get call
import json

# To get these values: 1) Login to PNI cloud account at https://parking.pnicloud.com
#                      2) Click on settings > REST API
#                      3) Click "Re-Generate API Key"
#                      4) Copy the API URL and the API Key into the below values
API_SERVER = ""
API_KEY = ""


# Api value fields for a parking lot as an object
class ParkingLot(object):
    def __init__(self, id, name, cameraId):
        self.id = id
        self.name = name
        self.cameraId = cameraId


# Api value fields for a sensor as an object
class Sensor(object):
    def __init__(self, sensorId, parkingSpace, parkingLot, status, carPresence,
                 gatewayTime, sentralTime, temperature, battery, lat,
                 lon, network, parkingLotId):
        self.sensorId = sensorId
        self.parkingSpace = parkingSpace
        self.parkingLot = parkingLot
        self.status = status
        self.carPresence = carPresence
        self.gateWayTime = gatewayTime
        self.sentralTime = sentralTime
        self.temperature = temperature
        self.battery = battery
        self.lat = lat
        self.lon = lon
        self.network = network
        self.parkingLotId = parkingLotId


# Create a ParkingLot object using the dictionary grabbed from the JSON
def createParkingLotPayload(dct):
    return ParkingLot(dct['id'], dct['name'], dct['cameraId'])


# Create a Sensor object using the dictionary grabbed from the JSON
def createSensorPayload(dct):
    return Sensor(dct['sensorId'], dct['parkingSpace'], dct['parkingLot'], dct['status'],
                   dct['carPresence'], dct['gatewayTime'], dct['sentralTime'], dct['temperature'],
                   dct['battery'], dct['lat'], dct['lon'], dct['network'], dct['parkingLotId'])


# Makes a get API call using the supplied url and key
def get(urlPath):

    # Concatenate the base server address with the url path
    url = API_SERVER + urlPath

    # Include the api key into the header
    headers = {'X-API-KEY': API_KEY}
    response = requests.get(url, headers=headers)

    if response.status_code == 401:
        print("Error: \"HTTP Error 401\" - Unauthorized: Access is denied due to invalid credentials.")
        return 0

    if response.status_code == 404:
        print("Error: \"HTTP Error 404\" - Not Found.")
        return 0

    print("Successful connection...")
    return response.content


# Makes a post API call using the supplied url, optional filters and key
def post(urlPath, data):

    # Concatenate the base server address with the url path
    url = API_SERVER + urlPath

    # Include the api key into the header
    # Must set content-type or else a 415 responce error will occur
    headers = {'X-API-KEY': API_KEY, 'content-type': 'application/json; charset=utf-8'}

    response = requests.post(url, data=data, headers=headers)

    if response.status_code == 401:
        print("Error: \"HTTP Error 401\" - Unauthorized: Access is denied due to invalid credentials.")
        return 0

    if response.status_code == 404:
        print("Error: \"HTTP Error 404\" - Not Found.")
        return 0

    if response.status_code == 415:
        print("Error: \"HTTP Error 415\" - Unsupported media type")
        return 0

    print("Successful connection...")
    return response.content


def main():
    print("Fetching Parking Lots...")

    # Make the api get call
    result = get("/api/parking-lots")

    # Stop running if an error occurs during the get call
    if result == 0:
        return

    # Get returns a "byte string" which needs to be decoded for use with json
    result = result.decode('utf_8')

    # Stop running if the get call returns empty JSON
    if result == '':
        print("Error: Get request returned no data")
        return

    # Breaks the json into a list of ParkingLot objects
    payload = json.loads(result, object_hook= createParkingLotPayload)

    # Sample iteration of retrieved parking lot data
    print("Got " + str(len(payload)) + " Parking Lots: ")
    for i in range(0, len(payload)):
        print("--> " + payload[i].id + ": " + payload[i].name)

    #---

    print("Fetching Sensors...")

    # Sample sensor post request with no filters applied
    result = post("/api/sensors", "{}")

    # Stop running if an error occurs during the post call
    if result == 0:
        return

    # Post returns a "byte string" which needs to be decoded for use with json
    result = result.decode('utf_8')

    # Stop running if the post call returns empty JSON
    if result == '':
        print("Error: Post request returned no data")
        return

    # Breaks the json into a list of Sensor objects
    payload = json.loads(result, object_hook= createSensorPayload)

    # Sample iteration of retrieved sensor data
    print("Got " + str(len(payload)) + " Sensors: ")
    for i in range(0, len(payload)):
        print("--> " + payload[i].sensorId + ": " + payload[i].parkingSpace + ", "
              + payload[i].status + ", " + payload[i].parkingLot)

main()