using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

using PlacePodApiClient.Lib;
using PlacePodApiClient.Models;

namespace PlacePodApiClient.Api {
    /// <summary>
    /// Contains methods routes under the base route of '/sensors'.
    /// </summary>
    public class SensorApi : BaseApi {
        public const string Path = "sensors";

        public SensorApi(IHttpAsync httpAsync) : base(httpAsync, Path) { }

        /// <summary>
        /// Send a recalibrate downlink request.
        /// </summary>
        /// <param name="id">Id of the sensor.</param>
        public Task<string> Recalibrate(string id) {
            try {
                return HttpAsync.Post($"/{Path}/{id}/recalibrate");
            } catch {
                Console.WriteLine("Couldn't send recalibrate request.");
                throw;
            }
        }

        /// <summary>
        /// Send a reboot downlink request.
        /// </summary>
        /// <param name="id">Id of the sensor.</param>
        public Task<string> Reboot(string id) {
            try {
                return HttpAsync.Post($"/{Path}/{id}/reboot");
            } catch {
                Console.WriteLine("Couldn't send reboot request.");
                throw;
            }
        }

        /// <summary>
        /// Send a deactivate downlink request.
        /// </summary>
        /// <param name="id">Id of the sensor.</param>
        public Task<string> Deactivate(string id) {
            try {
                return HttpAsync.Post($"/{Path}/{id}/deactivate");
            } catch {
                Console.WriteLine("Couldn't send deactivate request.");
                throw;
            }
        }

        /// <summary>
        /// Get logs for the given sensor withing the time interval.
        /// </summary>
        /// <param name="start">Starting date-time.</param>
        /// <param name="end">Ending date-time.</param>
        /// <param name="id">Id of the sensor.</param>
        public async Task<ICollection<SensorLog>> GetSensorLogs(string start, string end, string id) {
            try {
                string response = await HttpAsync.Get($"/{Path}/{id}/{SensorLogApi.Path}/{start}/{end}");

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
