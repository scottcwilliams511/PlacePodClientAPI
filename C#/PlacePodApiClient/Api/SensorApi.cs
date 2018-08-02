using Http_Async;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlacePodApiClient.Helpers;
using PlacePodApiClient.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace PlacePodApiClient.Api {

    /// <summary>
    /// Layer that attempts to abstract the http calls to the API.
    /// Contains all of the API methods related to a sensor.
    /// </summary>
    internal static class SensorApi {

        /// <summary>
        /// Http Client initialized in Program.
        /// </summary>
        private static HttpAsync http = Program.http;

        /// <summary>
        /// Get Sensors.
        /// Route: '/api/sensors'
        /// </summary>
        /// <param name="filter">Optional</param>
        /// <returns>Array of sensors</returns>
        public static async Task<List<Sensor>> GetSensors(string filter) {
            try {
                string sensors = await http.Post("/api/sensors", filter);
                return Factories.CreateCollection<Sensor>(sensors);
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
        public static Task<string> InsertSensor(string json) {
            try {
                return http.Post("/api/sensor/insert", json);
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
        public static Task<string> UpdateSensor(string json) {
            try {
                return http.Put("/api/sensor/update", json);
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
        public static Task<string> RemoveSensor(string json) {
            try {
                return http.Delete("/api/sensor/remove", json);
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
        public static async Task<List<SensorHistoryLog>> SensorHistory(string filter) {
            try {
                string sensorHistoryLogs = await http.Post("/api/sensor/history", filter);
                return Factories.CreateCollection<SensorHistoryLog>(sensorHistoryLogs);
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
        public static Task<string> Recalibrate(string json) {
            try {
                return http.Post("/api/sensor/recalibrate", json);
            } catch {
                Console.WriteLine("Couldn't send recalibrate request.");
                throw;
            }
        }


        /// <summary>
        /// Initialize a BIST.
        /// Route: '/api/sensor/initialize-bist'
        /// </summary>
        /// <param name="json">JSON string</param>
        public static Task<string> InitializeBist(string json) {
            try {
                return http.Post("/api/sensor/initialize-bist", json);
            } catch {
                Console.WriteLine("Couldn't initialize BIST");
                throw;
            }
        }


        /// <summary>
        /// Retrieve BIST results.
        /// Route: '/api/sensor/bist-response/{SensorId}/{LastUpdated}'
        /// </summary>
        /// <param name="sensorId">Id of the sensor to look for a response from</param>
        /// <param name="now">Only return results after this ISO timestamp</param>
        public static async Task<List<BistResponse>> BistResponse(string sensorId, string now) {
            try {
                string bistResponse = await http.Get("/api/sensor/bist-response/" + sensorId + "/" + now);
                return Factories.CreateCollection<BistResponse>(bistResponse);
            } catch {
                Console.WriteLine("Couldn't Fetch BIST results");
                throw;
            }
        }


        /// <summary>
        /// Sends a Ping initialization request to the sensor.
        /// Route: '/api/sensor/ping'
        /// </summary>
        /// <param name="json">JSON string</param>
        public static Task<string> InitializePing(string json) {
            try {
                return http.Post("/api/sensor/ping", json);
            } catch {
                Console.WriteLine("Couldn't send down Ping request");
                throw;
            }
        }


        /// <summary>
        /// Routes: '/api/sensor/ping' and '/api/sensor/ping-response/{SensorId}/{LastUpdated}'
        /// </summary>
        /// <param name="sensorId">Id of the sensor to look for a response from</param>
        /// <param name="now">Only return results after this ISO timestamp</param>
        /// <returns></returns>
        public static async Task<List<PingResponse>> PingResponse(string sensorId, string now) {
            try {
                string pingResponse = await http.Get("/api/sensor/ping-response/" + sensorId + "/" + now);
                return Factories.CreateCollection<PingResponse>(pingResponse);
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
            public static Task<string> ForceVacant(string json) {
            try {
                return http.Post("/api/sensor/force-vacant", json);
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
        public static Task<string> ForceOccupied(string json) {
            try {
                return http.Post("/api/sensor/force-occupied", json);
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
        public static Task<string> EnableTransitionStateReporting(string json) {
            try {
                return http.Post("/api/sensor/enable-transition-state-reporting", json);
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
        public static Task<string> DisableTransitionStateReporting(string json) {
            try {
                return http.Post("/api/sensor/disable-transition-state-reporting", json);
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
        public static Task<string> SetLoraWakeupInterval(string json) {
            try {
                return http.Post("/api/sensor/set-lora-wakeup-interval", json);
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
        public static Task<string> SetLoraTxPower(string json) {
            try {
                return http.Post("/api/sensor/set-lora-tx-power", json);
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
        public static Task<string> SetTxSpreadingFactor(string json) {
            try {
                return http.Post("/api/sensor/set-tx-spreading-factor", json);
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
        public static Task<string> SetFrequencySubBand(string json) {
            try {
                return http.Post("/api/sensor/set-frequency-sub-band", json);
            } catch {
                Console.WriteLine("Couldn't send set Tx spread factor request");
                throw;
            }
        }
    }
}
