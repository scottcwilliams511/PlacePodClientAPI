using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System;

/// <summary>
/// The placepod API is undocumented and subject to change
/// Fully documented API comming soon!
/// </summary>
/// <author>Byron Whitlock bwhitlock@pnicorp.com</author>
namespace PlacePodApiExample
{
    class Program
    {

        static void Main(string[] args)
        {

            //
            // The placepod API is undocumented and subject to change
            // Fully documented API comming soon!
            // 


            string user = "placepod@pnicorp.com";
            string pass = "placepod";
            string sensorId = "008000000400065c";
            string server = "https://parking.pnicloud.com/"; //always always always use https with this undocumented api


            WebRequest req = WebRequest.Create(server + "/api/login/");

            var data = string.Format("email={0}&password={1}", System.Net.WebUtility.HtmlEncode(user), System.Net.WebUtility.HtmlEncode(pass));

            UTF8Encoding enc = new UTF8Encoding();

            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            var dataStream = req.GetRequestStreamAsync().Result; // You can call .Result on a Task to wait for the result. or if it returns null use wait()
            dataStream.Write(enc.GetBytes(data), 0, data.Length);

            //Get the response
            WebResponse res = req.GetResponseAsync().Result;
            Stream receiveStream = res.GetResponseStream();
            StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
            string content = reader.ReadToEnd();

            dynamic auth = JsonConvert.DeserializeObject(content);


            ///You'll need to save the userId and token on the client, for subsequent authenticated requests.
            var authToken = auth.data.authToken;
            var userId = auth.data.userId;

            /* Logging Out
             * You also have an authenticated POST /api/logout endpoint for logging a user out.
             * If successful, the auth token that is passed in the request header will be 
             * invalidated (removed from the user account), so it will not work in any subsequent
             * requests.
             */


            /* Authenticated Calls
             * For any endpoints that require the default authentication, you must include the userId and authToken with each request under the following headers:
             *   X-User-Id
             *   X-Auth-Token
             */
            req = WebRequest.Create(server + "/api/sensor/status/" + sensorId);
            req.Headers["X-Auth-Token"] = authToken;
            req.Headers["X-User-Id"] = userId;

            //Get the sensor data response
            res = req.GetResponseAsync().Result; 
            receiveStream = res.GetResponseStream();
            reader = new StreamReader(receiveStream, Encoding.UTF8);
            content = reader.ReadToEnd();

            dynamic sensor = JsonConvert.DeserializeObject(content);
                       

            /*
             sensor.data = {
                _id:YoQHeicC7yuoRdKwr
                sensorId:008000000400065c
                parkingSpace:Test
                parkingId:xrsRsdGrhAdhXDRGk
                location:
                installationDate:17
                status:10
                lat:40.839690695485785
                lon:-122.75498628649076
                createdAt:2017-04-17 16:26:31.373
                lastSeen:2017-03-24T20:13:23.101Z
                hostFirmware:0.3.8
                sensorFirmware:1.2.14.0.59
                parkingName:Santa Rosa Downtown
                validationInProcess:false
            }
            */
            //All your api's are belong to us. 
            Console.WriteLine("_id:" + sensor._id);
            Console.WriteLine("sensorId:" + sensor.sensorId);
            Console.WriteLine("parkingSpace:" + sensor.parkingSpace);
            Console.WriteLine("parkingId:" + sensor.parkingId);
            Console.WriteLine("location:" + sensor.location);
            Console.WriteLine("installationDate:" + sensor.installationDate);
            Console.WriteLine("status:" + SensorStatus((int)sensor.status));
            Console.WriteLine("lat:" + sensor.lat);
            Console.WriteLine("lon:" + sensor.lon);
            Console.WriteLine("createdAt:" + sensor.createdAt);
            Console.WriteLine("hostFirmware:" + sensor.hostFirmware);
            Console.WriteLine("sensorFirmware:" + sensor.sensorFirmware);
            Console.WriteLine("parkingName:" + sensor.parkingName);
            Console.WriteLine("validationInProcess:" + sensor.validationInProcess);
            Console.WriteLine("");
            Console.WriteLine("Press Any Key to Continue...");
            Console.ReadKey();
        }


        /// <summary>
        /// Maps sensor status to human readable names
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string SensorStatus(int code)
        {
            var actualStatus = "Unknown (" + code + ")";

            if (code == 1)
                actualStatus = "Occupied";

            if (code == 2)
                actualStatus = "Vacant";

            if (code == 10)
                actualStatus = "Recalibrating";

            if (code == 20)
                actualStatus = "Car Entering";

            if (code == 40)
                actualStatus = "Car Leaving";

            return actualStatus;
        }
    }
}
