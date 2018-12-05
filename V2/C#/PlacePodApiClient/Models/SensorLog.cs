using System;
using Newtonsoft.Json;

namespace PlacePodApiClient.Models {

    /// <summary>
    /// Represents an event reported up to the cloud from the PlacePod.
    /// </summary>
    public class SensorLog {

        /// <summary>
        /// Placepod Sensor ID from the barcode.
        /// </summary>
        [JsonProperty("sensorId")]
        public string SensorId { get; set; }

        /// <summary>
        /// What type of mode the sensor reports. Currently two possibilities:
        ///     - CarPresence (PlacePod Vehicle Detection Sensor)
        ///     - CarCounter  (PlacePod Vehicle Counting Sensor)
        /// </summary>
        [JsonProperty("mode")]
        public string Mode { get; set; }

        /// <summary>
        /// String representation of the car presence value.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// State of sensor (only if mode is "CarPresence". otherwise this is null):
        /// -   1 = No Car Present (vacant)
        /// -   3 = Parked Car Detected (occupied)
        /// </summary>
        [JsonProperty("carPresence")]
        public int? CarPresence { get; set; }

        /// <summary>
        /// Last recorded number of vehicles (only if mode is "CarCounter", otherwise this is null).
        ///
        /// NOTE: For R05, the range of values this can be is [0, 128].
        /// If the last value was 127, then the next value will roll over to 0.
        /// A value of 128 indicates that the PlacePod has been recalibrated or rebooted and has an
        /// internal count of 0.
        /// </summary>
        [JsonProperty("carCounter")]
        public int? CarCounter { get; set; }

        /// <summary>
        /// Was the log a keep alive message?
        /// </summary>
        [JsonProperty("keepAlive")]
        public bool KeepAlive { get; set; }

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
        /// Sensor's current RSSI (received signal strength indication) status.
        /// </summary>
        [JsonProperty("rssi")]
        public double Rssi { get; set; }

        /// <summary>
        /// Sensor's current SNR (signal-to-noise ratio) status.
        /// </summary>
        [JsonProperty("snr")]
        public double Snr { get; set; }

        /// <summary>
        /// Temperature in celcius.
        /// - NOTE: For R05, the value is sent up once an hour only if it has changed
        ///         by +/- 3.0°C, so this will mostly be null.
        /// </summary>
        [JsonProperty("temperature")]
        public double? Temperature { get; set; }

        /// <summary>
        /// Battery Voltage.
        /// - NOTE: For R05, the value is sent up once an hour only if it has changed
        ///         by +/- 0.01v, so this may be null.
        /// </summary>
        [JsonProperty("battery")]
        public double? Battery { get; set; }


        internal string GetPrintString() {
            string status = (Mode == "CarPresence") ? Status : $"{CarCounter} car(s)";
            string keepAlive = (KeepAlive) ? " (Keep Alive)" : "";

            return $"--> {SensorId} at {ServerTime}{keepAlive}: {status}";
        }
    }
}
