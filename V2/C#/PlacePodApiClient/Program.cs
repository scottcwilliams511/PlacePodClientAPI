using System;
using System.Threading.Tasks;

using PlacePodApiClient.Applications;
using PlacePodApiClient.Lib;

namespace PlacePodApiClient {

    /// <summary>
    /// Sample PlacePod Client Application. This contains two sub applications to test API features.
    /// More information can be found in files "SampleAppOne.cs" and "SampleAppTwo.cs"
    /// 
    /// Implemented using PlacePod API V2
    /// </summary>
    /// <author>Scott Williams swilliams@pnicorp.com</author>
    public static class Program {

        /** Rest API is documented at https://api.pnicloud.com
         *
         *  To get these values:
         *    1) login to PNI cloud account at https://parking.pnicloud.com
         *    2) click on settings > REST API 
         *    3) Click GENERATE API KEY (if one doesn't exist)
         *    4) Copy the API URL and the API key into the below values.  */
        private const string API_SERVER = "https://api.pnicloud.com";
        private const string API_KEY = "";


        /// <summary>
        /// <para>Api version.</para>
        /// <para>- Leave this as "v2" as long as this client is used for PlacePod API V2.</para>
        /// </summary>
        private const string Version = "v2";


        /// <summary>
        /// Program entry point.
        /// </summary>
        public static async Task Main(string[] args) {
            if (!CheckVariables()) {
                return;
            }

            IHttpAsync http = new HttpAsync($"{API_SERVER}/api/{Version}", API_KEY);

            /* -- App 1 -- */
            Console.WriteLine("This first sample application will test the get, create, update," +
                " and delete functions of \"parkinglots\", \"driveways\" and \"sensors\".");

            Console.WriteLine("Run first sample application (y/n)? ");
            string input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                await new SampleAppOne(http).Run();
            }


            /* -- App 2 -- */
            Console.WriteLine("This second sample application will test the \"sensorlogs\" methods " +
                "as well as the \"sensor downlink\" methods.");

            Console.WriteLine("Run second sample application (y/n)? ");
            input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                await new SampleAppTwo(http).Run();
            }
        }


        /// <summary>
        /// Make sure these values are set. Program can't run without them!
        /// </summary>
        private static bool CheckVariables() {
            if (string.IsNullOrWhiteSpace(API_SERVER)) {
                Console.WriteLine("API_SERVER variable not set!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return false;
            }

            if (string.IsNullOrWhiteSpace(API_KEY)) {
                Console.WriteLine("API_KEY variable not set!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return false;
            }
            return true;
        }
    }
}
