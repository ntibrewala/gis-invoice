using Newtonsoft.Json;

namespace GIS.Framework.Models.EInvoice
{
    internal class TransactionDetails
    {
        [JsonProperty("TaxSch")]
        public string TaxScheme { get; set; }

        [JsonProperty("SupTyp")]
        public string SupplyType { get; set; }

        [JsonProperty("RegRev")]
        public string ReverseCharge { get; set; }

        [JsonProperty("EcmGstin", NullValueHandling = NullValueHandling.Ignore)]
        public string ECommerceGstin { get; set; }

        [JsonProperty("IgstOnInt")]
        public string IgstOnIntra { get; set; }
    }
}
