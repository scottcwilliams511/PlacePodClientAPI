using Newtonsoft.Json;


namespace PlacePodApiClient.Models {

    /// <summary>
    /// Represents an entity which contains a collection of PlacePods and possibly Gateways.
    /// </summary>
    internal class ParkingLot {

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
        [JsonProperty("streetAddress")]
        public string StreetAddress { get; set; }

        /// <summary>
        /// Parking lot's latitude coordinate
        /// </summary>
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        /// <summary>
        /// Parking lot's longitude coordinate
        /// </summary>
        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        /// <summary>
        /// Name of the camera id
        /// </summary>
        [JsonProperty("cameraId")]
        public string CameraId { get; set; }
    }
}
