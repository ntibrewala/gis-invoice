using Newtonsoft.Json;

namespace GIS.Framework.Models.EInvoice
{
    internal class DocumentDetails
    {
        [JsonProperty("Typ")]
        public string Type { get; set; }

        [JsonProperty("No")]
        public string Number { get; set; }

        [JsonProperty("Dt")]
        public string Date { get; set; }
    }
}
