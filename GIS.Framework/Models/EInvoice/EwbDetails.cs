using Newtonsoft.Json;

namespace GIS.Framework.Models.EInvoice
{
    internal class EwbDetails
    {
        [JsonProperty("TransId")]
        public string TransporterId { get; set; }

        [JsonProperty("TransName")]
        public string TransporterName { get; set; }

        [JsonProperty("TransMode")]
        public string TransporterMode { get; set; }

        [JsonProperty("Distance")]
        public int Distance { get; set; }

        [JsonProperty("TransDocNo")]
        public string TransporterDocNo { get; set; }

        [JsonProperty("TransDocDt")]
        public string TransporterDocDate { get; set; }

        [JsonProperty("VehNo")]
        public string VehicleNo { get; set; }

        [JsonProperty("VehType")]
        public string VehicleType { get; set; }
    }
}
