using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlacePodApiClient.Api;
using PlacePodApiClient.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace PlacePodApiClient {

    /// <summary>
    /// This application tests non-CRUD sensor API functions and how to
    /// properly call them. Note that while these can be used on a 'test' sensor that
    /// doesn't actually exist, you will want to use this on a real installed sensor.
    /// You can see the results of this test on the Parking Cloud by going to the sensor's
    /// diagnostic information.
    /// The tasks that can be optionally performed are:
    ///   1) Get sensor history at a specific 10 minute interval
    ///   2) Send a recalibrate request
    ///   3) Run a full BIST test (give up to 5 minutes for a response)
    ///   4) Send a ping and wait for response (up to 5 minutes)
    ///   5) Send a force vacant request
    ///   6) Send a force occupied request
    ///   7) Send an enable transition state reporting request
    ///   8) Send a disable transition state reporting request
    ///   9) Send a set lora wakeup interval to 5 minutes request
    ///  10) Send a set loRa Tx power call to 11dB request
    ///  11) Send a set Tx spreading factor call to SF 7, BW 125 kHz request
    ///  12) Send a set frequency sub band call to 902.3 kHz - 903.7 kHz - 125k request
    /// Please note that steps 10, 11, 12 are potentially dangerous if set incorrectly and
    /// may cause the placePod to go offline permanently. Please read available documentation
    /// on the API swagger page first!
    /// 
    /// NOTE: There are only so many requests that can be queued by a sensor at a time, so it
    /// is suggested to not make rapid calls to the sensor and wait around a minute before 
    /// sending another call to the same sensor.
    /// </summary>
    internal static class SecondApp {

        /// <summary>
        /// ISO date string of a time when your sensor was on
        /// </summary>
        private static readonly string startTime = "2017-12-08T01:00:00.000Z";


        /// <summary>
        /// ISO date string of a later time when your sensor was on
        /// </summary>
        private static readonly string endTime = "2017-12-08T01:10:00.000Z";


        /// <summary>
        /// Start the second application
        /// </summary>
        /// <returns></returns>
        public static void Run() {
            Console.WriteLine("This second sample application will test the other 'sensor' operaions. " +
                "A sensor ID must be provided to proceed.");

            Console.WriteLine("Run second sample application (y/n)? ");
            string input = Console.ReadLine();
            if (input != "y" && input != "Y") {
                return;
            }

            Console.WriteLine("Enter sensor ID: ");
            string sensorId = Console.ReadLine();

            Task.Run(async () => {
                Console.WriteLine("Running operations using sensor: " + sensorId);

                await GetSensorHistoryCount(sensorId);
                await SendRecalibrate(sensorId);
                await FullBist(sensorId);
                await FullPingTest(sensorId);
                await ForceVacant(sensorId);
                await ForceOccupied(sensorId);
                await EnableTransitionStateReporting(sensorId);
                await DisableTransitionStateReporting(sensorId);
                await SetWakeupInterval(sensorId);
                await SetTxPower(sensorId);
                await SetSpreadFactor(sensorId);
                await SetFrequencySubBand(sensorId);

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }).GetAwaiter().GetResult();
        } 


        /// <summary>
        /// Fetch sensor history within the timespan and report the number of results.
        /// Check private variables 'startTime' and 'endTime' for the timespan
        /// and adjust these as needed!
        /// </summary>
        private static async Task GetSensorHistoryCount(string sensorId) {
            Console.WriteLine("Test /api/sensor/history");

            Console.WriteLine("Get Sensor History (y/n)? ");
            string input = Console.ReadLine();
            if (input == "y" || input == "Y") {

                // Sample JSON to send
                JObject json = new JObject {
                    ["sensorId"] = sensorId,
                    ["startTime"] = startTime,
                    ["endTime"] = endTime
                };

                try {
                    List<SensorHistoryLog> logs = await SensorApi.SensorHistory(json.ToString());
                    Console.WriteLine("Number of results: " + logs.Count + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }
        }


        /// <summary>
        /// Send down a recalibrate request
        /// </summary>
        /// <returns></returns>
        private static async Task SendRecalibrate(string sensorId) {
            Console.WriteLine("Test /api/sensor/recalibrate");

            Console.WriteLine("Recalibrate sensor (y/n)? ");
            string input = Console.ReadLine();
            if (input == "y" || input == "Y") {

                // Sample JSON to send
                JObject json = new JObject {
                    ["sensorId"] = sensorId
                };

                try {
                    Console.WriteLine("Sending Recalibrate request...");

                    await SensorApi.Recalibrate(json.ToString());

                    Console.WriteLine("Recalibrate Sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }
        }

        /// <summary>
        /// First sends a BIST initialization request to the sensor, then checks
        /// for the sensor response every second over the course of 5 minutes.
        /// </summary>
        /// <returns></returns>
        private static async Task FullBist(string sensorId) {
            Console.WriteLine("Test /api/initialize-bist and /api/sensor/bist-response/{SensorId}/{LastUpdated}");
            Console.WriteLine("Run basic internal self test (BIST) (y/n)?");
            string input = Console.ReadLine();
            if (input == "y" || input == "Y") {

                // Sample JSON to send
                JObject json = new JObject {
                    ["sensorId"] = sensorId
                };

                try {
                    Console.WriteLine("Sending BIST request...");

                    string now = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss");

                    // Initialize the test
                    await SensorApi.InitializeBist(json.ToString());

                    Console.WriteLine("BIST Sent at UTC: " + now);

                    List<BistResponse> responses = new List<BistResponse>();

                    // We want to call the bist-response every second for 5 minutes
                    // or until a response comes back.
                    int timer = 0;
                    while (timer < 300) {
                        responses = await SensorApi.BistResponse(sensorId, now);
                        if (responses.Count > 0) {
                            break;
                        }

                        Console.WriteLine("Waiting for Bist response " + (++timer));
                        await Task.Delay(1000);
                    }

                    if (timer >= 300) {
                        Console.WriteLine("No response...");
                    } else {
                        Console.WriteLine("BIST response recieved!");

                        foreach (BistResponse response in responses) {
                            Console.WriteLine("--> " + response.SensorType + ": " + response.Status);
                        }
                    }

                    Console.WriteLine();
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }
        }


        /// <summary>
        /// Send down a Ping request and wait up to 5 minutes for the response
        /// </summary>
        /// <returns></returns>
        private static async Task FullPingTest(string sensorId) {
            Console.WriteLine("Test /api/sensor/ping and /api/sensor/ping-response/{SensorId}/{LastUpdated}");
            Console.WriteLine("Ping sensor (y/n)?");
            string input = Console.ReadLine();
            if (input == "y" || input == "Y") {

                // Sample JSON to send
                JObject json = new JObject {
                    ["sensorId"] = sensorId
                };

                try {
                    Console.WriteLine("Sending Ping request...");

                    string now = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss");

                    // Send down the Ping request
                    await SensorApi.InitializePing(json.ToString());

                    Console.WriteLine("Ping Sent at UTC: " + now);

                    List<PingResponse> responses = new List<PingResponse>();

                    // We want to call the ping-response every second for 5 minutes
                    // or until a response comes back.
                    int timer = 0;
                    while (timer < 300) {
                        responses = await SensorApi.PingResponse(sensorId, now);
                        if (responses.Count > 0) {
                            break;
                        }

                        Console.WriteLine("Waiting for Ping response " + (++timer));
                        await Task.Delay(1000);
                    }

                    if (timer >= 300) {
                        Console.WriteLine("No response...");
                    } else {
                        Console.WriteLine("Ping response recieved!");

                        foreach (PingResponse response in responses) {
                            Console.WriteLine("--> Ping RSSI: " + response.PingRssi + ", Ping SNR: " 
                                + response.PingSNR + ". Server time: " + response.ServerTime);
                        }
                    }

                    Console.WriteLine();
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }
        }


        /// <summary>
        /// Force the sensor's Car Presence to say vacant
        /// </summary>
        private static async Task ForceVacant(string sensorId) {
            Console.WriteLine("Test /api/sensor/force-vacant");
            Console.WriteLine("Force car presence to vacant (y/n)?");
            string input = Console.ReadLine();
            if (input == "y" || input == "Y") {

                // Sample JSON to send
                JObject json = new JObject {
                    ["sensorId"] = sensorId
                };

                try {
                    Console.WriteLine("Sending Force Vacant...");

                    await SensorApi.ForceVacant(json.ToString());

                    Console.WriteLine("Force Vacant Sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }
        }


        /// <summary>
        /// Force the sensor's Car Presence to say occcupied
        /// </summary>
        private static async Task ForceOccupied(string sensorId) {
            Console.WriteLine("Test /api/sensor/force-occupied");
            Console.WriteLine("Force car presence to occupied (y/n)?");
            string input = Console.ReadLine();
            if (input == "y" || input == "Y") {

                // Sample JSON to send
                JObject json = new JObject {
                    ["sensorId"] = sensorId
                };

                try {
                    Console.WriteLine("Sending Force Occupied...");

                    await SensorApi.ForceOccupied(json.ToString());

                    Console.WriteLine("Force Occupied Sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }
        }


        /// <summary>
        /// Enables transition state reporting for the sensor
        /// </summary>
        private static async Task EnableTransitionStateReporting(string sensorId) {
            Console.WriteLine("Test /api/sensor/enable-transition-state-reporting");
            Console.WriteLine("Enable transition state reporting (y/n)?");
            string input = Console.ReadLine();
            if (input == "y" || input == "Y") {

                // Sample JSON to send
                JObject json = new JObject {
                    ["sensorId"] = sensorId
                };

                try {
                    Console.WriteLine("Sending enable transition state reporting...");

                    await SensorApi.EnableTransitionStateReporting(json.ToString());

                    Console.WriteLine("Enable transition state reporting Sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }
        }


        /// <summary>
        /// Disables transition state reporting for the sensor
        /// </summary>
        /// <returns></returns>
        private static async Task DisableTransitionStateReporting(string sensorId) {
            Console.WriteLine("Test /api/sensor/disable-transition-state-reporting");
            Console.WriteLine("Disable transition state reporting (y/n)?");
            string input = Console.ReadLine();
            if (input == "y" || input == "Y") {

                // Sample JSON to send
                JObject json = new JObject {
                    ["sensorId"] = sensorId
                };

                try {
                    Console.WriteLine("Sending disable transition state reporting...");

                    await SensorApi.DisableTransitionStateReporting(json.ToString());

                    Console.WriteLine("Disable transition state reporting Sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }
        }


        /// <summary>
        /// Sets the sensor's wakeup interval to every 5 minutes.
        /// </summary>
        private static async Task SetWakeupInterval(string sensorId) {
            Console.WriteLine("Test /api/sensor/set-lora-wakeup-interval");
            Console.WriteLine("Set LoRa wakeup interval to 5 minutes (y/n)?");
            string input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                try {

                    // Sample JSON to send. Payload is in minutes (integer)
                    JObject json = new JObject {
                        ["sensorId"] = sensorId,
                        ["payload"] =  5
                    };

                    Console.WriteLine("Sending set LoRa wakeup interval...");

                    await SensorApi.SetLoraWakeupInterval(json.ToString());

                    Console.WriteLine("Set LoRa wakeup interval Sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }
        }


        /// <summary>
        /// This will set the sensor's LoRa Tx power to ll dBs.
        /// </summary>
        private static async Task SetTxPower(string sensorId) {
            Console.WriteLine("Test /api/sensor/set-lora-tx-power");
            Console.WriteLine("Set LoRa Tx Power to 11 (y/n)?");
            string input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                try {

                    // Sample JSON to send. Payload is between 1 and 30 inclusinve (integer)
                    JObject json = new JObject {
                        ["sensorId"] = sensorId,
                        ["payload"] = 11
                    };

                    Console.WriteLine("Sending set LoRa Tx Power...");

                    await SensorApi.SetLoraTxPower(json.ToString());

                    Console.WriteLine("Set LoRa Tx Power sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }
        }


        /// <summary>
        /// Change the sensor's spread factor to 7, BW 125 kHz
        /// </summary>
        private static async Task SetSpreadFactor(string sensorId) {
            Console.WriteLine("Test /api/sensor/set-tx-spreading-factor");
            Console.WriteLine("Set Tx Spread Factor to SF 7, BW 125 kHz (y/n)?");
            string input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                try {

                    // Sample JSON to send. Payload is an int (see API documentation)
                    JObject json = new JObject {
                        ["sensorId"] = sensorId,
                        ["payload"] = 6
                    };

                    Console.WriteLine("Sending set Tx spreading factor...");

                    await SensorApi.SetTxSpreadingFactor(json.ToString());

                    Console.WriteLine("Set Tx spreading factor sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }
        }


        /// <summary>
        /// Set sensor's frequency sub band to 902.3 kHz - 903.7 kHz - 125k"
        /// </summary>
        private static async Task SetFrequencySubBand(string sensorId) {
            Console.WriteLine("Test /api/sensor/set-frequency-sub-band");
            Console.WriteLine("Set Frequency sub band to 902.3 kHz - 903.7 kHz - 125k");
            string input = Console.ReadLine();
            if (input == "y" || input == "Y") {
                try {

                    // Sample JSON to send. Payload is an int (see API documentation)
                    JObject json = new JObject {
                        ["sensorId"] = sensorId,
                        ["payload"] = 1
                    };

                    Console.WriteLine("Sending set requency Sub Band...");

                    await SensorApi.SetFrequencySubBand(json.ToString());

                    Console.WriteLine("Set frequency sub band sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }
        }
    }
}