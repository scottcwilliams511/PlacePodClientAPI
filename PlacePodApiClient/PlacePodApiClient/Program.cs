using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System;

/// <summary>
/// The placepod API is undocumented and subject to change
/// Fully documented API comming soon!
/// </summary>
/// <author>Byron Whitlock bwhitlock@pnicorp.com</author>
namespace PlacePodApiExample
{
    class Program
    {
        static string API_SERVER = "https://api-dev.pnicloud.com";

        // Rest API is documented at https://api.pnicloud.com
        // login to PNI cloud account at https://parking.pnicloud.com
        // click on settings > REST API 
        // Click GENERATE API KEY 
        // Copy and paste API Key here.
        static string API_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJjb21wYW5pZXNBY2NvdW50c0lkIiwidW5pcXVlX25hbWUiOiJzUndUUkRNS2JlZTNZcWdyeiIsIm5iZiI6MTQ5Njg3MTU1OSwiZXhwIjoxNjU0NjM3OTU5LCJpYXQiOjE0OTY4NzE1NTksImlzcyI6Imh0dHBzOi8vd3d3LnBuaWNvcnAuY29tIiwiYXVkIjoiaHR0cHM6Ly93d3cucG5pY29ycC5jb20ifQ.U4A_i1JIYw9LU3jyP8kim9Nt-2bTV_ugQAjLcPVW34E";

        static void Main(string[] args)
        {

            // get list of parking lots
            /*
                [
                  {
                    "id": "string",
                    "name": "string",
                    "cameraId": "string"
                  }
                ]
             * */
            Console.WriteLine("Fetching Parking Lots...");
            var result = Get("/api/parking-lots");
            dynamic lots = JsonConvert.DeserializeObject(result);
            Console.WriteLine(" Got " + lots.Count + " Parking Lots: ");
            foreach (var lot in lots)
            {
                Console.WriteLine("--> " + lot.id + ": " + lot.name + " ");
            }
            Console.WriteLine();

            ///Grab all sensors
            /*
                 [
                  {
                    "sensorId": "string",
                    "parkingSpace": "string",
                    "parkingLot": "string",
                    "status": "string",
                    "carPresence": 0,
                    "serverTime": "2017-06-07T21:06:09.424Z",
                    "gatewayTime": "2017-06-07T21:06:09.424Z",
                    "sentralTime": 0,
                    "temperature": 0,
                    "battery": 0,
                    "lat": 0,
                    "lon": 0,
                    "network": "string",
                    "parkingLotId": "string"
                  }
                ]
             * */
            Console.WriteLine("Fetching Sensors");

            // result = Post("/api/sensors","{  "parkingLotId": "string"}");       // filter on parking lot
            // result = Post("/api/sensors", "{"sensorId": "008000000000b0dd"}");  // filter on sensor id
            result = Post("/api/sensors", "{}"); // all sensors 

            dynamic sensors = JsonConvert.DeserializeObject(result);
            Console.WriteLine(" Got " + sensors.Count + " Sensors");
            foreach (var sensor in sensors)
            {
                Console.WriteLine("--> "+ sensor.sensorId+ ": " + sensor.parkingSpace + ", " + sensor.status + ", "+sensor.parkingLot);
            }
            Console.WriteLine();

        }

        static string Post(string path, string data)
        {
            WebRequest req = WebRequest.Create(API_SERVER + path);
            UTF8Encoding enc = new UTF8Encoding();

            req.Method = "POST";
            req.ContentType = "application/json";
            req.Headers.Add(string.Format("X-API-KEY: {0}", API_KEY));

            var dataStream = req.GetRequestStreamAsync().Result; // You can call .Result on a Task to wait for the result. or if it returns null use wait()
            dataStream.Write(enc.GetBytes(data), 0, data.Length);

            //Get the response
            WebResponse res = req.GetResponseAsync().Result;
            Stream receiveStream = res.GetResponseStream();
            StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
            string content = reader.ReadToEnd();

            return content;
        }


        static string Get(string path)
        {
            WebRequest req = WebRequest.Create(API_SERVER + path);
            UTF8Encoding enc = new UTF8Encoding();

            req.Method = "GET";
            req.Headers.Add(string.Format("X-API-KEY: {0}", API_KEY));

            //Get the response
            WebResponse res = req.GetResponseAsync().Result;
            Stream receiveStream = res.GetResponseStream();
            StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
            string content = reader.ReadToEnd();
            return content;
        }
    }
}
