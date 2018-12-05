using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

using PlacePodApiClient.Lib;
using PlacePodApiClient.Models;

namespace PlacePodApiClient.Api {

    public class ParkingLotApi: BaseApi {

        public ParkingLotApi(IHttpAsync http) : base(http, "/parkinglots") { }


        public async Task<ICollection<Sensor>> GetSensors(string id) {
            try {
                string response = await Http.Get($"{Route}/{id}/sensors");

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


        public async Task<ICollection<Driveway>> GetDriveways(string id) {
            try {
                string response = await Http.Get($"{Route}/{id}/driveways");

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
