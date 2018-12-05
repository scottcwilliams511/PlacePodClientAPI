using Newtonsoft.Json;

namespace PlacePodApiClient.Models {

    /// <summary>
    /// Represents an entity which contains a collection of Sensors and possibly Driveways
    /// </summary>
    public class ParkingLot {

        /// <summary>
        /// Unique ID of the parking lot.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Name of Parking Lot.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Description of parking lot
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Parking lot's street address
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// Parking lot's latitude coordinate
        /// </summary>
        [JsonProperty("latitude")]
        public double? Latitude { get; set; }

        /// <summary>
        /// Parking lot's longitude coordinate
        /// </summary>
        [JsonProperty("longitude")]
        public double? Longitude { get; set; }

        /// <summary>
        /// Total number of parking spaces in the lot.
        /// </summary>
        [JsonProperty("totalSpaces")]
        public int? TotalSpaces { get; set; }

        /// <summary>
        /// The estimated number of vacant parking spaces in the parking lot.
        /// </summary>
        [JsonProperty("count")]
        public int? Count { get; set; }


        internal string GetPrintString() {
            return $"--> {Id}: {Name}";
        }
    }
}
