using Http_Async;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace API_Methods {

    /// <summary>
    /// Contains all of the API methods related to a sensor
    /// </summary>
    internal class SensorMethods {

        private HttpAsync http;
        public SensorMethods(string api_url, string api_key) {
            http = new HttpAsync(api_url, api_key);
        }


        /// <summary>
        /// Get Sensors.
        /// Route: '/api/sensors'
        /// </summary>
        /// <param name="filter">Optional</param>
        /// <returns>Array of sensors</returns>
        public async Task<JArray> GetSensors(string filter) {
            try {
                dynamic result = await http.Post("/api/sensors", filter);
                return JsonConvert.DeserializeObject(result);
            } catch {
                Console.WriteLine("Couldn't get Sensors");
                throw;
            }
        }


        /// <summary>
        /// Insert a new sensor.
        /// Route: '/api/sensor/insert'
        /// </summary>
        /// <param name="json">JSON string</param>
        public async Task<JArray> InsertSensor(string json) {
            try {
                dynamic result = await http.Post("/api/sensor/insert", json);
                return JsonConvert.DeserializeObject(result);
            } catch {
                Console.WriteLine("Couldn't Insert Sensor");
                throw;
            }
        }


        /// <summary>
        /// Update an existing sensor.
        /// Route: '/api/sensor/update'
        /// </summary>
        /// <param name="json">JSON string</param>
        public async Task<JArray> UpdateSensor(string json) {
            try {
                dynamic result = await http.Put("/api/sensor/update", json);
                return JsonConvert.DeserializeObject(result);
            } catch {
                Console.WriteLine("Couldn't Update Sensor");
                throw;
            }
        }


        /// <summary>
        /// Delete an existing sensor.
        /// Route: '/api/sensor/remove'
        /// </summary>
        /// <param name="json">JSON string</param>
        public async Task<JArray> RemoveSensor(string json) {
            try {
                dynamic result = await http.Delete("/api/sensor/remove", json);
                return JsonConvert.DeserializeObject(result);
            } catch {
                Console.WriteLine("Couldn't Remove Sensor");
                throw;
            }
        }


        /// <summary>
        /// Get sensor history.
        /// Route: '/api/sensor/history'
        /// </summary>
        /// <param name="filter">JSON string with start/end date and optional fields</param>
        /// <returns>An array of sensor time objects</returns>
        public async Task<JArray> SensorHistory(string filter) {
            try {
                dynamic result = await http.Post("/api/sensor/history", filter);
                return JsonConvert.DeserializeObject(result);
            } catch {
                Console.WriteLine("Couldn't fetch sensor history");
                throw;
            }
        }


        /// <summary>
        /// Send a recalibrate to a sensor
        /// Route: '/api/sensor/recalibrate'
        /// </summary>
        /// <param name="json">JSON string</param>
        public async Task<JArray> Recalibrate(string json) {
            try {
                dynamic result = await http.Post("/api/sensor/recalibrate", json);
                return JsonConvert.DeserializeObject(result);
            } catch {
                Console.WriteLine("Couldn't send recalibrate request.");
                throw;
            }
        }


        /// <summary>
        /// First sends a BIST initialization request to the sensor, then checks
        /// for the sensor response every second over the course of 5 minutes.
        /// Routes: '/api/sensor/initialize-bist' and '/api/sensor/bist-response/{SensorId}/{LastUpdated}'
        /// </summary>
        /// <param name="json">JSON string</param>
        /// <param name="sensorId">sensorId, but not in JSON form.</param>
        public async Task<JArray> Bist(string json, string sensorId) {
            string now = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
            JArray result = null;

            // Initialize the test
            try {
                await http.Post("/api/sensor/initialize-bist", json);
                Console.WriteLine("BIST Sent", now);
            } catch {
                Console.WriteLine("Couldn't initialize BIST");
                throw;
            }

            // We want to call the bist-response every second for 5 minutes
            // or until a response comes back.
            try {
                int timer = 0;
                while (timer < 300) {

                    dynamic rawResponse = await http.Get("/api/sensor/bist-response/" + sensorId + "/" + now);
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
                    return null;
                }

                Console.WriteLine("BIST response recieved!");
                return result;
            } catch {
                Console.WriteLine("Couldn't Fetch BIST results");
                throw;
            }
        }


        /// <summary>
        /// First sends a Ping initialization request to the sensor, then checks
        /// for the sensor response every second over the course of 5 minutes.
        /// Routes: '/api/sensor/ping' and '/api/sensor/ping-response/{SensorId}/{LastUpdated}'
        /// </summary>
        /// <param name="json">JSON string</param>
        /// <param name="sensorId">sensorId, but not in JSON form.</param>
        public async Task<JArray> Ping(string json, string sensorId) {
            string now = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");
            JArray result = null;

            // Send down the Ping request
            try {
                await http.Post("/api/sensor/ping", json);
                Console.WriteLine("Ping Sent", now, result);
            } catch {
                Console.WriteLine("Couldn't send down Ping request");
                throw;
            }

            // We want to call the ping-response every second for 5 minutes
            // or until a response comes back.
            try {
                int timer = 0;
                while (timer < 300) {
                    dynamic rawResponse = await http.Get("/api/sensor/ping-response/" + sensorId + "/" + now);
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
                    return null;
                }

                Console.WriteLine("Ping response recieved!");
                return result;
            } catch {
                Console.WriteLine("Couldn't Fetch Ping results");
                throw;
            }
        }


        /// <summary>
        /// Forces sensor's car presence state to vacant.
        /// Route: '/api/sensor/force-vacant'
        /// </summary>
        /// <param name="json">JSON string</param>
        public async Task<JArray> ForceVacant(string json) {
            try {
                dynamic result = await http.Post("/api/sensor/force-vacant", json);
                return JsonConvert.DeserializeObject(result);
            } catch {
                Console.WriteLine("Couldn't send force vacant request");
                throw;
            }
        }


        /// <summary>
        /// Forces sensor's car presence state to occupied.
        /// Route: '/api/sensor/force-occupied'
        /// </summary>
        /// <param name="json">JSON string</param>
        public async Task<JArray> ForceOccupied(string json) {
            try {
                dynamic result = await http.Post("/api/sensor/force-occupied", json);
                return JsonConvert.DeserializeObject(result);
            } catch {
                Console.WriteLine("Couldn't send force occupied request");
                throw;
            }
        }


        /// <summary>
        /// Turns on state reporting for car entering/leaving.
        /// Route: '/api/sensor/enable-transition-state-reporting'
        /// </summary>
        /// <param name="json">JSON string</param>
        public async Task<JArray> EnableTransitionStateReporting(string json) {
            try {
                dynamic result = await http.Post("/api/sensor/enable-transition-state-reporting", json);
                return JsonConvert.DeserializeObject(result);
            } catch {
                Console.WriteLine("Couldn't send enable transition state reporting request");
                throw;
            }
        }


        /// <summary>
        /// Turns off state reporting for a car entering/leaving.
        /// Route: '/api/sensor/disable-transition-state-reporting'
        /// </summary>
        /// <param name="json">JSON string</param>
        public async Task<JArray> DisableTransitionStateReporting(string json) {
            try {
                dynamic result = await http.Post("/api/sensor/disable-transition-state-reporting", json);
                return JsonConvert.DeserializeObject(result);
            } catch {
                Console.WriteLine("Couldn't send disable transition state reporting request");
                throw;
            }
        }


        /// <summary>
        /// Sets the time period in minutes for how long the sensor will sleep before waking up.
        /// Route: '/api/sensor/set-lora-wakeup-interval'
        /// </summary>
        /// <param name="json">JSON string</param>
        public async Task<JArray> SetLoraWakeupInterval(string json) {
            try {
                dynamic result = await http.Post("/api/sensor/set-lora-wakeup-interval", json);
                return JsonConvert.DeserializeObject(result);
            } catch {
                Console.WriteLine("Couldn't send set wakeup interval request");
                throw;
            }
        }


        /// <summary>
        /// Using an integer between 1 and 30, sets the loRa Tx power.
        /// Route: '/api/sensor/set-lora-tx-power'
        /// </summary>
        /// <param name="json">JSON string</param>
        public async Task<JArray> SetLoraTxPower(string json) {
            try {
                dynamic result = await http.Post("/api/sensor/set-lora-tx-power", json);
                return JsonConvert.DeserializeObject(result);
            } catch {
                Console.WriteLine("Couldn't send set Tx power request");
                throw;
            }
        }


        /// <summary>
        /// Using an integer based on the chart available on the API's Swagger page,
        /// adjusts the sensor's Tx spreading factor.
        /// Route: '/api/sensor/set-tx-spreading-factor'
        /// </summary>
        /// <param name="json">JSON string</param>
        public async Task<JArray> SetTxSpreadingFactor(string json) {
            try {
                dynamic result = await http.Post("/api/sensor/set-tx-spreading-factor", json);
                return JsonConvert.DeserializeObject(result);
            } catch {
                Console.WriteLine("Couldn't send set Tx spread factor request");
                throw;
            }
        }


        /// <summary>
        /// Using an integer based on the chart available on the API's Swagger page,
        /// adjusts the sensor's frequency sub band.
        /// Route: '/api/sensor/set-frequency-sub-band'
        /// </summary>
        /// <param name="json">JSON string</param>
        public async Task<JArray> SetFrequencySubBand(string json) {
            try {
                dynamic result = await http.Post("/api/sensor/set-frequency-sub-band", json);
                return JsonConvert.DeserializeObject(result);
            } catch {
                Console.WriteLine("Couldn't send set Tx spread factor request");
                throw;
            }
        }
    }
}
