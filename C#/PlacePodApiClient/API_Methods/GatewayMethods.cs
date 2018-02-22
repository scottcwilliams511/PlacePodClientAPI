using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace PlacePodApiClient.API_Methods {

    /// <summary>
    /// Contains all of the API methods related to a gateway
    /// </summary>
    internal class GatewayMethods {

        private Http http;
        internal GatewayMethods(Http httpClient) {
            http = httpClient;
        }


        /// <summary>
        /// Get all gateways.
        /// Route: '/api/gateways'
        /// </summary>
        /// <returns>Array of gateways</returns>
        public async Task<JArray> GetGateways() {
            try {
                dynamic result = await http.Get("/api/gateways");
                return result;
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
        public async Task<JArray> InsertGateway(string json) {
            try {
                dynamic result = await http.Post("/api/gateway/insert", json);
                return result;
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
        public async Task<JArray> UpdateGateway(string json) {
            try {
               dynamic result = await http.Put("/api/gateway/update", json);
                return result;
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
        public async Task<JArray> RemoveGateway(string json) {
            try {
                dynamic result = await http.Delete("/api/gateway/remove", json);
                return result;
            } catch {
                Console.WriteLine("Couldn't Remove Gateway");
                throw;
            }
        }
    }
}