using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlacePodApiClient.Api;
using PlacePodApiClient.Helpers;
using PlacePodApiClient.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace PlacePodApiClient {

    /// <summary>
    /// This application tests the Get, Insert, Update, and Remove operations
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
    internal static class FirstApp {

        /// <summary>
        /// Start the first application
        /// </summary>
        /// <returns></returns>
        public static void Run() {
            Console.WriteLine("This first sample application will test the get, insert, update, and remove functions" +
                " of 'gateways', 'parking-lots', and sensors'.");

            Console.WriteLine("Run first sample application (y/n)? ");
            string input = Console.ReadLine();
            if (input != "y" && input != "Y") {
                return;
            }
            Task.Run(async () => {

                // All of this is wrapped in a try/catch because if one operation fails it
                // can cause the rest of the first app to not behave as desired.
                try {
                    await GetParkingLots();
                    await GetSensors();
                    await GetGateways();

                    string lotId = await InsertParkingLot();
                    await UpdateParkingLot(lotId);

                    string sensorId = "abcd12340987fed0";
                    await InsertSensor(sensorId, lotId);
                    await UpdateSensor(sensorId);

                    string gatewayId = await InsertGateway(lotId);
                    await UpdateGateway(gatewayId);

                    await RemoveGateway(gatewayId);
                    await RemoveSensor(sensorId);
                    await RemoveParkingLot(lotId);
                } catch (Exception ex) {
                    Console.WriteLine("First sample application crashed. Error: " + ex.Message);
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }).GetAwaiter().GetResult();
        }


        /// <summary>
        /// Fetch all of the parking lots and print them out
        /// </summary>
        private static async Task GetParkingLots() {
            Console.WriteLine("Testing '/api/parking-lots'");

            List<ParkingLot> parkingLots = await ParkingLotApi.GetParkingLots();
            Console.WriteLine("Got " + parkingLots.Count + " Parking Lots:");

            foreach (ParkingLot parkingLot in parkingLots) {
                Console.WriteLine("--> " + parkingLot.Id + ": " + parkingLot.Name);
            }
            Console.WriteLine();
        }


        /// <summary>
        /// Fetch all of the sensors and print them out
        /// </summary>
        private static async Task GetSensors() {
            Console.WriteLine("Testing '/api/sensors'");

            List<Sensor> sensors = await SensorApi.GetSensors("{}");
            Console.WriteLine("Got " + sensors.Count + " Sensors:");

            foreach (Sensor sensor in sensors) {
                Console.WriteLine("--> " + sensor.SensorId + ": " + sensor.ParkingSpace + ", " + sensor.Status + ", " + sensor.ParkingLot);
            }
            Console.WriteLine();
        }


        /// <summary>
        /// Fetch all of the gateways and print them out
        /// </summary>
        private static async Task GetGateways() {
            Console.WriteLine("Testing '/api/gateways'");

            List<Gateway> gateways = await GatewayApi.GetGateways();
            Console.WriteLine("Got " + gateways.Count + " Gateways:");

            foreach (Gateway gateway in gateways) {
                Console.WriteLine("--> " + gateway.GatewayMac + ": " + gateway.Name);
            }
            Console.WriteLine();
        }


        /// <summary>
        /// Insert a new parking lot named "TEST: C#-api-lot-insert"
        /// </summary>
        /// <returns>New parking lot's Id</returns>
        private static async Task<string> InsertParkingLot() {
            Console.WriteLine("Testing '/api/parking-lot/insert'");

            const string parkingLotName = "TEST: C#-api-lot-insert";

            // Sample JSON to send
            JObject json = new JObject {
                ["parkingLotName"] = parkingLotName,
                ["description"] = "c# client test",
                ["streetAddress"] = "123 here",
                ["latitude"] = 33.810280507079874,
                ["longitude"] = -117.9189795255661
            };

            await ParkingLotApi.InsertParkingLot(json.ToString());
            Console.WriteLine("Parking Lot Insert Success");

            // Get the parkingLotId of the inserted lot
            List<ParkingLot> parkingLots = await ParkingLotApi.GetParkingLots();

            // Ideally the PlacePod API will include the id in the response message at a later time
            // so that we don't have to to a O(n) lookup...
            string parkingLotId = null;
            foreach (ParkingLot parkingLot in parkingLots) {
                if (parkingLot.Name == parkingLotName) {
                    parkingLotId = parkingLot.Id;
                    break;
                }
            }
            Console.WriteLine("ID of inserted parking lot: " + parkingLotId + "\n");
            return parkingLotId;
        }


        /// <summary>
        /// Update that new parking lot's name to "TEST: C#-api-lot-update"
        /// </summary>
        /// <param name="parkingLotId">Id of the new parking lot</param>
        private static async Task UpdateParkingLot(string parkingLotId) {
            Console.WriteLine("Testing '/api/parking-lot/update'");

            // Sample JSON to send
            JObject json = new JObject {
                ["id"] = parkingLotId,
                ["parkingLotName"] = "TEST: C#-api-lot-update"
            };

            await ParkingLotApi.UpdateParkingLot(json.ToString());
            Console.WriteLine("Parking Lot Update Success\n");
        }


        /// <summary>
        /// Insert a new sensor named "TEST: c#-api-sensor-insert"
        /// </summary>
        /// <param name="sensorId">The Id of the sensor to insert</param>
        /// <param name="parkingLotId">The Id of the parking lot inserted earlier</param>
        private static async Task InsertSensor(string sensorId, string parkingLotId) {
            Console.WriteLine("Testing '/api/sensor/insert'");

            // Sample JSON to send
            JObject json = new JObject {
                ["sensorId"] = sensorId,
                ["parkingSpace"] = "TEST: c#-api-sensor-insert",
                ["parkingLotId"] = parkingLotId,
                ["network"] = "PNI",
                ["appEui"] = "0000000000000000",
                ["appKey"] = "00000000000000000000000000000000",
                ["disabled"] = false,
                ["latitude"] = 33,
                ["longitude"] = -111
            };

            await SensorApi.InsertSensor(json.ToString());
            Console.WriteLine("Sensor Insert Success\n");
        }


        /// <summary>
        /// Update that new sensor's name to "TEST: c#-api-sensor-update". Also update its location.
        /// </summary>
        /// <param name="sensorId">The Id of the inserted sensor</param>
        private static async Task UpdateSensor(string sensorId) {
            Console.WriteLine("Testing '/api/sensor/update'");

            // Sample JSON to send
            JObject json = new JObject {
                ["sensorId"] = sensorId,
                ["parkingSpace"] = "TEST: c#-api-sensor-update",
                ["latitude"] = 33.810280507079874,
                ["longitude"] = -117.9189795255661
            };

            await SensorApi.UpdateSensor(json.ToString());
            Console.WriteLine("Sensor Update Success\n");
        }


        /// <summary>
        /// Insert a new gateway named "TEST: C#-api-gateway-insert"
        /// </summary>
        /// <param name="parkingLotId">The Id of the parking lot inserted earlier</param>
        /// <returns>New gateway's Id</returns>
        private static async Task<string> InsertGateway(string parkingLotId) {
            Console.WriteLine("Testing '/api/gateway/insert'");

            const string gatewayName = "TEST: C#-api-gateway-insert";

            // Sample JSON to send
            JObject json = new JObject {
                ["gatewayMac"] = "cdef78904321dcb0",
                ["gatewayName"] = gatewayName,
                ["parkingLotId"] = parkingLotId
            };

            await GatewayApi.InsertGateway(json.ToString());
            Console.WriteLine("Gateway Insert Success");

            // Get the gateway Id of the inserted gateway
            List<Gateway> gateways = await GatewayApi.GetGateways();

            string gatewayId = null;
            foreach (Gateway gateway in gateways) {
                if (gateway.Name == gatewayName) {
                    gatewayId = gateway.Id;
                    break;
                }
            }
            Console.WriteLine("ID of inserted gateway: " + gatewayId + "\n");
            return gatewayId;
        }


        /// <summary>
        /// Update that new gateway's name to "TEST: C#-api-gateway-update"
        /// </summary>
        /// <param name="gatewayId">The Id of the inserted gateway</param>
        private static async Task UpdateGateway(string gatewayId) {
            Console.WriteLine("Testing '/api/gateway/update'");

            // Sample JSON to send
            JObject json = new JObject {
                ["id"] = gatewayId,
                ["gatewayName"] = "TEST: C#-api-gateway-update"
            };
            
            await GatewayApi.UpdateGateway(json.ToString());
            Console.WriteLine("Gateway Update Success\n");
        }


        /// <summary>
        /// Remove that updated gateway
        /// </summary>
        /// <param name="gatewayId">The Id of the updated gateway</param>
        private static async Task RemoveGateway(string gatewayId) {
            Console.WriteLine("Testing '/api/gateway/remove'");

            // Sample JSON to send
            JObject json = new JObject {
                ["id"] = gatewayId
            };

            await GatewayApi.RemoveGateway(json.ToString());
            Console.WriteLine("Gateway Remove Success\n");
        }


        /// <summary>
        /// Remove that updated sensor
        /// </summary>
        /// <param name="sensorId">The Id of the updated sensor</param>
        private static async Task RemoveSensor(string sensorId) {
            Console.WriteLine("Testing '/api/sensor/remove'");

            // Sample JSON to send
            JObject json = new JObject {
                ["sensorId"] = sensorId
            };

            await SensorApi.RemoveSensor(json.ToString());
            Console.WriteLine("Sensor Remove Success\n");
        }


        /// <summary>
        /// Remove that updated parking lot
        /// </summary>
        /// <param name="parkingLotId">The Id of the updated parking lot</param>
        private static async Task RemoveParkingLot(string parkingLotId) {
            Console.WriteLine("Testing '/api/parking-lot/remove'");

            // Sample JSON to send
            JObject json = new JObject {
                ["id"] = parkingLotId
            };

            await ParkingLotApi.RemoveParkingLot(json.ToString());
            Console.WriteLine("Parking Lot Remove Success\n");
        }
    }
}
