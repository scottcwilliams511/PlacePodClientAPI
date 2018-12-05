using Newtonsoft.Json;

namespace PlacePodApiClient.Models {

    /// <summary>
    /// Represents an entity which contains two of Sensors.
    /// </summary>
    public class Driveway {

        /// <summary>
        /// Unique Id of the driveway.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name of this driveway.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// SensorId of the PlacePod in the front position.
        /// </summary>
        [JsonProperty("frontSensorId")]
        public string FrontSensorId { get; set; }

        /// <summary>
        /// SensorId of the PlacePod in the back position.
        /// </summary>
        [JsonProperty("backSensorId")]
        public string BackSensorId { get; set; }

        /// <summary>
        /// Unique Id of the Parking Lot that this is associated to.
        /// </summary>
        [JsonProperty("parkingLotId")]
        public string ParkingLotId { get; set; }

        /// <summary>
        /// Is this driveway going into or out of the Parking Lot?
        /// </summary>
        [JsonProperty("isDirectionIn")]
        public bool IsDirectionIn { get; set; }

        /// <summary>
        /// The last estimated count of vehicles this driveway has recorded.
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }


        internal string GetPrintString() {
            return $"--> {Id}: {Name}. Count: {Count}";
        }
    }
}
