using Newtonsoft.Json;
using System.Collections.Generic;

namespace GIS.Framework.Models.EInvoice
{
    internal class ItemDetails
    {
        [JsonProperty("SlNo")]
        public string SlNo { get; set; }

        [JsonProperty("PrdDesc")]
        public string ProductDescription { get; set; }

        [JsonProperty("IsServc")]
        public string IsService { get; set; }

        [JsonProperty("HsnCd")]
        public string HsnCode { get; set; }

        [JsonProperty("Qty")]
        public decimal Quantity { get; set; }

        [JsonProperty("Unit")]
        public string Unit { get; set; }

        [JsonProperty("UnitPrice")]
        public decimal UnitPrice { get; set; }

        [JsonProperty("TotAmt")]
        public decimal TotalAmount { get; set; }

        [JsonProperty("Discount")]
        public decimal Discount { get; set; }

        [JsonProperty("AssAmt")]
        public decimal AssessableAmount { get; set; }

        [JsonProperty("GstRt")]
        public decimal GstRate { get; set; }

        [JsonProperty("IgstAmt")]
        public decimal IgstAmount { get; set; }

        [JsonProperty("CgstAmt")]
        public decimal CgstAmount { get; set; }

        [JsonProperty("SgstAmt")]
        public decimal SgstAmount { get; set; }

        [JsonProperty("CesRt")]
        public decimal CessRate { get; set; }

        [JsonProperty("CesAmt")]
        public decimal CessAmount { get; set; }

        [JsonProperty("CesNonAdvlAmt", NullValueHandling = NullValueHandling.Ignore)]
        public decimal CessNonAdvolAmount { get; set; }

        [JsonProperty("StateCesRt", NullValueHandling = NullValueHandling.Ignore)]
        public decimal StateCessRate { get; set; }

        [JsonProperty("StateCesAmt", NullValueHandling = NullValueHandling.Ignore)]
        public decimal StateCessAmount { get; set; }

        [JsonProperty("StateCesNonAdvlAmt", NullValueHandling = NullValueHandling.Ignore)]
        public decimal StateCessNonAdvolAmount { get; set; }

        [JsonProperty("OthChrg", NullValueHandling = NullValueHandling.Ignore)]
        public decimal OtherCharges { get; set; }

        [JsonProperty("TotItemVal")]
        public decimal TotalItemValue { get; set; }
    }
}