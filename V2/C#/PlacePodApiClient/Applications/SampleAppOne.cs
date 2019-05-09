using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

using PlacePodApiClient.Api;
using PlacePodApiClient.Lib;
using PlacePodApiClient.Models;

using static System.Console;

namespace PlacePodApiClient.Applications {
    /// <summary>
    /// This application tests the Get, Create, Update, and Delete methods of
    /// "driveways", "sensors", and "parkinglots". Tasks are performed in this order:
    /// 1) Get all parking lots.
    /// 2) Get all driveways.
    /// 3) Get all sensors.
    /// 4) Create a new parking lot and get its id.
    /// 5) Create two new sensors and get their ids.
    /// 6) Create a driveway and get its id.
    /// 7) Get parking lot using the id.
    /// 8) Get sensors using the ids.
    /// 9) Get driveway using the id.
    /// 10) Get all sensors in the parking lot with the given id.
    /// 11) Get all driveways in the parking lot with the given id.
    /// 12) Update the parking lot using its id.
    /// 13) Update on of the sensors using its id.
    /// 14) Update the driveway using its id.
    /// 15) Delete the driveway using its id.
    /// 16) Delete both sensors using their ids.
    /// 17) Delete the parking lot using its id.
    /// 
    /// Note: If all tasks complete, then there will be no test data left on your
    /// account from this test application.
    /// 
    /// If a task fails, the program will stop since several calls rely on other calls.
    /// You will also want to remove any test data left on your account due to the insert calls.
    /// This can be done on either the API's swagger page or through the Parking Cloud.
    /// </summary>
    public class SampleAppOne {

        private readonly DrivewayApi _drivewayApi;

        private readonly ParkingLotApi _parkingLotApi;

        private readonly SensorApi _sensorApi;

        private readonly string _route;


        public SampleAppOne(IHttpAsync http) {
            _drivewayApi = new DrivewayApi(http);
            _parkingLotApi = new ParkingLotApi(http);
            _sensorApi = new SensorApi(http);
            _route = http.BaseRoute;
        }


        /// <summary>
        /// All of this is wrapped in a try/catch because if one operation fails it
        /// can cause the rest of the first app to not behave as desired.
        /// </summary>
        public async Task Run() {
            try {
                /* Get All */
                await GetParkingLots();
                await GetDriveways();
                await GetSensors();

                /* Create */
                string parkingLotId = await CreateParkingLot();
                string frontId      = await CreateSensor("1234567890fedcba", parkingLotId);
                string backId       = await CreateSensor("abcdef0987654321", parkingLotId);
                string drivewayId   = await CreateDriveway(frontId, backId, parkingLotId);

                /* Get by Id */
                await GetMethods(parkingLotId, frontId, backId, drivewayId);

                /* Get by parking lot */
                await ParkingLotGetMethods(parkingLotId);

                /* Update */
                await UpdateParkingLot(parkingLotId);
                await UpdateSensor(frontId);
                await UpdateDriveway(drivewayId);

                /* Delete */
                await DeleteDriveway(drivewayId);
                await DeleteSensor(frontId);
                await DeleteSensor(backId);
                await DeleteParkingLot(parkingLotId);
            } catch (System.Exception ex) {
                WriteLine($"First sample application crashed. Error: {ex.Message}");
            }

            WriteLine("Press any key to continue...");
            ReadKey();
        }


        private async Task GetParkingLots() {
            WriteLine($"Testing GET '{_route}/{ParkingLotApi.Path}'.");

            ICollection<ParkingLot> parkingLots = await _parkingLotApi.Get<ParkingLot>();
            WriteLine($"Got {parkingLots.Count} Parking Lots:");
            foreach (ParkingLot parkingLot in parkingLots) {
                WriteLine(parkingLot.GetPrintString());
            }
            WriteLine();
        }


        private async Task GetDriveways() {
            WriteLine($"Testing GET '{_route}/{DrivewayApi.Path}'.");

            ICollection<Driveway> driveways = await _drivewayApi.Get<Driveway>();
            WriteLine($"Got {driveways.Count} Driveways:");
            foreach (Driveway driveway in driveways) {
                WriteLine(driveway.GetPrintString());
            }
            WriteLine();
        }


        private async Task GetSensors() {
            WriteLine($"Testing GET '{_route}/{SensorApi.Path}'.");

            ICollection<Sensor> sensors = await _sensorApi.Get<Sensor>();
            WriteLine($"Got {sensors.Count} Sensors:");
            foreach (Sensor sensor in sensors) {
                WriteLine(sensor.GetPrintString());
            }
            WriteLine();
        }


        private async Task<string> CreateParkingLot() {
            WriteLine($"Testing POST '{_route}/{ParkingLotApi.Path}'.");

            const string parkingLotName = "TEST: C#-api-lot-create";

            // Sample JSON to send
            JObject json = new JObject {
                ["name"] = parkingLotName,
                ["description"] = "c# client test",
                ["address"] = "123 here",
                ["latitude"] = 33.810280507079874,
                ["longitude"] = -117.9189795255661
            };

            string result = await _parkingLotApi.Create(json.ToString());
            WriteLine("Parking Lot Creation Success\n");

            return JObject.Parse(result)["id"].ToString();
        }


        private async Task<string> CreateSensor(string sensorId, string parkingLotId) {
            WriteLine($"Testing POST '{_route}/{SensorApi.Path}'.");

            // Sample JSON to send
            JObject json = new JObject {
                ["sensorId"] = sensorId,
                ["parkingSpace"] = "TEST: c#-api-sensor-create",
                ["parkingLotId"] = parkingLotId,
                ["network"] = "PNI",
                ["appEui"] = "0000000000000000",
                ["appKey"] = "00000000000000000000000000000000",
                ["disabled"] = false,
                ["latitude"] = 33,
                ["longitude"] = -111
            };

            string result = await _sensorApi.Create(json.ToString());
            WriteLine("Sensor Creation Success\n");

            return JObject.Parse(result)["id"].ToString();
        }


        private async Task<string> CreateDriveway(string frontId, string backId, string parkingLotId) {
            WriteLine($"Testing POST '{_route}/{DrivewayApi.Path}'.");

            JObject json = new JObject {
                ["name"] = "TEST: c#-api-driveway-create",
                ["frontSensorId"] = frontId,
                ["backSensorId"] = backId,
                ["parkingLotId"] = parkingLotId,
                ["isDirectionIn"] = true,
            };

            string result = await _drivewayApi.Create(json.ToString());
            WriteLine("Driveway Creation Success\n");

            return JObject.Parse(result)["id"].ToString();
        }


        private async Task GetMethods(string parkingLotId, string frontId, string backId, string drivewayId) {
            WriteLine($"Testing GET '{_route}/{ParkingLotApi.Path}/{parkingLotId}'.");
            ParkingLot parkingLot = await _parkingLotApi.Get<ParkingLot>(parkingLotId);

            WriteLine($"Testing GET '{_route}/{SensorApi.Path}/{frontId}'.");
            Sensor frontSensor = await _sensorApi.Get<Sensor>(frontId);

            WriteLine($"Testing GET '{_route}/{SensorApi.Path}/{backId}'.");
            Sensor backSensor = await _sensorApi.Get<Sensor>(backId);

            WriteLine($"Testing GET '{_route}/{DrivewayApi.Path}/{drivewayId}'.");
            Driveway driveway = await _drivewayApi.Get<Driveway>(drivewayId);

            WriteLine("Objects that were just created:");
            WriteLine($"--> Parking Lot  {parkingLot.GetPrintString()}");
            WriteLine($"--> Front Sensor {frontSensor.GetPrintString()}");
            WriteLine($"--> Back Sensor  {backSensor.GetPrintString()}");
            WriteLine($"--> Driveway     {driveway.GetPrintString()}");
            WriteLine();
        }


        private async Task ParkingLotGetMethods(string id) {
            WriteLine($"Testing GET '{_route}/{ParkingLotApi.Path}/{id}/{SensorApi.Path}'.");

            ICollection<Sensor> sensors = await _parkingLotApi.GetSensors(id);
            WriteLine($"Got {sensors.Count} Sensors:");
            foreach (Sensor sensor in sensors) {
                WriteLine(sensor.GetPrintString());
            }
            WriteLine();

            WriteLine($"Testing GET '{_route}/{ParkingLotApi.Path}/{id}/{DrivewayApi.Path}'.");

            ICollection<Driveway> driveways = await _parkingLotApi.GetDriveways(id);
            WriteLine($"Got {driveways.Count} Driveways:");
            foreach (Driveway driveway in driveways) {
                WriteLine(driveway.GetPrintString());
            }
            WriteLine();
        }


        private async Task UpdateParkingLot(string id) {
            WriteLine($"Testing PUT '{_route}/{SensorApi.Path}/{id}'.");

            // Sample JSON to send
            JObject json = new JObject {
                ["name"] = "TEST: C#-api-lot-update"
            };

            await _parkingLotApi.Update(id, json.ToString());
            WriteLine("Parking Lot Update Success\n");
        }


        private async Task UpdateSensor(string id) {
            WriteLine($"Testing PUT '{_route}/{SensorApi.Path}/{id}'.");

            // Sample JSON to send
            JObject json = new JObject {
                ["parkingSpace"] = "TEST: c#-api-sensor-update",
                ["latitude"] = 33.810280507079874,
                ["longitude"] = -117.9189795255661
            };

            await _sensorApi.Update(id, json.ToString());
            WriteLine("Sensor Update Success\n");
        }


        private async Task UpdateDriveway(string id) {
            WriteLine($"Testing PUT '{_route}/{DrivewayApi.Path}/{id}'.");

            // Sample JSON to send
            JObject json = new JObject {
                ["name"] = "TEST: c#-api-driveway-update"
            };

            await _drivewayApi.Update(id, json.ToString());
            WriteLine("Driveway Update Success\n");
        }


        private async Task DeleteSensor(string id) {
            WriteLine($"Testing DELETE '{_route}/{SensorApi.Path}/{id}'.");

            // Sample JSON to send
            JObject json = new JObject {
                ["willUnprovision"] = true
            };

            await _sensorApi.Delete(id, json.ToString());
            WriteLine("Sensor Delete Success\n");
        }


        private async Task DeleteDriveway(string id) {
            WriteLine($"Testing DELETE '{_route}/{DrivewayApi.Path}/{id}'.");

            await _drivewayApi.Delete(id);
            WriteLine("Driveway Delete Success\n");
        }


        private async Task DeleteParkingLot(string id) {
            WriteLine($"Testing DELETE '{_route}/{ParkingLotApi.Path}/{id}'.");

            await _parkingLotApi.Delete(id);
            WriteLine("Parking Lot Delete Success\n");
        }
    }
}
