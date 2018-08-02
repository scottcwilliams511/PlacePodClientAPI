using Newtonsoft.Json;


namespace PlacePodApiClient.Models {

    /// <summary>
    /// Represents a gateway entity.
    /// </summary>
    internal class Gateway {

        /// <summary>
        /// Unique Id of the gateway.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gateway's mac address.
        /// </summary>
        [JsonProperty("gatewayMac")]
        public string GatewayMac { get; set; }

        /// <summary>
        /// Name of Gateway.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Unique ID of the Parking lot this gateway is assigned to.
        /// </summary>
        [JsonProperty("parkingLotId")]
        public string ParkingLotId { get; set; }
    }
}
