using System;
using Http_Async;


namespace PlacePodApiClient {

    /// <summary>
    /// Sample PlacePod Client Application. This contains two sub applications to test API features.
    /// More information can be found in files "FirstApp.cs" and "SecondApp.cs"
    /// 
    /// Implemented using Placepod API V1.1
    /// Last updated: March 8th, 2018
    /// 
    /// The placepod API is undocumented and subject to change
    /// Fully documented API comming soon!
    /// 
    /// </summary>
    /// <author>Byron Whitlock bwhitlock@pnicorp.com</author>
    /// <author>Scott Williams swilliams@pnicorp.com</author>
    public class Program {

        /** Rest API is documented at https://api.pnicloud.com
         *
         *  To get these values:
         *    1) login to PNI cloud account at https://parking.pnicloud.com
         *    2) click on settings > REST API 
         *    3) Click GENERATE API KEY 
         *    4) Copy the API URL and the API key into the below values.  */
        public static readonly string API_SERVER = "";
        public static readonly string API_KEY = "";

        /// <summary>
        /// Http Client used to make post, get, etc... requests to the PlacePod API.
        /// </summary>
        internal static HttpAsync http;


        /// <summary>
        /// Main function that initializes the two sample applications
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) {

            /** Make sure these values are set. Program can't run without them! */
            if (string.IsNullOrWhiteSpace(API_SERVER)) {
                Console.WriteLine("API_SERVER variable not set!");

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            } else if (string.IsNullOrWhiteSpace(API_KEY)) {
                Console.WriteLine("API_KEY variable not set!");

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }
            http = new HttpAsync(API_SERVER, API_KEY);

            /** Program 1 */
            FirstApp.Run();

            /** Program 2 */
            SecondApp.Run();
        }
    }
}