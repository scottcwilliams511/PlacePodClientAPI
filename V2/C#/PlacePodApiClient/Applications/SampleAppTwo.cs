using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PlacePodApiClient.Api;
using PlacePodApiClient.Lib;
using PlacePodApiClient.Models;

using static System.Console;

namespace PlacePodApiClient.Applications {


    /// <summary>
    /// This application tests the "sensorlogs" methods as well as downlinks.
    /// 
    /// You will be asked to provide the id of a sensor. This sensor should be one that is provisioned
    /// in your system and is sending data up to PNI Cloud.
    /// </summary>
    public class SampleAppTwo {

        private readonly SensorApi Sensors;

        private readonly SensorLogApi SensorLogs;

        private readonly string Route;


        public SampleAppTwo(IHttpAsync http) {
            Sensors = new SensorApi(http);
            SensorLogs = new SensorLogApi(http);
            Route = http.BaseRoute;
        }


        public async Task Run() {

            string defaultStart = "2018-11-16T20:10:00.000Z";
            string defaultEnd = "2018-11-16T20:20:00.000Z";

            WriteLine("Get all sensor logs? ");
            string input = ReadLine();
            if (input == "y" || input == "Y") {
                WriteLine("First a date-time range will be required. Please provide 2 date " +
                    "strings, or type \"default\" to use the strings" +
                    $" \"{defaultStart}\" and \"{defaultEnd}\".");

                WriteLine("Start date-time: ");
                string start = ReadLine();
                string end = "";

                if (start == "default") {
                    start = defaultStart;
                    end = defaultEnd;
                } else {
                    WriteLine("End date-time: ");
                    end = ReadLine();
                }

                await GetSensorLogs(start, end);
            }

            WriteLine("For the rest of the methods you will need a valid Id of an existing sensor in your system.");
            WriteLine("Enter sensor ID: ");
            string sensorId = ReadLine();

            WriteLine($"Running operations using sensor: {sensorId}");

            WriteLine($"Get sensor logs for sensor {sensorId}? ");
            input = ReadLine();
            if (input == "y" || input == "Y") {
                defaultStart = "2018-11-30T22:00:00.000Z";
                defaultEnd = "2018-11-30T22:30:00.000Z";

                WriteLine("Another date-time range will be required. Please provide 2 date " +
                    "strings, or type \"default\" to use the strings" +
                    $" \"{defaultStart}\" and \"{defaultEnd}\".");

                WriteLine("Start date-time: ");
                string start = ReadLine();
                string end = "";

                if (start == "default") {
                    start = defaultStart;
                    end = defaultEnd;
                } else {
                    WriteLine("End date-time: ");
                    end = ReadLine();
                }

                await GetSensorLogs(start, end, sensorId);
            }

            WriteLine($"Downlink tests");

            WriteLine("Recalibrate sensor (y/n)? ");
            input = ReadLine();
            if (input == "y" || input == "Y") {
                await Recalibrate(sensorId);
            }

            // These are advanced features. Contact support for access.
            //await Reboot(sensorId);
            //await Deactivate(sensorId);

            WriteLine("Press any key to continue...");
            ReadKey();
        }


        private async Task GetSensorLogs(string start, string end, string id = null) {
            try {

                ICollection<SensorLog> sensorLogs;
                if (id == null) {
                    WriteLine($"Testing GET \"{Route}{SensorLogs.Route}/{start}/{end}\"");
                    sensorLogs = await SensorLogs.GetSensorLogs(start, end);
                } else {
                    WriteLine($"Testing GET \"{Route}{Sensors.Route}/{id}{SensorLogs.Route}/{start}/{end}\"");
                    sensorLogs = await Sensors.GetSensorLogs(start, end, id);
                }
                
                WriteLine($"Got {sensorLogs.Count} Sensor Logs:");
                foreach (SensorLog sensorLog in sensorLogs) {
                    WriteLine(sensorLog.GetPrintString());
                }
                WriteLine();
            } catch (Exception ex) {
                WriteLine($"Method Error: {ex.Message}\n");
            }
        }


        private async Task Recalibrate(string id) {
            WriteLine($"Testing POST \"{Route}{Sensors.Route}/{id}/recalibrate\"");

            try {
                string response = await Sensors.Recalibrate(id);
                WriteLine($"Recalibrate Sent.\nNetwork response: {response}");
            } catch (Exception ex) {
                WriteLine($"Method Error: {ex.Message}\n");
            }
        }


        private async Task Reboot(string id) {
            WriteLine("Reboot sensor (y/n)? ");
            string input = ReadLine();
            if (input == "y" || input == "Y") {
                WriteLine($"Testing POST \"{Route}{Sensors.Route}/{id}/reboot\"");
                try {
                    string response = await Sensors.Reboot(id);
                    WriteLine($"Reboot Sent.\nNetwork response: {response}");
                } catch (Exception ex) {
                    WriteLine($"Method Error: {ex.Message}\n");
                }
            }
        }


        private async Task Deactivate(string id) {
            WriteLine("Deactivate sensor (y/n)? ");
            string input = ReadLine();
            if (input == "y" || input == "Y") {
                WriteLine($"Testing POST \"{Route}{Sensors.Route}/{id}/deactivate\"");
                try {
                    string response = await Sensors.Deactivate(id);
                    WriteLine($"Deactivate Sent.\nNetwork response: {response}");
                } catch (Exception ex) {
                    WriteLine($"Method Error: {ex.Message}\n");
                }
            }
        }
    }
}
