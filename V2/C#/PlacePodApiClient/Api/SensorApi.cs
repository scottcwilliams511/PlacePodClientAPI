using System;
using System.Threading.Tasks;

using PlacePodApiClient.Lib;

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
    }
}
