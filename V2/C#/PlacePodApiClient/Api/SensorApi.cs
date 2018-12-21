using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlacePodApiClient.Lib;
using PlacePodApiClient.Models;

namespace PlacePodApiClient.Api {

    public class SensorApi : BaseApi {

        public SensorApi(IHttpAsync http) : base(http, "/sensors") { }


        public Task<string> Recalibrate(string id) {
            try {
                return Http.Post($"{Route}/{id}/recalibrate", null);
            } catch {
                Console.WriteLine("Couldn't send recalibrate request.");
                throw;
            }
        }


        public Task<string> Reboot(string id) {
            try {
                return Http.Post($"{Route}/{id}/reboot", null);
            } catch {
                Console.WriteLine("Couldn't send reboot request.");
                throw;
            }
        }


        public Task<string> Deactivate(string id) {
            try {
                return Http.Post($"{Route}/{id}/deactivate", null);
            } catch {
                Console.WriteLine("Couldn't send deactivate request.");
                throw;
            }
        }


        public async Task<ICollection<SensorLog>> GetSensorLogs(string start, string end, string id) {
            try {
                string response = await Http.Get($"{Route}/{id}/sensorlogs/{start}/{end}");

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
