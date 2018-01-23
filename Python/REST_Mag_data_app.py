# --------------------------------------------------#
# Python WEB API client                             #
# File: REST.py                                     #
# Author: Scott Williams swilliams@pnicorp.com      #
# Date: January 22nd, 2018                          #
# Last updated: January 23nd, 2018                  #
# Developed in: PyCharm Community Edition 2016.3.2  #
# Project interpreter: 3.5.0                        #
# This program will ask for a sensorID. Given it, a #
# request will be sent down to the customer API     #
# that will request that the sensor sends back some #
# mag data. After waiting for the sensor to wake    #
# up, another API call will be made so that the     #
# data can be pulled from the database.             #
#-------------------------------------------------- #

# Used for timestamps and to make the app sleep
from datetime import datetime
import time

# Used to make get requests to the api
import requests

# Used to deserialize JSON obtained from the get call
import json

# To get these values: 1) Login to PNI cloud account at https://parking.pnicloud.com
#                      2) Click on settings > REST API
#                      3) If there is no key, click "Re-Generate API Key"
#                      4) Copy the API URL and the API Key into the below values
API_SERVER = ""
API_KEY = ""

# Number of seconds. Default is 300
#WAKEUP_INTERVAL = 300
WAKEUP_INTERVAL = ""


'''
@function main
This application is meant to only work with one sensorId, and is strictly a proof of concept
for attempting to grab mag data. '''
def main():
    if API_SERVER == "":
        print("API_SERVER url is not defined.")
        return
    if API_KEY == "":
        print("API_KEY is not defined.")
        return
    if WAKEUP_INTERVAL == "":
        print("WAKEUP_INTERVAL is not defined.")
        return

    userInput = input("Run mag data test? ")
    if userInput == 'y' or userInput == 'Y':
        sensorId = input("Enter sensor ID: ")
        sensorId = str(sensorId)
        timeSent = initializeMagDataTest(sensorId)
        if timeSent is None:
            return

        print("Request was sent at " + timeSent)

        # The timeSent time will be our lower time limit bound,
        # while our upper time limit bound will be when the call
        # to the second API method is made.
        print("Waiting " + str(WAKEUP_INTERVAL) + " seconds for sensor to wakeup...")

        # We want the sensor that we are using to have waken up in between
        # this interval of time. Only then will we get the proper data we want.
        time.sleep(WAKEUP_INTERVAL)

        test = magDataTestResponse(sensorId, timeSent)
        if test is None:
            return

        # Formatting filenames can be weird...
        timeSent = timeSent.replace(":", ".")
        filename = sensorId + "_" + timeSent
        file = open("%s.txt" % filename, "w+")  # "w+" = write, create if doesn't exist
        file.write()
        file.close()
        print("File " + "%s.txt" % filename + " created!")
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
    timeStr = str(d).replace(" ", "T").split(".")[0]

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
    result = result.decode('utf_8')
    print("Mag Data Test response recieved!")

    payload = json.loads(result)
    print(payload)
    return str(payload)
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

    # If the status isn't 200, then an error occured!
    if response.status_code != 200:
        return errorHandler(response)

    print("Successful connection...")
    return response.content
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


class Error(object):
    def __init__(self, code, message):
        self.code = code
        self.message = message
#---end


def createErrorPayload(dct):
    return Error(dct['Code'], dct['Message'])
#---end


# Once all functions have been defined, it is safe to execute main
main()