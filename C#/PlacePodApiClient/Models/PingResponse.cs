using Newtonsoft.Json;
using System;


namespace PlacePodApiClient.Models {

    /// <summary>
    /// Model holding the response of a Ping.
    /// </summary>
    internal class PingResponse {

        /// <summary>
        /// RSSI from initial Ping
        /// </summary>
        [JsonProperty("pingRssi")]
        public float PingRssi { get; set; }

        /// <summary>
        /// SNR from inital ping 
        /// </summary>
        [JsonProperty("pingSNR")]
        public float PingSNR { get; set; }

        /// <summary>
        /// Placepod Sensor ID from the barcode.
        /// </summary>
        [JsonProperty("sensorId")]
        public string SensorId { get; set; }

        /// <summary>
        /// Time Event was received UTC.
        /// </summary>
        [JsonProperty("serverTime")]
        public DateTime? ServerTime { get; set; }

        /// <summary>
        /// Time from gateway. This may not be accurate since gateway may have clock skew and/or incorrect time settings.
        /// </summary>
        [JsonProperty("gatewayTime")]
        public DateTime? GatewayTime { get; set; }

        /// <summary>
        /// Time from Sentral coprocessor in ticks.
        /// </summary>
        [JsonProperty("sentralTime")]
        public double SentralTime { get; set; }

        /// <summary>
        /// Sensor's received signal strength indication
        /// </summary>
        [JsonProperty("loraWan.rssi")]
        public double LoraRssi { get; set; }

        /// <summary>
        /// Sensor's signal to noise ratio
        /// </summary>
        [JsonProperty("loraWan.loRaSNR")]
        public double LoRaSNR { get; set; }

        /// <summary>
        /// Number of gateways connecting to the sensor
        /// </summary>
        [JsonProperty("loraWan.gwCount")]
        public int LoRaGatewayCount { get; set; }
    }
}
