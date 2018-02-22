using System;
using PlacePodApiClient;
using PlacePodApiClient.API_Methods;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace PlacePodApiExample {

    /// <summary>
    /// 
    /// Implemented using Placepod API V1.1
    /// Last updated: February 22nd, 2018
    /// 
    /// The placepod API is undocumented and subject to change
    /// Fully documented API comming soon!
    /// 
    /// </summary>
    /// <author>Byron Whitlock bwhitlock@pnicorp.com</author>
    /// <author>Scott Williams swilliams@pnicorp.com</author>
    public class Program {

        // Rest API is documented at https://api.pnicloud.com

        // To get these values:
        //   1) login to PNI cloud account at https://parking.pnicloud.com
        //   2) click on settings > REST API 
        //   3) Click GENERATE API KEY 
        //   4) Copy the API URL and the API key into the below values
        private static readonly string API_SERVER = "";
        private static readonly string API_KEY = "";


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

            
            // Program 2
            Console.WriteLine("This second sample application will test the other 'sensor' operaions. " +
                "A sensor ID must be provided to proceed.");
            Console.WriteLine("Run second sample application (y/n)? ");
            input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                Console.WriteLine("Enter sensor ID: ");
                input = Console.ReadLine();
                Task.Run(async () => {
                    await SecondApp(input);
                }).GetAwaiter().GetResult();
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
                Console.WriteLine("ID of inserted parking lot: " + parkingLotId + "\n");


                // Update that new parking lot
                Console.WriteLine("Testing '/api/parking-lot/update'");
                json = new JObject(
                    new JProperty("id", parkingLotId),
                    new JProperty("parkingLotName", "TEST: C#-api-lot-update")
                );
                await parkingLotMethods.UpdateParkingLot(json.ToString());
                Console.WriteLine("Parking Lot Update Success" + "\n");


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
                Console.WriteLine("Sensor Insert Success" + "\n");


                // Update that new sensor
                Console.WriteLine("Testing '/api/sensor/update'");
                json = new JObject(
                    new JProperty("sensorId", sensorId),
                    new JProperty("parkingSpace", "TEST: c#-api-sensor-update"),
                    new JProperty("latitude", 33.810280507079874),
                    new JProperty("longitude", -117.9189795255661)
                );
                await sensorMethods.UpdateSensor(json.ToString());
                Console.WriteLine("Sensor Update Success" + "\n");


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
                Console.WriteLine("ID of inserted gateway: " + gatewayId + "\n");


                // Update that new gateway
                Console.WriteLine("Testing '/api/gateway/update'");
                json = new JObject(
                    new JProperty("id", gatewayId),
                    new JProperty("gatewayName", "TEST: C#-api-gateway-update")
                );
                await gatewayMethods.UpdateGateway(json.ToString());
                Console.WriteLine("Gateway Update Success" + "\n");


                // Remove that updated gateway
                Console.WriteLine("Testing '/api/gateway/remove'");
                json = new JObject(
                    new JProperty("id", gatewayId)
                );
                await gatewayMethods.RemoveGateway(json.ToString());
                Console.WriteLine("Gateway Remove Success" + "\n");


                // Remove that updated sensor
                Console.WriteLine("Testing '/api/sensor/remove'");
                json = new JObject(
                    new JProperty("sensorId", sensorId)
                );
                await sensorMethods.RemoveSensor(json.ToString());
                Console.WriteLine("Sensor Remove Success" + "\n");


                // Remove that updated parking lot
                Console.WriteLine("Testing '/api/parking-lot/remove'");
                json = new JObject(
                    new JProperty("id", parkingLotId)
                );
                await parkingLotMethods.RemoveParkingLot(json.ToString());
                Console.WriteLine("Parking Lot Remove Success" + "\n");

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
        public async static Task SecondApp(string sensorId) {
            SensorMethods sensorMethods = new SensorMethods(HttpClient);

            Console.WriteLine("Running operations using sensor: " + sensorId);
            JObject json;


            // Fetch sensor history within the timespan
            Console.WriteLine("Test /api/sensor/history");
            Console.WriteLine("Get Sensor History (y/n)? ");
            var input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                // startTime and endTime are in ISO form. Modify these as needed to when your sensor has been online.
                json = new JObject(
                    new JProperty("sensorId", sensorId),
                    new JProperty("startTime", "2017-12-08T01:00:00.000Z"),
                    new JProperty("endTime", "2017-12-08T01:10:00.000Z")
                );
                try {
                    var history = await sensorMethods.SensorHistory(json.ToString());
                    Console.WriteLine("Number of results: " + history.Count + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }

            json = new JObject(
                new JProperty("sensorId", sensorId)
            );


            // Send down a recalibrate request
            Console.WriteLine("Test /api/sensor/recalibrate");
            Console.WriteLine("Recalibrate sensor (y/n)? ");
            input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                try {
                    Console.WriteLine("Sending Recalibrate request...");

                    await sensorMethods.Recalibrate(json.ToString());

                    Console.WriteLine("Recalibrate Sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }


            // Run a full BIST test that waits up to 5 minutes for the results
            Console.WriteLine("Test /api/initialize-bist and /api/sensor/bist-response/{SensorId}/{LastUpdated}");
            Console.WriteLine("Run basic internal self test (BIST) (y/n)?");
            input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                try {
                    Console.WriteLine("Sending BIST request...");

                    dynamic result = await sensorMethods.Bist(json.ToString(), sensorId);
                    foreach (var i in result) {
                        Console.WriteLine("--> " + i.sensorType + ": " + i.status);
                    }
                    Console.WriteLine();
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }


            // Send down a Ping request and wait up to 5 minutes for the response
            Console.WriteLine("Test /api/sensor/ping and /api/sensor/ping-response/{SensorId}/{LastUpdated}");
            Console.WriteLine("Ping sensor (y/n)?");
            input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                try {
                    Console.WriteLine("Sending Ping request...");

                    dynamic result = await sensorMethods.Ping(json.ToString(), sensorId);
                    foreach (var i in result) {
                        Console.WriteLine("--> Ping RSSI: " + i.pingRssi + ", Ping SNR: " + i.pingSNR + ". Server time: " + i.serverTime);
                    }
                    Console.WriteLine();
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }

            
            // Force the sensor's Car Presence to say vacant
            Console.WriteLine("Test /api/sensor/force-vacant");
            Console.WriteLine("Force car presence to vacant (y/n)?");
            input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                try {
                    Console.WriteLine("Sending Force Vacant...");

                    await sensorMethods.ForceVacant(json.ToString());

                    Console.WriteLine("Force Vacant Sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }


            // Force the sensor's Car Presence to say occcupied
            Console.WriteLine("Test /api/sensor/force-occupied");
            Console.WriteLine("Force car presence to occupied (y/n)?");
            input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                try {
                    Console.WriteLine("Sending Force Occupied...");

                    await sensorMethods.ForceOccupied(json.ToString());

                    Console.WriteLine("Force Occupied Sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }


            // Enables transition state reporting for the sensor
            Console.WriteLine("Test /api/sensor/enable-transition-state-reporting");
            Console.WriteLine("Enable transition state reporting (y/n)?");
            input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                try {
                    Console.WriteLine("Sending enable transition state reporting...");

                    await sensorMethods.EnableTransitionStateReporting(json.ToString());

                    Console.WriteLine("Enable transition state reporting Sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }


            // Disables transition state reporting for the sensor
            Console.WriteLine("Test /api/sensor/disable-transition-state-reporting");
            Console.WriteLine("Disable transition state reporting (y/n)?");
            input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                try {
                    Console.WriteLine("Sending disable transition state reporting...");

                    await sensorMethods.DisableTransitionStateReporting(json.ToString());

                    Console.WriteLine("Disable transition state reporting Sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }


            // Sensor will report every 5 minutes
            Console.WriteLine("Test /api/sensor/set-lora-wakeup-interval");
            Console.WriteLine("Set LoRa wakeup interval to 5 minutes (y/n)?");
            input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                try {
                    // Payload is in minutes (int)
                    json = new JObject(
                        new JProperty("sensorId", sensorId),
                        new JProperty("payload", 5)   
                    );
                    Console.WriteLine("Sending set LoRa wakeup interval...");

                    await sensorMethods.SetLoraWakeupInterval(json.ToString());

                    Console.WriteLine("Set LoRa wakeup interval Sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }


            // Change the sensor's LoRa Tx Power to 11
            Console.WriteLine("Test /api/sensor/set-lora-tx-power");
            Console.WriteLine("Set LoRa Tx Power to 11 (y/n)?");
            input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                try {
                    // Payload is an int (1-30)
                    json = new JObject(
                        new JProperty("sensorId", sensorId),
                        new JProperty("payload", 11)
                    );
                    Console.WriteLine("Sending set LoRa Tx Power...");

                    await sensorMethods.SetLoraTxPower(json.ToString());

                    Console.WriteLine("Set LoRa Tx Power sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }

            // Change the sensor's spread factor to 7
            Console.WriteLine("Test /api/sensor/set-tx-spreading-factor");
            Console.WriteLine("Set Tx Spread Factor to SF 7, BW 125 kHz (y/n)?");
            input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                try {
                    // Payload is an int (see API documentation)
                    json = new JObject(
                        new JProperty("sensorId", sensorId),
                        new JProperty("payload", 6)
                    );
                    Console.WriteLine("Sending set Tx spreading factor...");

                    await sensorMethods.SetTxSpreadingFactor(json.ToString());

                    Console.WriteLine("Set Tx spreading factor sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }

            /// Test /api/sensor/set-frequency-sub-band

            Console.WriteLine("Set Frequency sub band to 902.3 kHz - 903.7 kHz - 125k");
            input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                try {
                    // Payload is an int (see API documentation)
                    json = new JObject(
                        new JProperty("sensorId", sensorId),
                        new JProperty("payload", 1)
                    );
                    Console.WriteLine("Sending set requency Sub Band...");

                    await sensorMethods.SetFrequencySubBand(json.ToString());

                    Console.WriteLine("Set frequency sub band sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }

            // End of program
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}