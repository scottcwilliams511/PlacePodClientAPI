using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

using PlacePodApiClient.Lib;
using PlacePodApiClient.Models;

namespace PlacePodApiClient.Api {
    /// <summary>
    /// Contains methods routes under the base route of '/parkingLots'.
    /// </summary>
    public class ParkingLotApi: BaseApi {
        public const string Path = "parkinglots";

        public ParkingLotApi(IHttpAsync httpAsync) : base(httpAsync, Path) { }

        /// <summary>
        /// Get all sensor that belong to the parking lot.
        /// </summary>
        /// <param name="id">Id of the parking lot.</param>
        public async Task<ICollection<Sensor>> GetSensors(string id) {
            try {
                string response = await HttpAsync.Get($"/{Path}/{id}/{SensorApi.Path}");

                try {
                    return JsonConvert.DeserializeObject<ICollection<Sensor>>(response);
                } catch (Exception ex) {
                    Console.WriteLine($"Couldn't create sensor objects: " + ex);
                    return new List<Sensor>();
                }
            } catch {
                Console.WriteLine($"Couldn't get sensors.");
                throw;
            }
        }

        /// <summary>
        /// Get all driveways that belong to the parking lot.
        /// </summary>
        /// <param name="id">Id of the parking lot.</param>
        public async Task<ICollection<Driveway>> GetDriveways(string id) {
            try {
                string response = await HttpAsync.Get($"/{Path}/{id}/{DrivewayApi.Path}");

                try {
                    return JsonConvert.DeserializeObject<ICollection<Driveway>>(response);
                } catch (Exception ex) {
                    Console.WriteLine($"Couldn't create driveway objects: " + ex);
                    return new List<Driveway>();
                }
            } catch {
                Console.WriteLine($"Couldn't get driveways.");
                throw;
            }
        }
    }
}
