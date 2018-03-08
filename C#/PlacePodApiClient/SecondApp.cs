using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlacePodApiClient.API_Methods;
using System;
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
    ///   2) Send a recalibrate
    ///   3) Run a full BIST test (give up to 5 minutes for a response)
    ///   4) Send a ping and wait for response (up to 5 minutes)
    ///   5) Send a force vacant
    ///   6) Send a force occupied
    ///   7) Send an enable transition state reporting
    ///   8) Send a disable transition state reporting
    ///   9) Send a set lora wakeup interval to 5 minutes
    ///  10) Send a set loRa Tx power call to 11dB.
    ///  11) Send a set Tx spreading factor call to SF 7, BW 125 kHz
    ///  12) Send a set frequency sub band call to 902.3 kHz - 903.7 kHz - 125k
    /// Please note that steps 10, 11, 12 are potentially dangerous if set incorrectly and
    /// may cause the placePod to go offline permanently. Please read available documentation
    /// on the API swagger page first!
    /// 
    /// NOTE: There are only so many requests that can be queued by a sensor at a time, so it
    /// is suggested to not make rapid calls to the sensor and wait around a minute before 
    /// sending another call to the same sensor.
    /// </summary>
    class SecondApp {

        /// <summary>
        /// ISO date string of a time when your sensor was on
        /// </summary>
        private static readonly string startTime = "2017-12-08T01:00:00.000Z";


        /// <summary>
        /// ISO date string of a later time when your sensor was on
        /// </summary>
        private static readonly string endTime = "2017-12-08T01:10:00.000Z";


        /// <summary>
        /// Constructor. Call .Run next to run the application.
        /// </summary>
        public SecondApp() {
            Console.WriteLine("This second sample application will test the other 'sensor' operaions. " +
                "A sensor ID must be provided to proceed.");
        }


        /// <summary>
        /// Start the second application
        /// </summary>
        /// <returns></returns>
        public void Run() {
            Console.WriteLine("Run second sample application (y/n)? ");
            string input = Console.ReadLine();
            if (input == "y" || input == "Y") {

                Console.WriteLine("Enter sensor ID: ");
                sensorId = Console.ReadLine();

                Task.Run(async () => {
                    Console.WriteLine("Running operations using sensor: " + sensorId);

                    await GetSensorHistoryCount();
                    await SendRecalibrate();
                    await FullBist();
                    await FullPingTest();
                    await ForceVacant();
                    await ForceOccupied();
                    await EnableTransitionStateReporting();
                    await DisableTransitionStateReporting();
                    await SetWakeupInterval();
                    await SetTxPower();
                    await SetSpreadFactor();
                    await SetFrequencySubBand();

                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }).GetAwaiter().GetResult();

            }
        }


        /// <summary>
        /// Contains the API methods for a sensor
        /// </summary>
        private static SensorMethods sensorMethods = new SensorMethods();

        private static string sensorId;

        /// <summary>
        /// Fetch sensor history within the timespan and report the number of results.
        /// Check private variables 'startTime' and 'endTime' for the timespan
        /// and adjust these as needed!
        /// </summary>
        private async static Task GetSensorHistoryCount() {
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
                    dynamic result = await sensorMethods.SensorHistory(json.ToString());
                    JArray history = JsonConvert.DeserializeObject(result);
                    Console.WriteLine("Number of results: " + history.Count + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }
        }


        /// <summary>
        /// Send down a recalibrate request
        /// </summary>
        /// <returns></returns>
        private async static Task SendRecalibrate() {
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

                    await sensorMethods.Recalibrate(json.ToString());

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
        private async Task FullBist() {
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

                    string now = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
                    JArray result = null;

                    // Initialize the test
                    await sensorMethods.InitializeBist(json.ToString());

                    Console.WriteLine("BIST Sent", now);

                    // We want to call the bist-response every second for 5 minutes
                    // or until a response comes back.
                    int timer = 0;
                    while (timer < 300) {
                        dynamic rawResponse = await sensorMethods.BistResponse(sensorId, now);

                        result = JsonConvert.DeserializeObject(rawResponse);
                        if (result.ToString() != "[]") {
                            break;
                        }

                        Console.WriteLine("Waiting for Bist response " + (++timer));
                        await Task.Delay(1000);
                        now = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
                    }

                    if (timer >= 300) {
                        Console.WriteLine("No response...");
                    } else {
                        Console.WriteLine("BIST response recieved!");

                        foreach (JToken i in result) {
                            Console.WriteLine("--> " + i["sensorType"] + ": " + i["status"]);
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
        private static async Task FullPingTest() {
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

                    string now = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
                    JArray result = null;

                    // Send down the Ping request
                    await sensorMethods.InitializePing(json.ToString());

                    Console.WriteLine("Ping Sent", now, result);

                    // We want to call the ping-response every second for 5 minutes
                    // or until a response comes back.
                    int timer = 0;
                    while (timer < 300) {
                        dynamic rawResponse = await sensorMethods.PingResponse(sensorId, now);

                        result = JsonConvert.DeserializeObject(rawResponse);
                        if (result.ToString() != "[]") {
                            break;
                        }

                        Console.WriteLine("Waiting for Ping response " + (++timer));
                        await Task.Delay(1000);
                        now = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
                    }

                    if (timer >= 300) {
                        Console.WriteLine("No response...");
                    } else {
                        Console.WriteLine("Ping response recieved!");

                        foreach (JToken i in result) {
                            Console.WriteLine("--> Ping RSSI: " + i["pingRssi"] + ", Ping SNR: " 
                                + i["pingSNR"] + ". Server time: " + i["serverTime"]);
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
        private static async Task ForceVacant() {
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

                    await sensorMethods.ForceVacant(json.ToString());

                    Console.WriteLine("Force Vacant Sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }
        }


        /// <summary>
        /// Force the sensor's Car Presence to say occcupied
        /// </summary>
        private static async Task ForceOccupied() {
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

                    await sensorMethods.ForceOccupied(json.ToString());

                    Console.WriteLine("Force Occupied Sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }
        }


        /// <summary>
        /// Enables transition state reporting for the sensor
        /// </summary>
        private static async Task EnableTransitionStateReporting() {
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

                    await sensorMethods.EnableTransitionStateReporting(json.ToString());

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
        private static async Task DisableTransitionStateReporting() {
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

                    await sensorMethods.DisableTransitionStateReporting(json.ToString());

                    Console.WriteLine("Disable transition state reporting Sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }
        }


        /// <summary>
        /// Sets the sensor's wakeup interval to every 5 minutes.
        /// </summary>
        private static async Task SetWakeupInterval() {
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

                    await sensorMethods.SetLoraWakeupInterval(json.ToString());

                    Console.WriteLine("Set LoRa wakeup interval Sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }
        }


        /// <summary>
        /// This will set the sensor's LoRa Tx power to ll dBs.
        /// </summary>
        private static async Task SetTxPower() {
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

                    await sensorMethods.SetLoraTxPower(json.ToString());

                    Console.WriteLine("Set LoRa Tx Power sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }
        }


        /// <summary>
        /// Change the sensor's spread factor to 7, BW 125 kHz
        /// </summary>
        private static async Task SetSpreadFactor() {
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

                    await sensorMethods.SetTxSpreadingFactor(json.ToString());

                    Console.WriteLine("Set Tx spreading factor sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }
        }


        /// <summary>
        /// Set sensor's frequency sub band to 902.3 kHz - 903.7 kHz - 125k"
        /// </summary>
        private static async Task SetFrequencySubBand() {
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

                    await sensorMethods.SetFrequencySubBand(json.ToString());

                    Console.WriteLine("Set frequency sub band sent" + "\n");
                } catch (Exception ex) {
                    Console.WriteLine("Method Error: " + ex.Message + "\n");
                }
            }
        }
    }
}