# --------------------------------------------------#
# Python WEB API client                             #
# File: REST.py                                     #
# Author: Scott Williams swilliams@pnicorp.com      #
# Date: June 8th, 2017                              #
# Last updated: October 26th, 2017                  #
# Developed in: PyCharm Community Edition 2016.3.2  #
# Project interpreter: 3.5.0                        #
# Tests the Rest Web API for PlacePod. This program #
# assumes that the gotten data will conform to the  #
# below class objects. Program may crash otherwise. #
#-------------------------------------------------- #

from datetime import datetime
import time

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


def main():
    print("This first sample application will test the get, insert, update, and "
          + "remove function of 'gateways', 'parking lots' and 'sensors'")
    userInput = input("Run first sample application (y/n)? ")
    if userInput == 'y' or userInput == 'Y':
        getInsertUpdateRemoveTests()

    print("This second sample application will test the other 'sensor' operations. "
          + "A sensor ID must be provided to proceed.")
    userInput = input("Run second sample application (y/n)? ")
    if userInput == 'y' or userInput == 'Y':
        sensorId = input("Enter sensor ID: ")
        sensorOperations(sensorId)
#---end main


#--------------------------------First Sample Application----------------------------#
# Title: Get/Insert/Update/Remove tests:  'getInsertUpdateRemoveTests()'             #
#                                                                                    #
# Description: Program will do the following (CRUD) tasks, waiting for input         #
# between tasks for user verification:                                               #
#   1) Get all parking lots                                                          #
#   2) Get all sensors                                                               #
#   3) Get all gateways                                                              #
#   4) Insert a new 'test' parking lot and get its ID                                #
#   5) Update the 'test' parking lot using its ID                                    #
#   6) Insert a new 'test' sensor to the 'test' lot                                  #
#   7) Update the 'test' sensor                                                      #
#   8) Insert a new 'test' gateway                                                   #
#   9) Update the 'test' gateway using its ID                                        #
#  10) Remove the 'test' gateway                                                     #
#  11) Remove the 'test' sensor                                                      #
#  12) Remove the 'test' parking lot                                                 #
# NOTE: If all tasks complete, then there will be no test data left on your account  #
# from this test application.                                                        #
#                                                                                    #
# If a task fails, the error message will be displayed in the output, and the        #
# program will attempt to go to the next task. It is advised to stop the program     #
# since several tasks rely on the results of the previous tasks. You will also want  #
# to remove any test data left on your account due to the insert calls. This can be  #
# done on either the API's Swagger page or through the parking Cloud.                #
#------------------------------------------------------------------------------------#
def getInsertUpdateRemoveTests():

#----- Get Parking Lots -----
    # Test '/api/parking-lots'
    parkingLots = getParkingLots()
    # Sample iteration of retrieved parking lot data
    print("Got " + str(len(parkingLots)) + " Parking Lots: ")
    for i in range(0, len(parkingLots)):
        print("--> " + parkingLots[i].id + ": " + parkingLots[i].name)
    print("")
    wait()

#----- Get Sensors -----
    # Test '/api/sensors'
    sensors = getSensors()
    # Sample iteration of retrieved sensor data
    print("Got " + str(len(sensors)) + " Sensors: ")
    for i in range(0, len(sensors)):
        print("--> " + sensors[i].sensorId + ": " + sensors[i].parkingSpace + ", "
              + sensors[i].status + ", " + sensors[i].parkingLot)
    print("")
    wait()

#----- Get Gateways -----
    # Test '/api/gateways'
    gateways= getGateways()
    # Sample iteration of retrieved gateway data
    print("Got " + str(len(gateways)) + " Gateways: ")
    for i in range(0, len(gateways)):
        print("--> " + gateways[i].gatewayMac + ": " + gateways[i].name)
    print("")
    wait()

#----- Insert Parking Lot -----
    # 'JSON' object for inserting a new parking lot. Used += just for JSON readability
    params =  "{"
    params +=   " 'parkingLotName': 'TEST: python-api-lot-insert', "
    params +=   " 'description': 'python client test', "
    params +=   " 'streetAddress': '123 here', "
    params +=   " 'latitude': '33.810280507079874', "
    params +=   " 'longitude': '-117.9189795255661' "
    params += "}"
    # Test '/api/parking-lot/insert'
    insertParkingLot(params)
    # Get the id of the parking lot inserted above for further operations
    parkingLots = getParkingLots()
    parkingLotId = ""
    for i in range(0, len(parkingLots)):
        if parkingLots[i].name == 'TEST: python-api-lot-insert':
            parkingLotId = parkingLots[i].id
    print("ID of inserted parking lot: " + parkingLotId)
    wait()

#----- Update Parking Lot -----
    # 'JSON' object for updating the name of the new parking lot
    params =  "{"
    params +=   " 'id': '" + parkingLotId + "', "
    params +=   " 'parkingLotName': 'TEST: python-api-lot-update' "
    params += "}"
    # Test '/api/parking-lot/update'
    updateParkingLot(params)
    wait()

#----- Insert Sensor -----
    sensorId = "abcd12340987fedc"
    # 'JSON' Object for inserting a new sensor
    params =  "{"
    params +=   " 'sensorId': '" + sensorId + "', "
    params +=   " 'parkingSpace': 'TEST: python-api-sensor-insert', "
    params +=   " 'parkingLotId': '" + parkingLotId + "', "
    params +=   " 'network': 'PNI', "
    params +=   " 'disabled': false, "
    params +=   " 'latitude':  33, "
    params +=   " 'longitude': -111 "
    params += "}"
    # Test '/api/sensor/insert'
    insertSensor(params)
    wait()

#----- Update Sensor -----
    # 'JSON' Object for updating the name and location of the new sensor
    params =  "{"
    params +=   " 'sensorId': '" + sensorId + "', "
    params +=   " 'parkingSpace': 'TEST: python-api-sensor-update', "
    params +=   " 'latitude':  33.810280507079874, "
    params +=   " 'longitude': -117.9189795255661 "
    params += "}"
    # Test '/api/sensor/update'
    updateSensor(params)
    wait()

#----- Insert Gateway -----
    # 'JSON' Object for inserting a new gateway
    params =  "{"
    params +=   " 'gatewayMac': 'cdef78904321dcba', "
    params +=   " 'gatewayName':  'TEST: python-api-gateway-insert', "
    params +=   " 'parkingLotId': '" + parkingLotId + "' "
    params += "}"
    # Test '/api/gateway/insert'
    insertGateway(params)
    # Get the id of the gateway inserted above for further operations
    gateways = getGateways()
    gatewayId = ""
    for i in range(0, len(gateways)):
        if gateways[i].gatewayMac == "cdef78904321dcba":
            gatewayId = gateways[i].id
    print("ID of inserted gateway: " + gatewayId)
    wait()

#----- Update Gateway -----
    # 'JSON' Object for updating the name of the new gateway
    params =  "{"
    params +=   " 'id': '" + gatewayId + "', "
    params +=   " 'gatewayName': 'TEST: python-api-gateway-update' "
    params += "}"
    # Test '/api/gateway/update'
    updateGateway(params)
    wait()

#----- Remove Gateway -----
    # 'JSON' Object for removing the new gateway
    params =  "{"
    params +=   " 'id': '" + gatewayId + "' "
    params += "}"
    # Test '/api/gateway/remove'
    removeGateway(params)
    wait()

#----- Remove Sensor -----
    # 'JSON' Object for removing the new sensor
    params =  "{"
    params +=   " 'sensorId': '" + sensorId + "', "
    params += "}"
    # Test '/api/sensor/remove'
    removeSensor(params)
    wait()

#----- Remove Parking Lot -----
    # 'JSON' for removing the new parking lot
    params =  "{"
    params +=   " 'id': '" + parkingLotId + "', "
    params += "}"
    # Test '/api/parkingLot/remove'
    removeParkingLot(params)
    wait()

    print("First Sample Application Finished")
#---end getInsertUpdateRemoveTests


#--------------------------------Second Sample Application---------------------------#
# Title: Sensor Operation tests:  'sensorOperations()'                               #
#                                                                                    #
# Description: This function tests non-CRUD sensor API functions and how to          #
# properly call them. Note that while there can be used on a 'test' sensor that      #
# doesn't actually exist, you will want to use this on a real installed sensor.      #
# You can see the results of this test on the Parking Cloud by going to the sensor's #
# diagnostic information.                                                            #
# The tasks that can be optionally performed are:                                    #
#   1) Get sensor history                                                            #
#   2) Send a recalibrate                                                            #
#   3) Run a full BIST test (give up to 5 minutes for a response)                    #
#   4) Send a Ping and wait for response (up to 5 minutes)                           #
#   5) Send a force vacant                                                           #
#   6) Send a force occupied                                                         #
#   7) Send an enable transition state reporting                                     #
#   8) Sen a disable transition state reporting                                      #
#   9) Send a set lora wakeup interval to 5 minutes                                  #
#  10) Send a set loRa Tx power call to 11dB                                         #
#  11) Send a Tx spreading factor call to SF 7, BW 125 kHz                           #
#  12) Send a set frequency sub band call to 902.3 kHz  - 903.7 kHz - 125k           #
#                                                                                    #
# NOTE: There are only so many requests that can be queued by a sensor at a time,    #
# so it is suggested to not make rapid calls to the sensor and wait around a minute  #
# before sending another call to the same sensor.                                    #
#------------------------------------------------------------------------------------#
def sensorOperations(sensorId):
    print("Running operations using sensor: " + sensorId)
    sensorId = str(sensorId)

#--- Sensor History ---
    # 'JSON' object for history. Time range here is 10 minutes apart.
    # If your sensor was installed after this date, you may need to
    # modify the time range
    params =  "{"
    params +=   " 'sensorId': '" + sensorId + "', "
    params +=   " 'startTime': '2017-09-08T01:00:00.000Z', "
    params +=   " 'endTime': '2017-09-08T01:10:00.000Z' "
    params += "}"
    # Test '/api/sensor/history'
    # Requires a properly installed sensor
    userInput = input("Get Sensor History (y/n)? ")
    if userInput == "y" or userInput == "Y":
        history = sensorHistory(params)
        # Once history has been populated, you can iterate it to watch
        # how various fields change over time. This is just a small 10
        # minute test
        print("Number of results: " + str(len(history)))
        wait()

    # 'JSON' object for a number of calls
    idParam =  "{"
    idParam +=   " 'sensorId': '" + sensorId + "' "
    idParam += "}"

#--- Recalibrate ---
    userInput = input("Recalibrate sensor (y/n)? ")
    if userInput == "y" or userInput == "Y":
        # Test '/api/sensor/recalibrate'
        recalibrate(idParam)
        wait()

#--- BIST ---
    userInput = input("Run basic internal self test (BIST) (y/n)? ")
    if userInput == "y" or userInput == "Y":
        # Test '/api/sensor/initialize-bist' and '/api/sensor/bist-response/{SensorId}/{LastUpdated}'
        bist(idParam, sensorId)
        wait()

#--- Ping Sensor ---
    userInput = input("Ping sensor (y/n)? ")
    if userInput == "y" or userInput == "Y":
        # Test '/api/sensor/ping' and '/api/sensor/ping-response/{SensorId}/{LastUpdated}'
        ping(idParam, sensorId)
        wait()

#--- Force Vacant ---
    userInput = input("Force car presence to vacant (y/n)? ")
    if userInput == "y" or userInput == "Y":
        # Test '/api/sensor/force-vacant'
        forceVacant(idParam)
        wait()

#--- Force Occupied ---
    userInput = input("Force are presence to occupied (y/n)? ")
    if userInput == "y" or userInput == "Y":
        # Test '/api/sensor/force-occupied'
        forceOccupied(params)
        wait()

#--- Enable Transition State Reporting ---
    userInput = input("Enable Transition State Reporting (y/n)? ")
    if userInput == "y" or userInput == "Y":
        # Test '/api/sensor/enable-transition-state-reporting'
        enableTransitionStateReporting(params)
        wait()

#--- Disable transition state reporting ---
    userInput = input("Disable Transition State Reporting (y/n)? ")
    if userInput == "y" or userInput == "Y":
        # Test '/api/sensor/disable-transition-state-reporting'
        disableTransitionStateReporting(params)
        wait()


#--- Set LoRa Wakeup Interval ---
    userInput = input("Set LoRa Wakeup Interval to 5 minutes (y/n)?" )
    if userInput == "y" or userInput == "Y":
        # Test 'api/sensor/set-lora-wakeup-interval'
        params =  "{"
        params +=   " 'sensorId': '" + sensorId + "', "
        params +=   " 'payload': 5 "
        params += "}"
        setLoraWakeupInterval(params)
        wait()

#--- Set LoRa Tx Power ---
    userInput = input("Set LoRa Tx Power to 11 (y/n)? ")
    if userInput == "y" or userInput == "Y":
        # Carefully read the documentation on this function before using it since it can be dangerous!
        # Test '/api/sensor/set-lora-tx-power'
        params =  "{"
        params +=   " 'sensorId': '" + sensorId + "', "
        params +=   " 'payload': 11 "
        params += "}"
        setLoraTxPower(params)
        wait()

#--- Set Tx Spreading Factor ---
    userInput = input("Set Tx Spreading Factor to SF 7, BW 125 kHz (y/n)? ")
    if userInput == "y" or userInput == "Y":
        # Carefully read the documentation on this function before using it since it can be dangerous!
        # Test '/api/sensor/set-tx-spreading-factor'
        params =  "{"
        params +=   " 'sensorId': '" + sensorId + "', "
        params +=   " 'payload': 6 "
        params += "}"
        setTxSpreadingFactor(params)
        wait()

#--- Set Frequency Sub Band ---
    userInput = input("Set Frequency Sub Band to 902.3 kHz  -  903.7 kHz - 125k (y/n)? ")
    if userInput == "y" or userInput == "Y":
        # Carefully read the documentation on this function before using it since it can be dangerous!
        # Test '/api/sensor/set-frequency-sub-band'
        params =  "{"
        params +=   " 'sensorId': '" + sensorId + "', "
        params +=   " 'payload': 1 "
        params += "}"
        setFrequencySubBand(params)
        wait()

#---end sensorOperations


#--------------------------------API Functions--------------------------------
# These functions are listed as they appear on the Swagger API page.

        # -------Placepod API-------

    # ---------Gateway Functions---------

# --- Get Gateways ---
def getGateways():
    print("Fetching Gateways...")
    # Make the api get call
    result = get("/api/gateways")
    # Stop running if an error occurs during the get call
    if result == "Exiting...":
        return
    # Get returns a "byte string" which needs to be decoded for use with json
    result = result.decode('utf_8')
    # Stop running if the get call returns empty JSON
    if result == '':
        print("Error: Get request returned no data")
        return
    # Breaks the json into a list of ParkingLot objects
    payload = json.loads(result, object_hook=createGatewayPayload)
    # Return the parking lots for later use
    print("Get Gateways Success")
    return payload
#---end getGateways

# --- Insert Gateway ---
def insertGateway(params):
    print("Inserting Gateway...")
    # Make the api post call
    result = post("/api/gateway/insert", params)
    # Stop running if an error occurs during the post call
    if result == "Exiting...":
        return
    # If an error didn't occur, then the insert was successful
    print("Gateway Insert Success")
#---end insertGateway

#--- Remove Gateway ---
def removeGateway(params):
    print("Removing Gateway...")
    # Make the api delete call
    result = delete("/api/gateway/remove", params)
    # Stop running if an error occurs during the post call
    if result == "Exiting...":
        return
    # If an error didn't occur, then the update was successful
    print("Gateway Remove Success")
#---end removeGateway

#--- Update Gateway ---
def updateGateway(params):
    print("Updating Gateway...")
    # Make the api put call
    result = put("/api/gateway/update", params)
    # Stop running if an error occurs during the put call
    if result == "Exiting...":
        return
    # If an error didn't occur, then the update was successful
    print("Gateway Update Success")
#---end updateGateway

    #---------Parking Lot Functions---------

#--- Get Parking Lots ---
def getParkingLots():
    print("Fetching Parking Lots...")
    # Make the api get call
    result = get("/api/parking-lots")
    # Stop running if an error occurs during the get call
    if result == "Exiting...":
        return
    # Get returns a "byte string" which needs to be decoded for use with json
    result = result.decode('utf_8')
    # Stop running if the get call returns empty JSON
    if result == '':
        print("Error: Get request returned no data")
        return
    # Breaks the json into a list of ParkingLot objects
    payload = json.loads(result, object_hook= createParkingLotPayload)
    # Return the parking lots for later use
    print("Get Parking Lots Success")
    return payload
#---end getParkingLots

#--- Insert Parking Lot ---
def insertParkingLot(params):
    print("Inserting Parking Lot...")
    # Make the api post call
    result = post("/api/parking-lot/insert", params)
    # Stop running if an error occurs during the post call
    if result == "Exiting...":
        return
    # If an error didn't occur, then the insert was successful
    print("Parking Lot Insert Success")
#---end insertParkingLot

#--- Remove Parking Lot ---
def removeParkingLot(params):
    print("Removing Parking Lot...")
    # Make the api delete call
    result = delete("/api/parking-lot/remove", params)
    # Stop running if an error occurs during the post call
    if result == "Exiting...":
        return
    # If an error didn't occur, then the update was successful
    print("Parking Lot Remove Success")
#---end removeParkingLot

#--- Update Parking Lot ---
def updateParkingLot(params):
    print("Updating Parking Lot...")
    # Make the api put call
    result = put("/api/parking-lot/update", params)
    # Stop running if an error occurs during the put call
    if result == "Exiting...":
        return
    # If an error didn't occur, then the update was successful
    print("Parking Lot Update Success")
#---end updateParkingLot

    #---------Sensor Functions---------

#--- Get Sensors ---
def getSensors():
    print("Fetching Sensors...")
    # Sample sensor post request with no filters applied
    result = post("/api/sensors", "{}")
    # Stop running if an error occurs during the post call
    if result == "Exiting...":
        return
    # Post returns a "byte string" which needs to be decoded for use with json
    result = result.decode('utf_8')
    # Stop running if the post call returns empty JSON
    if result == '':
        print("Error: Post request returned no data")
        return
    # Breaks the json into a list of Sensor objects
    payload = json.loads(result, object_hook= createSensorPayload)
    # Return the sensors for later use
    print("Get Sensors Success")
    return payload
#---end getSensors

#--- Insert Sensor ---
def insertSensor(params):
    print("Inserting sensor...")
    # Make the post call
    result = post("/api/sensor/insert", params)
    # Stop running if an error occurs during the post call
    if result == "Exiting...":
        return
    # If an error didn't occur, then the insert was successful
    print("Sensor Insert Success")
#---end insertSensor

#--- Remove Sensor ---
def removeSensor(params):
    print("Removing Sensor...")
    # Make the api delete call
    result = delete("/api/sensor/remove", params)
    # Stop running if an error occurs during the post call
    if result == "Exiting...":
        return
    # If an error didn't occur, then the update was successful
    print("Sensor Remove Success")
#---end removeSensor

#--- Update Sensor ---
def updateSensor(params):
    print("Updating sensor...")
    # Make the api put call
    result = put("/api/sensor/update", params)
    # Stop running if an error occurs during the put call
    if result == "Exiting...":
        return
    # If an error didn't occur, then the update was successful
    print("Sensor Update Success")
#---end updateSensor

#--- Sensor History ---
def sensorHistory(params):
    print("Fetching Sensor history...")

    result = post("/api/sensor/history", params)
    # Stop running if an error occurs during the post call
    if result == "Exiting...":
        return
    # Post returns a "byte string" which needs to be decoded for use with json
    result = result.decode('utf_8')
    # Stop running if the post call returns empty JSON
    if result == '':
        print("Error: Post request returned no data")
        return
    # Breaks the json into a list of Sensor objects
    payload = json.loads(result, object_hook= createSensorPayload)
    # Return the sensors for later use
    print("Got Sensor History")
    return payload
#---end sensorHistory

#--- Recalibrate ---
def recalibrate(params):
    print("Sending Recalibrate...")
    # Make the post call
    result = post("/api/sensor/recalibrate", params)
    # Stop running if an error occurs during the post call
    if result == "Exiting...":
        return
    # If an error didn't occur, then the call was successful
    print("Recalibrate Sent")
#---end recalibrate

#--- BIST ---
# This function handles
#   1) api/sensor/initialize-bist
#   2) api/sensor/bist-response/{SensorId}/{LastUpdated}
def bist(params, sensorId):
    print("Sending BIST...")

    # Gross way to get current time in the desired format
    d = datetime.now()
    timeStr = str(d).replace(" ", "T").split(".")[0]

    # Make the post call
    result = post("/api/sensor/initialize-bist", params)
    # Stop running if an error occurs during the post call
    if result == "Exiting...":
        return
    # If an error didn't occur, then the call was successful
    print("BIST Sent")

    # We want to call the bist-response every second for 5 minutes
    # or until a response comes back
    timer = 0
    while timer < 300:
        # Make the get call
        result = get("/api/sensor/bist-response/" + sensorId + "/" + timeStr)

        # Stop running if an error occurs during the get call
        if result == "Exiting...":
            return
        result = result.decode('utf_8')

        # If we get a valid result, exit the loop
        if result != '[]':
            break

        timer += 1
        print("Waiting for Bist Response " + str(timer))
        time.sleep(1)

        # Re-get the current time
        d = datetime.now()
        timeStr = str(d).replace(" ", "T").split(".")[0]

    # If we are outside the loop because of the timer, we didn't get a result
    if timer >= 300:
        print("No response...")
        return

    print("BIST response recieved!")
    payload = json.loads(result)
    print(payload)
    for i in range(0, len(payload)):
        print("--> " + payload[i]["sensorType"] + ": " + payload[i]["status"])
    print("")
#---end bist

#--- Ping ---
# This function's implementation is very similar to BIST
def ping(params, sensorId):
    print("Sending Ping...")

    # Gross way to get current time in the desired format
    d = datetime.now()
    timeStr = str(d).replace(" ", "T").split(".")[0]

    # Make the post call
    result = post("/api/sensor/ping", params)
    # Stop running if an error occurs during the post call
    if result == "Exiting...":
        return
    # If an error didn't occur, then the call was successful
    print("Ping Sent")

    # We want to call the ping-response every second for 5 minutes
    # or until a response comes back
    timer = 0
    while timer < 300:
        # Make the get call
        result = get("/api/sensor/ping-response/" + sensorId + "/" + timeStr)

        # Stop running if an error occurs during the get call
        if result == "Exiting...":
            return
        result = result.decode('utf_8')

        # If we get a valid result, exit the loop
        if result != '[]':
            break

        timer += 1
        print("Waiting for Ping Response " + str(timer))
        time.sleep(1)

        # Re-get the current time
        d = datetime.now()
        timeStr = str(d).replace(" ", "T").split(".")[0]

    # If we are outside the loop because of the timer, we didn't get a result
    if timer >= 300:
        print("No response...")
        return

    print("Ping response recieved!")
    payload = json.loads(result)
    print(payload)
    for i in range(0, len(payload)):
        print("--> Ping RSSI: " + str(payload[i]["pingRssi"]) + ", Ping SNR: " + str(payload[i]["pingSNR"])
              + ". Server time: " + payload[i]["serverTime"])
    print("")
#---end ping

#--- Force Vacant ---
# Note: The following api calls are all implemented the same as force vacant, just changing out the api route
#  1) /api/sensor/force-occupied
#  2) /api/sensor/enable-transition-state-reporting
#  3) /api/sensor/disable-transition-state-reporting
def forceVacant(params):
    print("Sending Force Vacant...")
    # Make the post call
    result = post("/api/sensor/force-vacant", params)
    # Stop running if an error occurs during the post call
    if result == "Exiting...":
        return
    print("Force Vacant Sent")
#---end forceVacant

#--- Force Occupied ---
def forceOccupied(params):
    print("Sending Force Occupied...")
    # Make the post call
    result = post("/api/sensor/force-occupied", params)
    if result == "Exiting...":
        return
    print("Force Occupied Sent")
#---end forceOccupied

#--- Enable Transition State Reporting ---
def enableTransitionStateReporting(params):
    print("Sending Enable Transition State Reporting...")
    # Make the post call
    result = post("/api/sensor/enable-transition-state-reporting", params)
    if result == "Exiting...":
        return
    print("Enable Transition State Reporting Sent")
#---end enableTransitionStateReporting

#--- Disable Transition State Reporting ---
def disableTransitionStateReporting(params):
    print("Sending Disable Transition State Reporting...")
    # Make the post call
    result = post('/api/sensor/disable-transition-state-reporting', params)
    if result == "Exiting":
        return
    print("Disable Transition State Reporting Send")
#---end disableTransitionStateReporting

#--- Set LoRa Wakeup Interval ---
#  1)
#  2) /api/sensor/set-tx-spreading-factor
#  3) /api/sensor/set-frequency-sub-band
def setLoraWakeupInterval(params):
    print("Sending Set LoRa Wakeup Interval...")
    # Make the post call
    result = post("/api/sensor/set-lora-wakeup-interval", params)
    # Stop running if an error occurs during the post call
    if result == "Exiting...":
        return
    print("Set LoRa Wakeup Interval Sent")
#---end setLoraWakeupInterval

#--- Set LoRa Tx Power ---
def setLoraTxPower(params):
    print("Sending Set LoRa Tx Power...")
    # Make the post call
    result = post("/api/sensor/set-lora-tx-power", params)
    # Stop running if an error occurs during the post call
    if result == "Exiting...":
        return
    print("Set LoRa Tx Power Sent")
#---end setLoraTxPower

#--- Set Tx Spreading Factor ---
def setTxSpreadingFactor(params):
    print("Sending Set Tx Spreading Factor...")
    # Make the post call
    result = post("/api/sensor/set-tx-spreading-factor", params)
    if result == "Exiting...":
        return
    print("Set Tx Spreading Factor Sent")
#---end setTxSpreadingFactor

#--- Set Frequency Sub Band ---
def setFrequencySubBand(params):
    print("Sending Set Frequency Sub Band...")
    # Make the post call
    result = post("/api/sensor/set-frequency-sub-band", params)
    if result == "Exiting...":
        return
    print("Set Frequency Sub Band Sent")
#---end setFrequencySubBand

#-----------------------------------------------------------------------------------


#--------------------------------HTTP request methods--------------------------------

#--- GET ---
# Makes a get API call using the supplied url and key
def get(urlPath):
    # Concatenate the base server address with the url path
    url = API_SERVER + urlPath
    # Include the api key into the header
    headers = {'X-API-KEY': API_KEY}
    response = requests.get(url, headers=headers)
    # If the status isn't 200, then an error occured!
    if response.status_code != 200:
        return errorHandler(response)
    print("Successful connection...")
    return response.content
#---end get

#--- POST ---
# Makes a post API call using the supplied url, optional filters and key
def post(urlPath, data):
    # Concatenate the base server address with the url path
    url = API_SERVER + urlPath
    # Include the api key into the header
    # Must set content-type or else a 415 responce error will occur
    headers = {'X-API-KEY': API_KEY, 'content-type': 'application/json; charset=utf-8'}
    response = requests.post(url, data=data, headers=headers)
    # If the status isn't 200, then an error occured!
    if response.status_code != 200:
        return errorHandler(response)
    print("Successful connection...")
    return response.content
#---end post

#--- PUT ---
# Makes a put API call using the supplied url, optional filters and key
def put(urlPath, data):
    # Concatenate the base server address with the url path
    url = API_SERVER + urlPath
    # Include the api key into the header
    # Must set content-type or else a 415 responce error will occur
    headers = {'X-API-KEY': API_KEY, 'content-type': 'application/json; charset=utf-8'}
    response = requests.put(url, data=data, headers=headers)
    # If the status isn't 200, then an error occured!
    if response.status_code != 200:
        return errorHandler(response)
    print("Successful connection...")
    return response.content
#---end put

#--- DELETE ---
# Makes a delete API call using the supplied url, optional filters and key
def delete(urlPath, data):
    # Concatenate the base server address with the url path
    url = API_SERVER + urlPath
    # Include the api key into the header
    # Must set content-type or else a 415 responce error will occur
    headers = {'X-API-KEY': API_KEY, 'content-type': 'application/json; charset=utf-8'}
    response = requests.delete(url, data=data, headers=headers)
    # If the status isn't 200, then an error occured!
    if response.status_code != 200:
        return errorHandler(response)
    print("Successful connection...")
    return response.content
#---end delete
#-----------------------------------------------------------------------------------


#-------------------------Objects and object helpers for use in code----------------

# Api value fields for a gateway as an object
class Gateway(object):
    def __init__(self, id, gatewayMac, name, parkingLotId):
        self.id = id
        self.gatewayMac = gatewayMac
        self.name = name
        self.parkingLotId = parkingLotId
#---end Gateway

# Create a Gateway object using the dictionary grabbed from the JSON
def createGatewayPayload(dct):
    return Gateway(dct['id'], dct['gatewayMac'], dct['name'], dct['parkingLotId'])
#--end createGatewayPayload

# Api value fields for a parking lot as an object
class ParkingLot(object):
    def __init__(self, id, name, cameraId):
        self.id = id
        self.name = name
        self.cameraId = cameraId
#---end ParkingLot

# Create a ParkingLot object using the dictionary grabbed from the JSON
def createParkingLotPayload(dct):
    return ParkingLot(dct['id'], dct['name'], dct['cameraId'])
#---end createParkingLotPayload

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
#---end Sensor

# Create a Sensor object using the dictionary grabbed from the JSON
def createSensorPayload(dct):
    return Sensor(dct['sensorId'], dct['parkingSpace'], dct['parkingLot'], dct['status'],
                   dct['carPresence'], dct['gatewayTime'], dct['sentralTime'], dct['temperature'],
                   dct['battery'], dct['lat'], dct['lon'], dct['network'], dct['parkingLotId'])
#---end createSensorPayload
#-----------------------------------------------------------------------------------


#--------------------------------Error Handling-------------------------------------
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
#---end errorHandler

class Error(object):
    def __init__(self, code, message):
        self.code = code
        self.message = message
#---end Error

def createErrorPayload(dct):
    return Error(dct['Code'], dct['Message'])
#---end createErrorPayload
#-----------------------------------------------------------------------------------

def wait():
    input("Press enter to continue...")
    print("")


# Once all functions have been defined, it is safe to execute main
main()