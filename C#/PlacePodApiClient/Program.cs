using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System;
using PlacePodApiClient;
using PlacePodApiClient.Gateways;
using PlacePodApiClient.API_Methods;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

/// <summary>
/// 
/// Implemented using Placepod API V1.1
/// Last updated: February 21th, 2018
/// 
/// The placepod API is undocumented and subject to change
/// Fully documented API comming soon!
/// 
/// Unlike the python application, the actual error message seems to get lost,
/// so all we get here is a internal 500 error if something blows up like 
/// a bad sensor ID. Errors can be confirmed by entering the same request
/// that caused the error on the API's Swagger page.
/// 
/// </summary>
/// <author>Byron Whitlock bwhitlock@pnicorp.com</author>
/// <author>Scott Williams swilliams@pnicorp.com</author>
namespace PlacePodApiExample {
    public class Program {

        // Rest API is documented at https://api.pnicloud.com

        // To get these values:
        //   1) login to PNI cloud account at https://parking.pnicloud.com
        //   2) click on settings > REST API 
        //   3) Click GENERATE API KEY 
        //   4) Copy the API URL and the API key into the below values
        private static readonly string API_SERVER = "https://api-dev.pnicloud.com";
        private static readonly string API_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJjb21wYW5pZXNBY2NvdW50c0lkIiwidW5pcXVlX25hbWUiOiJzUndUUkRNS2JlZTNZcWdyeiIsIm5iZiI6MTUxMjUwNDUzMSwiZXhwIjoxNjcwMjcwOTMxLCJpYXQiOjE1MTI1MDQ1MzEsImlzcyI6Imh0dHBzOi8vd3d3LnBuaWNvcnAuY29tIiwiYXVkIjoiaHR0cHM6Ly93d3cucG5pY29ycC5jb20ifQ.1EvN7VThWGBRY2NzSZsL6DhT4rFoI6wYSEpQskjCXD4";
        private static Http HttpClient;


        /// <summary>
        /// Main function that initializes the two sample applications
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) {
            // Make sure these values are set. Program can't run without them!
            if (string.IsNullOrWhiteSpace(API_SERVER)) {
                Console.WriteLine("API_SERVER variable not set!");
                return;
            } else if (string.IsNullOrWhiteSpace(API_KEY)) {
                Console.WriteLine("API_KEY variable not set!");
                return;
            }

            HttpClient = new Http(API_SERVER, API_KEY);

            // Program 1
            Console.WriteLine("This first sample application will test the get, insert, update, and remove functions" +
                   " of 'gateways', 'parking-lots', and sensors'.");
            Console.WriteLine("Run first sample application (y/n)? ");
            string input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                Task.Run(async () => {
                    await FirstApp();
                }).GetAwaiter().GetResult();
            }

            


            // TODO finish this and all methods related to it....
            // Program 2
            Console.WriteLine("This second sample application will test the other 'sensor' operaions. " +
                "A sensor ID must be provided to proceed.");
            Console.WriteLine("Run second sample application (y/n)? ");
            input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                Console.WriteLine("Enter sensor ID: ");
                input = Console.ReadLine();
                SecondApp(input);
            }

        }


        /// <summary>
        /// Tests the Get, Insert, Update, and Remove operations
        /// of 'gateway', 'parking lot' and 'sensor'. Tasks are performed in this order:
        ///   1) Get all parking lots
        ///   2) Get all sensors
        ///   3) Get all gateways
        ///   4) Insert a new 'test' parking lot and get its ID
        ///   5) Update the 'test' parking lot using its ID
        ///   6) Insert a new 'test' sensor to the 'test' lot
        ///   7) Update the 'test' sensor
        ///   8) Insert a new 'test' gateway
        ///   9) Update the 'test' gateway using its ID
        ///  11) Remove the 'test' gateway
        ///  12) Remove the 'test' sensor
        ///  13) Remove the 'test' parking lot
        /// Note: If all tasks complete, then there will be no test data left on your
        /// account from this test application.
        /// 
        /// If a task fails, the program will stop since several calls rely on other calls.
        /// You will also want to remove any test data left on your account due to the insert calls.
        /// This can be done on either the API's swagger page or through the Parking Cloud.
        /// </summary>
        public async static Task FirstApp() {
            // All of this is wrapped in a try/catch because if one operation fails it
            // can cause the rest of the first app to not behave as desired.
            try {

                GatewayMethods gatewayMethods = new GatewayMethods(HttpClient);
                ParkingLotMethods parkingLotMethods = new ParkingLotMethods(HttpClient);
                SensorMethods sensorMethods = new SensorMethods(HttpClient);


                // Fetch all parking lots
                Console.WriteLine("Testing '/api/parking-lots'");
                dynamic lots = await parkingLotMethods.GetParkingLots();
                Console.WriteLine("Got " + lots.Count + " Parking Lots: ");
                foreach (var lot in lots) {
                    Console.WriteLine("--> " + lot.id + ": " + lot.name + " ");
                }
                Console.WriteLine();


                // Fetch all sensors
                Console.WriteLine("Testing '/api/sensors'");
                dynamic sensors = await sensorMethods.GetSensors("{}");
                Console.WriteLine("Got " + sensors.Count + " Sensors");
                foreach (var sensor in sensors) {
                    Console.WriteLine("--> " + sensor.sensorId + ": " + sensor.parkingSpace + ", " + sensor.status + ", " + sensor.parkingLot);
                }
                Console.WriteLine();


                // Fetch all gateways
                Console.WriteLine("Testing '/api/gateways'");
                dynamic gateways = await gatewayMethods.GetGateways();
                Console.WriteLine("Got " + gateways.Count + " Gateways: ");
                foreach (var gateway in gateways) {
                    Console.WriteLine("--> " + gateway.gatewayMac + ": " + gateway.name + " ");
                }
                Console.WriteLine();


                // Insert a new parking lot
                Console.WriteLine("Testing '/api/parking-lot/insert'");
                JObject json = new JObject(
                    new JProperty("parkingLotName", "TEST: C#-api-lot-insert"),
                    new JProperty("description", "c# client test"),
                    new JProperty("streetAddress", "123 here"),
                    new JProperty("latitude", 33.810280507079874),
                    new JProperty("longitude", -117.9189795255661)
                );
                await parkingLotMethods.InsertParkingLot(json.ToString());
                Console.WriteLine("Parking Lot Insert Success");


                // Get the parkingLotId of the inserted lot
                lots = await parkingLotMethods.GetParkingLots();
                string parkingLotId = null;
                foreach (var lot in lots) {
                    if (lot.name == "TEST: C#-api-lot-insert") {
                        parkingLotId = lot.id;
                    }
                }
                Console.WriteLine("ID of inserted parking lot: " + parkingLotId);
                Console.WriteLine();


                // Update that new parking lot
                Console.WriteLine("Testing '/api/parking-lot/update'");
                json = new JObject(
                    new JProperty("id", parkingLotId),
                    new JProperty("parkingLotName", "TEST: C#-api-lot-update")
                );
                await parkingLotMethods.UpdateParkingLot(json.ToString());
                Console.WriteLine("Parking Lot Update Success");
                Console.WriteLine();


                // Insert a new sensor
                Console.WriteLine("Testing '/api/sensor/insert'");
                string sensorId = "abcd12340987fed0";
                json = new JObject(
                    new JProperty("sensorId", sensorId),
                    new JProperty("parkingSpace", "TEST: c#-api-sensor-insert"),
                    new JProperty("parkingLotId", parkingLotId),
                    new JProperty("network", "PNI"),
                    new JProperty("disabled", false),
                    new JProperty("latitude", 33),
                    new JProperty("longitude", -111)
                );
                await sensorMethods.InsertSensor(json.ToString());
                Console.WriteLine("Sensor Insert Success");
                Console.WriteLine();


                // Update that new sensor
                Console.WriteLine("Testing '/api/sensor/update'");
                json = new JObject(
                    new JProperty("sensorId", sensorId),
                    new JProperty("parkingSpace", "TEST: c#-api-sensor-update"),
                    new JProperty("latitude", 33.810280507079874),
                    new JProperty("longitude", -117.9189795255661)
                );
                await sensorMethods.UpdateSensor(json.ToString());
                Console.WriteLine("Sensor Update Success");
                Console.WriteLine();


                // Insert a new gateway
                Console.WriteLine("Testing '/api/gateway/insert'");
                json = new JObject(
                    new JProperty("gatewayMac", "cdef78904321dcb0"),
                    new JProperty("gatewayName", "TEST: C#-api-gateway-insert"),
                    new JProperty("parkingLotId", parkingLotId)
                );
                await gatewayMethods.InsertGateway(json.ToString());
                Console.WriteLine("Gateway Insert Success");


                // Get the gateway Id of the inserted gateway
                gateways = await gatewayMethods.GetGateways();
                string gatewayId = null;
                foreach (var gateway in gateways) {
                    if (gateway.name == "TEST: C#-api-gateway-insert") {
                        gatewayId = gateway.id;
                    }
                }
                Console.WriteLine("ID of inserted gateway: " + gatewayId);
                Console.WriteLine();


                // Update that new gateway
                Console.WriteLine("Testing '/api/gateway/update'");
                json = new JObject(
                    new JProperty("id", gatewayId),
                    new JProperty("gatewayName", "TEST: C#-api-gateway-update")
                );
                await gatewayMethods.UpdateGateway(json.ToString());
                Console.WriteLine("Gateway Update Success");
                Console.WriteLine();


                // Remove that updated gateway
                Console.WriteLine("Testing '/api/gateway/remove'");
                json = new JObject(
                    new JProperty("id", gatewayId)
                );
                await gatewayMethods.RemoveGateway(json.ToString());
                Console.WriteLine("Gateway Remove Success");
                Console.WriteLine();


                // Remove that updated sensor
                Console.WriteLine("Testing '/api/sensor/remove'");
                json = new JObject(
                    new JProperty("sensorId", sensorId)
                );
                await sensorMethods.RemoveSensor(json.ToString());
                Console.WriteLine("Sensor Remove Success");
                Console.WriteLine();


                // Remove that updated parking lot
                Console.WriteLine("Testing '/api/parking-lot/remove'");
                json = new JObject(
                    new JProperty("id", parkingLotId)
                );
                await parkingLotMethods.RemoveParkingLot(json.ToString());
                Console.WriteLine("Parking Lot Remove Success");
                Console.WriteLine();

            } catch (Exception ex) {
                Console.WriteLine("First sample application crashed. Error: " + ex.Message);
            }

            // End of program
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }


        /// <summary>
        /// This function tests non-CRUD sensor API functions and how to
        /// properly call them. Note that while these can be used on a 'test' sensor that
        /// doesn't actually exist, you will want to use this on a real installed sensor.
        /// You can see the results of this test on the Parking Cloud by going to the sensor's
        /// diagnostic information.
        /// The tasks that can be optionally performed are:
        ///   1) Get sensor history at a specific 10 minute interval
        ///   2) Send a recalibrate
        ///   3) Run a full BIST test (give up to 5 minutes for a response)
        ///   4) Send a ping and wait for response (up to 5 minutes)
        ///   5) Send a force vacant
        ///   6) Send a force occupied
        ///   7) Send an enable transition state reporting
        ///   8) Send a disable transition state reporting
        ///   9) Send a set lora wakeup interval to 5 minutes
        ///  10) Send a set loRa Tx power call to 11dB.
        ///  11) Send a set Tx spreading factor call to SF 7, BW 125 kHz
        ///  12) Send a set frequency sub band call to 902.3 kHz - 903.7 kHz - 125k
        /// Please note that steps 10, 11, 12 are potentially dangerous if set incorrectly and
        /// may cause the placePod to go offline permanently. Please read available documentation
        /// on the API swagger page first!
        /// 
        /// NOTE: There are only so many requests that can be queued by a sensor at a time, so it
        /// is suggested to not make rapid calls to the sensor and wait around a minute before 
        /// sending another call to the same sensor.
        /// </summary>
        /// <param name="sensorId">ID of the sensor that the operations will be performed on</param>
        static void SecondApp(string sensorId)
        {
            Console.WriteLine("Running operations using sensor: " + sensorId);
            string json = null;

            /// Test /api/sensor/history
            Console.WriteLine("Get Sensor History (y/n)? ");
            var input = Console.ReadLine();
            if (input == "y" || input == "Y")
            {
                json = "{" +
                    " 'sensorId': '" + sensorId + "', " +
                    " 'startTime': '2017-09-08T01:00:00.000Z', " + // startTime and endTime are in ISO form.
                    " 'endTime': '2017-09-08T01:10:00.000Z' " +    // Modify these as needed to when your sensor
                "}";                                               // has been online.
                try
                {
                    var history = SensorHistory(json);
                    Console.WriteLine("Number of results: " + history.Count);
                } catch { /* Eat exception */ }
            }

            json = "{" +
                " 'sensorId': '" + sensorId + "', " +
            "}";

            /// Test /api/sensor/recalibrate
            Console.WriteLine("Recalibrate sensor (y/n)? ");
            input = Console.ReadLine();
            if (input == "y" || input == "Y")
            {
                try { Recalibrate(json); }
                catch { /* Eat exception */ }
            }
                
            /// Test /api/initialize-bist and /api/sensor/bist-response/{SensorId}/{LastUpdated}
            Console.WriteLine("Run basic internal self test (BIST) (y/n)?");
            input = Console.ReadLine();
            if (input == "y" || input == "Y")
            {
                try { Bist(json, sensorId); }
                catch { /* Eat exception */ }
            }

            /// Test /api/sensor/ping and /api/sensor/ping-response/{SensorId}/{LastUpdated}
            Console.WriteLine("Ping sensor (y/n)?");
            input = Console.ReadLine();
            if (input == "y" || input == "Y")
            {
                try { Ping(json, sensorId); }
                catch { /* Eat exception */ }
            }

            /// Test /api/sensor/force-vacant
            Console.WriteLine("Force car presence to vacant (y/n)?");
            input = Console.ReadLine();
            if (input == "y" || input == "Y")
            {
                try { ForceVacant(json); }
                catch { /* Eat exception */ }
            }

            /// Test /api/sensor/force-occupied
            Console.WriteLine("Force car presence to occupied (y/n)?");
            input = Console.ReadLine();
            if (input == "y" || input == "Y")
            {
                try { ForceOccupied(json); }
                catch { /* Eat exception */ }
            }

            /// Test /api/sensor/enable-transition-state-reporting
            Console.WriteLine("Enable transition state reporting (y/n)?");
            input = Console.ReadLine();
            if (input == "y" || input == "Y")
            {
                try { EnableTransitionStateReporting(json); }
                catch { /* Eat exception */ }
            }

            /// Test /api/sensor/disable-transition-state-reporting
            Console.WriteLine("Disable transition state reporting (y/n)?");
            input = Console.ReadLine();
            if (input == "y" || input == "Y")
            {
                try { DisableTransitionStateReporting(json); }
                catch { /* Eat exception */ }
            }

            /// Test /api/sensor/set-lora-wakeup-interval
            json = "{" +
                " 'sensorId': '" + sensorId + "', " +
                " 'payload': 5" +  // Payload is in minutes (int)
            "}";
            Console.WriteLine("Set loRa wakeup interval to 5 minutes (y/n)?");
            input = Console.ReadLine();
            if (input == "y" || input == "Y")
            {
                try { SetLoraWakeupInterval(json); }
                catch { /* Eat exception */ }
            }

            /// Test /api/sensor/set-lora-tx-power
            json = "{" +
                " 'sensorId': '" + sensorId + "', " +
                " 'payload': 11" +  // Payload is an int (1-30)
            "}";
            Console.WriteLine("Set loRa Tx Power to 11 (y/n)?");
            input = Console.ReadLine();
            if (input == "y" || input == "Y")
            {
                try { SetLoraTxPower(json); }
                catch { /* Eat exception */ }
            }

            /// Test /api/sensor/set-tx-spreading-factor
            json = "{" +
                " 'sensorId': '" + sensorId + "', " +
                " 'payload': 6" +  // Payload is an int (see API documentation)
            "}";
            Console.WriteLine("Set Tx Spread Factor to SF 7, BW 125 kHz (y/n)?");
            input = Console.ReadLine();
            if (input == "y" || input == "Y")
            {
                try { SetTxSpreadingFactor(json); }
                catch { /* Eat exception */ }
            }

            /// Test /api/sensor/set-frequency-sub-band
            json = "{" +
                " 'sensorId': '" + sensorId + "', " +
                " 'payload': 1" +  // Payload is an int (see API documentation)
            "}";
            Console.WriteLine("Set Frequency sub band to 902.3 kHz - 903.7 kHz - 125k");
            input = Console.ReadLine();
            if (input == "y" || input == "Y")
            {
                try { SetFrequencySubBand(json); }
                catch { /* Eat exception */ }
            }

            // End of program
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }





        //          HTTP Methods


        /// <summary>
        /// Method for making a HTTP 'Get' request to the API
        /// </summary>
        /// <param name="path">Route for the API method</param>
        /// <returns>Result of the request</returns>
        static string Get(string path) {
            WebRequest req = WebRequest.Create(API_SERVER + path);
            UTF8Encoding enc = new UTF8Encoding();

            req.Method = "GET";
            req.Headers.Add(string.Format("X-API-KEY: {0}", API_KEY));

            try {   //Get the response
                WebResponse res = req.GetResponseAsync().Result;
                Stream receiveStream = res.GetResponseStream();
                StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
                return reader.ReadToEnd();
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.InnerException);
                throw;
            }
        }


        /// <summary>
        /// Method for making a HTTP 'Post' request to the API
        /// </summary>
        /// <param name="path">Route for the API method</param>
        /// <returns>Result of the request</returns>
        static string Post(string path, string data) {
            WebRequest req = WebRequest.Create(API_SERVER + path);
            UTF8Encoding enc = new UTF8Encoding();

            req.Method = "POST";
            req.ContentType = "application/json";
            req.Headers.Add(string.Format("X-API-KEY: {0}", API_KEY));

            var dataStream = req.GetRequestStreamAsync().Result; // You can call .Result on a Task to wait for the result. or if it returns null use wait()
            dataStream.Write(enc.GetBytes(data), 0, data.Length);

            try {   //Get the response
                WebResponse res = req.GetResponseAsync().Result;
                Stream receiveStream = res.GetResponseStream();
                StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
                return reader.ReadToEnd();
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.InnerException);
                throw;
            }
        }


        /// <summary>
        /// Method for making a HTTP 'Put' request to the API
        /// </summary>
        /// <param name="path">Route for the API method</param>
        /// <returns>Result of the request</returns>
        static string Put(string path, string data) {
            WebRequest req = WebRequest.Create(API_SERVER + path);
            UTF8Encoding enc = new UTF8Encoding();

            req.Method = "Put";
            req.ContentType = "application/json";
            req.Headers.Add(string.Format("X-API-KEY: {0}", API_KEY));

            var dataStream = req.GetRequestStreamAsync().Result; // You can call .Result on a Task to wait for the result. or if it returns null use wait()
            dataStream.Write(enc.GetBytes(data), 0, data.Length);

            try {   //Get the response
                WebResponse res = req.GetResponseAsync().Result;
                Stream receiveStream = res.GetResponseStream();
                StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
                return reader.ReadToEnd();
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.InnerException);
                throw;
            }
        }


        /// <summary>
        /// Method for making a HTTP 'Delete' request to the API
        /// </summary>
        /// <param name="path">Route for the API method</param>
        /// <returns>Result of the request</returns>
        static string Delete(string path, string data) {
            WebRequest req = WebRequest.Create(API_SERVER + path);
            UTF8Encoding enc = new UTF8Encoding();

            req.Method = "Delete";
            req.ContentType = "application/json";
            req.Headers.Add(string.Format("X-API-KEY: {0}", API_KEY));

            var dataStream = req.GetRequestStreamAsync().Result; // You can call .Result on a Task to wait for the result. or if it returns null use wait()
            dataStream.Write(enc.GetBytes(data), 0, data.Length);

            try {   //Get the response
                WebResponse res = req.GetResponseAsync().Result;
                Stream receiveStream = res.GetResponseStream();
                StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
                return reader.ReadToEnd();
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.InnerException);
                throw;
            }
        }




        /// <summary>
        /// Get sensor history.
        /// Route: '/api/sensor/history'
        /// </summary>
        /// <param name="filter">JSON string with start/end date and optional fields</param>
        /// <returns>An array of sensor time objects</returns>
        static dynamic SensorHistory(string filter)
        {
            Console.WriteLine("Fetching Sensor History");
            dynamic result = Post("/api/sensor/history", filter);
            return JsonConvert.DeserializeObject(result);
        }


        /// <summary>
        /// Send a recalibrate to a sensor
        /// Route: '/api/sensor/recalibrate'
        /// </summary>
        /// <param name="json">JSON string</param>
        static void Recalibrate(string json)
        {
            Console.WriteLine("Sending Recalibrate...");
            dynamic result = Post("/api/sensor/recalibrate", json);
            Console.WriteLine("Recalibrate Sent");
        }

        /// <summary>
        /// First sends a BIST initialization request to the sensor, then checks
        /// for the sensor response every second over the course of 5 minutes.
        /// Routes: '/api/sensor/initialize-bist' and '/api/sensor/bist-response/{SensorId}/{LastUpdated}'
        /// </summary>
        /// <param name="json">JSON string</param>
        /// <param name="sensorId">sensorId, but not in JSON form.</param>
        static void Bist(string json, string sensorId)
        {
            Console.WriteLine("Sending BIST...");
            var now = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");

            // Make the post call
            dynamic result = Post("/api/sensor/initialize-bist", json);
            Console.WriteLine("BIST Sent", now);

            // We want to call the bist-response every second for 5 minutes
            // or until a response comes back.
            var timer = 0;
            while (timer < 300)
            {
                result = Get("/api/sensor/bist-response/" + sensorId + "/" + now);
                if (result != "[]")
                    break;

                Console.WriteLine("Waiting for Bist response " + (++timer));
                System.Threading.Thread.Sleep(1000);
                now = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
            }

            if (timer >= 300)
            {
                Console.WriteLine("No response...");
                return;
            }

            Console.WriteLine("BIST response recieved!");
            var jsonObj =  JsonConvert.DeserializeObject(result);
            foreach (var i in jsonObj)
                Console.WriteLine("--> " + i.sensorType + ": " + i.status);
        }

        /// <summary>
        /// First sends a Ping initialization request to the sensor, then checks
        /// for the sensor response every second over the course of 5 minutes.
        /// Routes: '/api/sensor/ping' and '/api/sensor/ping-response/{SensorId}/{LastUpdated}'
        /// </summary>
        /// <param name="json">JSON string</param>
        /// <param name="sensorId">sensorId, but not in JSON form.</param>
        static void Ping(string json, string sensorId)
        {
            Console.WriteLine("Sending Ping...");
            var now = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");

            // Make the post call
            dynamic result = Post("/api/sensor/ping", json);
            Console.WriteLine("Ping Sent", now, result);

            // We want to call the ping-response every second for 5 minutes
            // or until a response comes back.
            var timer = 0;
            while (timer < 300)
            {
                result = Get("/api/sensor/ping-response/" + sensorId + "/" + now);
                if (result != "[]")
                    break;

                Console.WriteLine("Waiting for Ping response " + (++timer));
                System.Threading.Thread.Sleep(1000);
                now = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
            }

            if (timer >= 300)
            {
                Console.WriteLine("No response...");
                return;
            }

            Console.WriteLine("Ping response recieved!");
            var jsonObj = JsonConvert.DeserializeObject(result);
            foreach (var i in jsonObj)
                Console.WriteLine("--> Ping RSSI: " + i.pingRssi + ", Ping SNR: " + i.pingSNR + ". Server time: " + i.serverTime);
        }


        /// <summary>
        /// Forces sensor's car presence state to vacant.
        /// Route: '/api/sensor/force-vacant'
        /// </summary>
        /// <param name="json">JSON string</param>
        static void ForceVacant(string json)
        {
            Console.WriteLine("Sending Force Vacant...");
            dynamic result = Post("/api/sensor/force-vacant", json);
            Console.WriteLine("Force Vacant Sent");
        }


        /// <summary>
        /// Forces sensor's car presence state to occupied.
        /// Route: '/api/sensor/force-occupied'
        /// </summary>
        /// <param name="json">JSON string</param>
        static void ForceOccupied(string json)
        {
            Console.WriteLine("Sending Force Occupied...");
            dynamic result = Post("/api/sensor/force-occupied", json);
            Console.WriteLine("Force Occupied Sent");
        }


        /// <summary>
        /// Turns on state reporting for car entering/leaving.
        /// Route: '/api/sensor/enable-transition-state-reporting'
        /// </summary>
        /// <param name="json">JSON string</param>
        static void EnableTransitionStateReporting(string json)
        {
            Console.WriteLine("Sending enable transition state reporting...");
            dynamic result = Post("/api/sensor/enable-transition-state-reporting", json);
            Console.WriteLine("Enable transition state reporting Sent");
        }


        /// <summary>
        /// Turns off state reporting for a car entering/leaving.
        /// Route: '/api/sensor/disable-transition-state-reporting'
        /// </summary>
        /// <param name="json">JSON string</param>
        static void DisableTransitionStateReporting(string json)
        {
            Console.WriteLine("Sending disable transition state reporting...");
            dynamic result = Post("/api/sensor/disable-transition-state-reporting", json);
            Console.WriteLine("Disable transition state reporting Sent");
        }


        /// <summary>
        /// Sets the time period in minutes for how long the sensor will sleep before waking up.
        /// Route: '/api/sensor/set-lora-wakeup-interval'
        /// </summary>
        /// <param name="json">JSON string</param>
        static void SetLoraWakeupInterval(string json)
        {
            Console.WriteLine("Sending set loRa wakeup interval...");
            dynamic result = Post("/api/sensor/set-lora-wakeup-interval", json);
            Console.WriteLine("Set loRa wakeup interval Sent");
        }


        /// <summary>
        /// Using an integer between 1 and 30, sets the loRa Tx power.
        /// Route: '/api/sensor/set-lora-tx-power'
        /// </summary>
        /// <param name="json">JSON string</param>
        static void SetLoraTxPower(string json)
        {
            Console.WriteLine("Sending set loRa Tx Power...");
            dynamic result = Post("/api/sensor/set-lora-tx-power", json);
            Console.WriteLine("Set loRa Tx Power sent");
        }


        /// <summary>
        /// Using an integer based on the chart available on the API's Swagger page,
        /// adjusts the sensor's Tx spreading factor.
        /// Route: '/api/sensor/set-tx-spreading-factor'
        /// </summary>
        /// <param name="json">JSON string</param>
        static void SetTxSpreadingFactor(string json)
        {
            Console.WriteLine("Sending set Tx spreading factor...");
            dynamic result = Post("/api/sensor/set-tx-spreading-factor", json);
            Console.WriteLine("Set Tx spreading factor sent");
        }


        /// <summary>
        /// Using an integer based on the chart available on the API's Swagger page,
        /// adjusts the sensor's frequency sub band.
        /// Route: '/api/sensor/set-frequency-sub-band'
        /// </summary>
        /// <param name="json">JSON string</param>
        static void SetFrequencySubBand(string json)
        {
            Console.WriteLine("Sending set requency Sub Band...");
            dynamic result = Post("/api/sensor/set-frequency-sub-band", json);
            Console.WriteLine("Set frequency sub band sent");
        }
    }
}