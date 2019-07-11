using Http_Async;
using PlacePodApiClient.Helpers;
using PlacePodApiClient.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace PlacePodApiClient.Api {

    /// <summary>
    /// Layer that attempts to abstract the http calls to the API.
    /// Contains all of the API methods related to a parking lot
    /// </summary>
    internal static class ParkingLotApi {

        private static HttpAsync http = Program.http;

        /// <summary>
        /// Get Parking Lots.
        /// Route: '/api/parking-lots'
        /// </summary>
        /// <returns>Array of parking lots</returns>
        public static async Task<List<ParkingLot>> GetParkingLots() {
            try {
                string parkingLots = await http.Get("/api/parking-lots");
                return Factories.CreateCollection<ParkingLot>(parkingLots);
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
        public static Task<string> InsertParkingLot(string json) {
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
        public static Task<string> UpdateParkingLot(string json) {
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
        public static Task<string> RemoveParkingLot(string json) {
            try {
                return http.Delete("/api/parking-lot/remove", json);
            } catch {
                Console.WriteLine("Couldn't Remove Parking Lot");
                throw;
            }
        }
    }
}