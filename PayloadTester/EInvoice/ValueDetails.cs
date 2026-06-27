using Newtonsoft.Json;

namespace GIS.Framework.Models.EInvoice
{
    internal class ValueDetails
    {
        [JsonProperty("AssVal")]
        public decimal AssessableValue { get; set; }

        [JsonProperty("CgstVal")]
        public decimal CgstValue { get; set; }

        [JsonProperty("SgstVal")]
        public decimal SgstValue { get; set; }

        [JsonProperty("IgstVal")]
        public decimal IgstValue { get; set; }

        [JsonProperty("CesVal")]
        public decimal CessValue { get; set; }

        [JsonProperty("StCesVal")]
        public decimal StateCessValue { get; set; }

        [JsonProperty("Discount")]
        public decimal Discount { get; set; }

        [JsonProperty("OthChrg")]
        public decimal OtherCharges { get; set; }

        [JsonProperty("RndOffAmt")]
        public decimal RoundOffAmount { get; set; }

        [JsonProperty("TotInvVal")]
        public decimal TotalInvoiceValue { get; set; }
    }
}
