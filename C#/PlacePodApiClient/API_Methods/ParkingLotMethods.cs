using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace PlacePodApiClient.API_Methods {

    /// <summary>
    /// Contains all of the API methods related to a parking lot
    /// </summary>
    internal class ParkingLotMethods {

        private Http http;
        public ParkingLotMethods(Http httpClient) {
            http = httpClient;
        }


        /// <summary>
        /// Get Parking Lots.
        /// Route: '/api/parking-lots'
        /// </summary>
        /// <returns>Array of parking lots</returns>
        public async Task<JArray> GetParkingLots() {
            try {
                dynamic result = await http.Get("/api/parking-lots");
                return result;
            } catch {
                Console.WriteLine("Couldn't get Parking Lots");
                throw;
            }
        }


        /// <summary>
        /// Insert a new parking lot.
        /// Route: '/api/parking-lot/insert'
        /// </summary>
        /// <param name="json">JSON string</param>
        public async Task<JArray> InsertParkingLot(string json) {
            try {
                dynamic result = await http.Post("/api/parking-lot/insert", json);
                return result;
            } catch {
                Console.WriteLine("Couldn't Insert Parking Lot");
                throw;
            }
            
        }


        /// <summary>
        /// Updating an existing parking lot.
        /// Route: '/api/parking-lot/update'
        /// </summary>
        /// <param name="json">JSON string</param>
        public async Task<JArray> UpdateParkingLot(string json) {
            try {
                dynamic result = await http.Put("/api/parking-lot/update", json);
                return result;
            } catch {
                Console.WriteLine("Couldn't Update Parking Lot");
                throw;
            }
        }


        /// <summary>
        /// Delete an existing parking lot.
        /// Route: '/api/parking-lot-remove'
        /// </summary>
        /// <param name="json">JSON string</param>
        public async Task<JArray> RemoveParkingLot(string json) {
            try {
                dynamic result = await http.Delete("/api/parking-lot/remove", json);
                return result;
            } catch {
                Console.WriteLine("Couldn't Remove Parking Lot");
                throw;
            }
        }
    }
}