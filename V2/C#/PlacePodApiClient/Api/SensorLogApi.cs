using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

using PlacePodApiClient.Lib;
using PlacePodApiClient.Models;

namespace PlacePodApiClient.Api {
    /// <summary>
    /// Contains methods routes under the base route of '/sensorlogs'.
    /// </summary>
    public class SensorLogApi : BaseApi {
        public const string Path = "sensorlogs";

        public SensorLogApi(IHttpAsync httpAsync) : base(httpAsync, Path) { }

        /// <summary>
        /// Get all sensor logs withing the time interval.
        /// </summary>
        /// <param name="start">Starting date-time.</param>
        /// <param name="end">Ending date-time.</param>
        public async Task<ICollection<SensorLog>> GetSensorLogs(string start, string end) {
            try {
                string response = await HttpAsync.Get($"/{Path}/{start}/{end}");

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
