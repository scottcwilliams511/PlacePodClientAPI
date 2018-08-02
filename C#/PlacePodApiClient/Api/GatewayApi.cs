using Http_Async;
using PlacePodApiClient.Helpers;
using PlacePodApiClient.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace PlacePodApiClient.Api {

    /// <summary>
    /// Layer that attempts to abstract the http calls to the API.
    /// Contains all of the API methods related to a gateway
    /// </summary>
    internal static class GatewayMethods {

        private static HttpAsync http = Program.http;


        /// <summary>
        /// Get all gateways.
        /// Route: '/api/gateways'
        /// </summary>
        /// <returns>Array of gateways</returns>
        public static async Task<List<Gateway>> GetGateways() {
            try {
                string gateways = await http.Get("/api/gateways");
                return Factories.CreateCollection<Gateway>(gateways);
            } catch {
                Console.WriteLine("Couldn't get Gateways");
                throw;
            }
        }


        /// <summary>
        /// Insert a new gateway.
        /// Route: '/api/gateway/insert'
        /// </summary>
        /// <param name="json">JSON string</param>
        public static Task<string> InsertGateway(string json) {
            try {
                return http.Post("/api/gateway/insert", json);
            } catch {
                Console.WriteLine("Couldn't Insert Gateway");
                throw;
            }
        }


        /// <summary>
        /// Update an existing gateway.
        /// Route: '/api/gateway/update'
        /// </summary>
        /// <param name="json">JSON string</param>
        public static Task<string> UpdateGateway(string json) {
            try {
               return http.Put("/api/gateway/update", json);
            } catch {
                Console.WriteLine("Couldn't Update Gateway");
                throw;
            }
        }


        /// <summary>
        /// Delete an existing gateway.
        /// Route: '/api/gateway/remove'
        /// </summary>
        /// <param name="json">JSON string</param>
        public static Task<string> RemoveGateway(string json) {
            try {
                return http.Delete("/api/gateway/remove", json);
            } catch {
                Console.WriteLine("Couldn't Remove Gateway");
                throw;
            }
        }
    }
}