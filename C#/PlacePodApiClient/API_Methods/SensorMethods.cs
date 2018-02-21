using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlacePodApiClient.API_Methods {

    /// <summary>
    /// Contains all of the API methods related to a sensor
    /// </summary>
    internal class SensorMethods {

        private Http http;
        public SensorMethods(Http httpClient) {
            http = httpClient;
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
                return result;
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
                return result;
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
                return result;
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
                return result;
            } catch {
                Console.WriteLine("Couldn't Remove Sensor");
                throw;
            }
        }
    }
}
