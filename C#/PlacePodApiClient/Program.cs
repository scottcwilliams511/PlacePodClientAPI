using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System;

/// <summary>
/// 
/// Implemented using Placepod API V1.1
/// Last updated: October 26th, 2017
/// 
/// The placepod API is undocumented and subject to change
/// Fully documented API comming soon!
/// 
/// Unline the python application, the actual error message seems to get lost,
/// so all we get here is a internal 500 error if something blows up like 
/// a bad sensor ID. Errors can be confirmed by entering the same request
/// that caused the error on the API's Swagger page.
/// 
/// </summary>
/// <author>Byron Whitlock bwhitlock@pnicorp.com</author>
/// <author>Scott Williams swilliams@pnicorp.com</author>
namespace PlacePodApiExample
{
    class Program
    {
        // Rest API is documented at https://api.pnicloud.com

        // To get these values:
        //   1) login to PNI cloud account at https://parking.pnicloud.com
        //   2) click on settings > REST API 
        //   3) Click GENERATE API KEY 
        //   4) Copy the API URL and the API key into the below values
        static string API_SERVER = "";
        static string API_KEY = "";

        /// <summary>
        /// Main function that initializes the two sample applications
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("This first sample application will test the get, insert, update, and remove functions" +
                " of 'gateways', 'parking-lots', and sensors'.");
            Console.WriteLine("Run first sample application (y/n)? ");
            var input = Console.ReadLine();
            if (input == "y" || input == "Y")
                FirstApp();

            Console.WriteLine("This second sample application will test the other 'sensor' operaions. " +
                "A sensor ID must be provided to proceed.");
            Console.WriteLine("Run second sample application (y/n)? ");
            input = Console.ReadLine();
            if (input == "y" || input == "Y")
            {
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
        /// Since error messages from the API are not available here, you may want to run
        /// the command that crashed the program on the API's swagger page. You will also
        /// want to remove any test data left on your account due to the insert calls.
        /// This can be done on either the API's swagger page or through the Parking Cloud.
        /// </summary>
        static void FirstApp()
        {   // All of this is wrapped in a try/catch because if one operation fails it
            // can cause the rest of the first app to not behaive as desired.
            try
            {   /// Test /api/parking-lots
                dynamic lots = GetParkingLots();
                Console.WriteLine(" Got " + lots.Count + " Parking Lots: ");
                foreach (var lot in lots)
                    Console.WriteLine("--> " + lot.id + ": " + lot.name + " ");
                Console.WriteLine();

                // JSON filter
                var filter = "{}";
                // filter = "{ 'parkingLotId': 'string' }"        // filter on parking lot
                // filter = "{ 'sensorId': '008000000000b0dd'}"   // filter on sensor id

                /// Test /api/sensors
                dynamic sensors = GetSensors(filter);
                Console.WriteLine(" Got " + sensors.Count + " Sensors");
                foreach (var sensor in sensors)
                    Console.WriteLine("--> " + sensor.sensorId + ": " + sensor.parkingSpace + ", " + sensor.status + ", " + sensor.parkingLot);
                Console.WriteLine();

                /// Test /api/gateways
                dynamic gateways = GetGateways();
                Console.WriteLine(" Got " + gateways.Count + " Gateways: ");
                foreach (var gateway in gateways)
                    Console.WriteLine("--> " + gateway.gatewayMac + ": " + gateway.name + " ");
                Console.WriteLine();

                /// Test /api/parking-lot/insert
                var json = "{" +
                    " 'parkingLotName': 'TEST: C#-api-lot-insert', " +
                    " 'description': 'c# client test', " +
                    " 'streetAddress': '123 here', " +
                    " 'latitude': '33.810280507079874', " +
                    " 'longitude': '-117.9189795255661' " +
                "}";
                InsertParkingLot(json);
                lots = GetParkingLots();
                string parkingLotId = null;
                foreach (var lot in lots)
                    if (lot.name == "TEST: C#-api-lot-insert")
                        parkingLotId = lot.id;
                Console.WriteLine("ID of inserted parking lot: " + parkingLotId);

                /// Test /api/parking-lot/update
                json = "{" +
                    " 'id': '" + parkingLotId + "', " +
                    " 'parkingLotName': 'TEST: C#-api-lot-update' " +
                "}";
                UpdateParkingLot(json);

                /// Test /api/sensor/insert
                var sensorId = "abcd12340987fed0";
                json = "{" +
                    " 'sensorId': '" + sensorId + "', " +
                    " 'parkingSpace': 'TEST: c#-api-sensor-insert', " +
                    " 'parkingLotId': '" + parkingLotId + "', " +
                    " 'network': 'PNI', " +
                    " 'disabled': false," +
                    " 'latitude': 33, " +
                    " 'longitude': -111 " +
                "}";
                InsertSensor(json);

                /// Test /api/sensor/update
                json = "{" +
                    " 'sensorId': '" + sensorId + "', " +
                    " 'parkingSpace': 'TEST: c#-api-sensor-update', " +
                    " 'latitude': 33.810280507079874, " +
                    " 'longitude': -117.9189795255661 " +
                "}";
                UpdateSensor(json);

                /// Test /api/gateway/insert
                json = "{" +
                    " 'gatewayMac': 'cdef78904321dcb0', " +
                    " 'gatewayName': 'TEST: C#-api-gateway-insert', " +
                    " 'parkingLotId': '" + parkingLotId + "'" +
                "}";
                InsertGateway(json);
                gateways = GetGateways();
                string gatewayId = null;
                foreach (var gateway in gateways)
                    if (gateway.name == "TEST: C#-api-gateway-insert")
                        gatewayId = gateway.id;
                Console.WriteLine("ID of inserted gateway: " + gatewayId);

                /// Test /api/gateway/update
                json = "{" +
                  " 'id': '" + gatewayId + "', " +
                  " 'gatewayName': 'TEST: C#-api-gateway-update', " +
                "}";
                UpdateGateway(json);

                /// Test /api/gateway/remove
                json = "{" +
                  " 'id': '" + gatewayId + "' " +
                "}";
                RemoveGateway(json);

                /// Test /api/sensor/remove
                json = "{" +
                  " 'sensorId': '" + sensorId + "' " +
                "}";
                RemoveSensor(json);

                /// Test /api/parking-lot/remove
                json = "{" +
                  " 'id': '" + parkingLotId + "' " +
                "}";
                RemoveParkingLot(json);
            } catch { Console.WriteLine("First sample application crashed..."); }

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
        /// <param name="sensorId">ID of the sensorthat the operations will be performed on</param>
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
        static string Get(string path)
        {
            WebRequest req = WebRequest.Create(API_SERVER + path);
            UTF8Encoding enc = new UTF8Encoding();

            req.Method = "GET";
            req.Headers.Add(string.Format("X-API-KEY: {0}", API_KEY));

            try
            {   //Get the response
                WebResponse res = req.GetResponseAsync().Result;
                Stream receiveStream = res.GetResponseStream();
                StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.InnerException);
                throw;
            }
        }


        /// <summary>
        /// Method for making a HTTP 'Post' request to the API
        /// </summary>
        /// <param name="path">Route for the API method</param>
        /// <returns>Result of the request</returns>
        static string Post(string path, string data)
        {
            WebRequest req = WebRequest.Create(API_SERVER + path);
            UTF8Encoding enc = new UTF8Encoding();

            req.Method = "POST";
            req.ContentType = "application/json";
            req.Headers.Add(string.Format("X-API-KEY: {0}", API_KEY));

            var dataStream = req.GetRequestStreamAsync().Result; // You can call .Result on a Task to wait for the result. or if it returns null use wait()
            dataStream.Write(enc.GetBytes(data), 0, data.Length);

            try
            {   //Get the response
                WebResponse res = req.GetResponseAsync().Result;
                Stream receiveStream = res.GetResponseStream();
                StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.InnerException);
                throw;
            }
        }


        /// <summary>
        /// Method for making a HTTP 'Put' request to the API
        /// </summary>
        /// <param name="path">Route for the API method</param>
        /// <returns>Result of the request</returns>
        static string Put(string path, string data)
        {
            WebRequest req = WebRequest.Create(API_SERVER + path);
            UTF8Encoding enc = new UTF8Encoding();

            req.Method = "Put";
            req.ContentType = "application/json";
            req.Headers.Add(string.Format("X-API-KEY: {0}", API_KEY));

            var dataStream = req.GetRequestStreamAsync().Result; // You can call .Result on a Task to wait for the result. or if it returns null use wait()
            dataStream.Write(enc.GetBytes(data), 0, data.Length);

            try
            {   //Get the response
                WebResponse res = req.GetResponseAsync().Result;
                Stream receiveStream = res.GetResponseStream();
                StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.InnerException);
                throw;
            }
        }


        /// <summary>
        /// Method for making a HTTP 'Delete' request to the API
        /// </summary>
        /// <param name="path">Route for the API method</param>
        /// <returns>Result of the request</returns>
        static string Delete(string path, string data)
        {
            WebRequest req = WebRequest.Create(API_SERVER + path);
            UTF8Encoding enc = new UTF8Encoding();

            req.Method = "Delete";
            req.ContentType = "application/json";
            req.Headers.Add(string.Format("X-API-KEY: {0}", API_KEY));

            var dataStream = req.GetRequestStreamAsync().Result; // You can call .Result on a Task to wait for the result. or if it returns null use wait()
            dataStream.Write(enc.GetBytes(data), 0, data.Length);

            try
            {   //Get the response
                WebResponse res = req.GetResponseAsync().Result;
                Stream receiveStream = res.GetResponseStream();
                StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.InnerException);
                throw;
            }
        }


        //          API Operations


        /// <summary>
        /// Get Gateways.
        /// Route: '/api/gateways'
        /// </summary>
        /// <returns>Array of gateways</returns>
        static dynamic GetGateways()
        {
            Console.WriteLine("Fetching Gateways");
            try
            {
                dynamic result = Get("/api/gateways");
                return JsonConvert.DeserializeObject(result);
            }
            catch
            {
                Console.WriteLine("Couldn't get Gateways");
                throw;
            }
        }


        /// <summary>
        /// Insert a new gateway.
        /// Route: '/api/gateway/insert'
        /// </summary>
        /// <param name="json">JSON string</param>
        static void InsertGateway(string json)
        {
            try { dynamic result = Post("/api/gateway/insert", json); }
            catch
            {
                Console.WriteLine("Couldn't Insert Gateway");
                throw;
            }
            Console.WriteLine("Gateway Insert Success");
        }


        /// <summary>
        /// Update an existing gateway.
        /// Route: '/api/gateway/update'
        /// </summary>
        /// <param name="json">JSON string</param>
        static void UpdateGateway(string json)
        {
            try { dynamic result = Put("/api/gateway/update", json); }
            catch
            {
                Console.WriteLine("Couldn't Update Gateway");
                throw;
            }
            Console.WriteLine("Gateway Update Success");
        }


        /// <summary>
        /// Delete an existing gateway.
        /// Route: '/api/gateway/remove'
        /// </summary>
        /// <param name="json">JSON string</param>
        static void RemoveGateway(string json)
        {
            try { dynamic result = Delete("/api/gateway/remove", json); }
            catch
            {
                Console.WriteLine("Couldn't Remove Gateway");
                throw;
            }
            Console.WriteLine("Gateway Remove Success");
        }


        /// <summary>
        /// Get Parking Lots.
        /// Route: '/api/parking-lots'
        /// </summary>
        /// <returns>Array of parking lots</returns>
        static dynamic GetParkingLots()
        {
            Console.WriteLine("Fetching Parking Lots...");
            try
            {
                var result = Get("/api/parking-lots");
                return JsonConvert.DeserializeObject(result);
            }
            catch
            {
                Console.WriteLine("Couldn't get Parking Lots");
                throw;
            }
        }


        /// <summary>
        /// Insert a new parking lot.
        /// Route: '/api/parking-lot/insert'
        /// </summary>
        /// <param name="json">JSON string</param>
        static void InsertParkingLot(string json)
        {
            try { dynamic result = Post("/api/parking-lot/insert", json); }
            catch
            {
                Console.WriteLine("Couldn't Insert Parking Lot");
                throw;
            }
            Console.WriteLine("Parking Lot Insert Success");
        }


        /// <summary>
        /// Updating an existing parking lot.
        /// Route: '/api/parking-lot/update'
        /// </summary>
        /// <param name="json">JSON string</param>
        static void UpdateParkingLot(string json)
        {
            try { dynamic result = Put("/api/parking-lot/update", json); }
            catch
            {
                Console.WriteLine("Couldn't Update Parking Lot");
                throw;
            }
            Console.WriteLine("Parking Lot Update Success");
        }


        /// <summary>
        /// Delete an existing parking lot.
        /// Route: '/api/parking-lot-remove'
        /// </summary>
        /// <param name="json">JSON string</param>
        static void RemoveParkingLot(string json)
        {
            try { dynamic result = Delete("/api/parking-lot/remove", json); }
            catch
            {
                Console.WriteLine("Couldn't Remove Parking Lot");
                throw;
            }
            Console.WriteLine("Parking Lot Remove Success");
        }


        /// <summary>
        /// Get Sensors.
        /// Route: '/api/sensors'
        /// </summary>
        /// <param name="filter">Optional</param>
        /// <returns>Array of sensors</returns>
        static dynamic GetSensors(string filter)
        {
            Console.WriteLine("Fetching Sensors");
            try
            {
                dynamic result = Post("/api/sensors", filter); // all sensors 
                return JsonConvert.DeserializeObject(result);
            }
            catch
            {
                Console.WriteLine("Couldn't get Sensors");
                throw;
            }
        }


        /// <summary>
        /// Insert a new sensor.
        /// Route: '/api/sensor/insert'
        /// </summary>
        /// <param name="json">JSON string</param>
        static void InsertSensor(string json)
        {
            try { dynamic result = Post("/api/sensor/insert", json); }
            catch
            {
                Console.WriteLine("Couldn't Insert Sensor");
                throw;
            }
            Console.WriteLine("Sensor Insert Success");
        }


        /// <summary>
        /// Update an existing sensor.
        /// Route: '/api/sensor/update'
        /// </summary>
        /// <param name="json">JSON string</param>
        static void UpdateSensor(string json)
        {
            try { dynamic result = Put("/api/sensor/update", json); }
            catch
            {
                Console.WriteLine("Couldn't Update Sensor");
                throw;
            }
            Console.WriteLine("Sensor Update Success");
        }


        /// <summary>
        /// Delete an existing sensor.
        /// Route: '/api/sensor/remove'
        /// </summary>
        /// <param name="json">JSON string</param>
        static void RemoveSensor(string json)
        {
            try { dynamic result = Delete("/api/sensor/remove", json); }
            catch
            {
                Console.WriteLine("Couldn't Remove Sensor");
                throw;
            }
            Console.WriteLine("Sensor Remove Success");
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