using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

using PlacePodApiClient.Lib;
using PlacePodApiClient.Models;

namespace PlacePodApiClient.Api {

    public class SensorLogApi : BaseApi {

        public SensorLogApi(IHttpAsync http) : base(http, "/sensorlogs") { }


        public async Task<ICollection<SensorLog>> GetSensorLogs(string start, string end) {
            try {
                string response = await Http.Get($"{Route}/{start}/{end}");

                try {
                    return JsonConvert.DeserializeObject<ICollection<SensorLog>>(response);
                } catch (Exception ex) {
                    Console.WriteLine($"Couldn't create sensor log objects: " + ex);
                    return new List<SensorLog>();
                }
            } catch {
                Console.WriteLine($"Couldn't get sensor logs.");
                throw;
            }
        }
    }
}
