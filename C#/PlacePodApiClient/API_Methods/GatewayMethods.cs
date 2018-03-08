using Http_Async;
using System;
using System.Threading.Tasks;


namespace PlacePodApiClient.API_Methods {

    /// <summary>
    /// Layer that attempts to abstract the http calls to the API.
    /// Contains all of the API methods related to a gateway
    /// </summary>
    internal class GatewayMethods {

        private HttpAsync http;

        /// <summary>
        /// Constructor. Sets the HttpAsync instance from Program
        /// </summary>
        public GatewayMethods() {
            http = Program.http;
        }


        /// <summary>
        /// Get all gateways.
        /// Route: '/api/gateways'
        /// </summary>
        /// <returns>Array of gateways</returns>
        public Task<string> GetGateways() {
            try {
                return http.Get("/api/gateways");
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
        public Task<string> InsertGateway(string json) {
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
        public Task<string> UpdateGateway(string json) {
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
        public Task<string> RemoveGateway(string json) {
            try {
                return http.Delete("/api/gateway/remove", json);
            } catch {
                Console.WriteLine("Couldn't Remove Gateway");
                throw;
            }
        }
    }
}