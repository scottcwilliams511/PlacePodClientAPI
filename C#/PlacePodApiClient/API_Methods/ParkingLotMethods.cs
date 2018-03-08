using Http_Async;
using System;
using System.Threading.Tasks;


namespace PlacePodApiClient.API_Methods {

    /// <summary>
    /// Layer that attempts to abstract the http calls to the API.
    /// Contains all of the API methods related to a parking lot
    /// </summary>
    internal class ParkingLotMethods {

        private HttpAsync http;

        /// <summary>
        /// Constructor. Sets the HttpAsync instance from Program
        /// </summary>
        public ParkingLotMethods() {
            http = Program.http;
        }


        /// <summary>
        /// Get Parking Lots.
        /// Route: '/api/parking-lots'
        /// </summary>
        /// <returns>Array of parking lots</returns>
        public Task<string> GetParkingLots() {
            try {
                return http.Get("/api/parking-lots");
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
        public Task<string> InsertParkingLot(string json) {
            try {
                return http.Post("/api/parking-lot/insert", json);
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
        public Task<string> UpdateParkingLot(string json) {
            try {
                return http.Put("/api/parking-lot/update", json);
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
        public Task<string> RemoveParkingLot(string json) {
            try {
                return http.Delete("/api/parking-lot/remove", json);
            } catch {
                Console.WriteLine("Couldn't Remove Parking Lot");
                throw;
            }
        }
    }
}