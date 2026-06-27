using Newtonsoft.Json;

namespace GIS.Framework.Models.EInvoice
{
    internal class PartyDetails
    {
        [JsonProperty("Gstin")]
        public string Gstin { get; set; }

        [JsonProperty("LglNm")]
        public string LegalName { get; set; }

        [JsonProperty("TrdNm")]
        public string TradeName { get; set; }

        [JsonProperty("Pos")]
        public string StateCode { get; set; }

        [JsonProperty("Addr1")]
        public string Address1 { get; set; }

        [JsonProperty("Addr2")]
        public string Address2 { get; set; }

        [JsonProperty("Loc")]
        public string Location { get; set; }

        [JsonProperty("Pin")]
        public int Pincode { get; set; }

        [JsonProperty("Stcd")]
        public string StateCode_Address { get; set; }

        [JsonProperty("Ph", NullValueHandling = NullValueHandling.Ignore)]
        public string Phone { get; set; }

        [JsonProperty("Em", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }
    }
}