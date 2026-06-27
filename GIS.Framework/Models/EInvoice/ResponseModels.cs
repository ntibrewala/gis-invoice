using System;
using System.Collections.Generic;

namespace GIS.Framework.Models.EInvoice
{
    internal class RespPlGenIRN
    {
        public string AckNo { get; set; }
        public string AckDt { get; set; }
        public string Irn { get; set; }
        public string SignedInvoice { get; set; }
        public string SignedQRCode { get; set; }
        public string Status { get; set; }
        public string EwbNo { get; set; }
        public string EwbDt { get; set; }
        public string EwbValidTill { get; set; }
        public RespGenIRNInvData ExtractedSignedInvoiceData { get; set; }
        public RespGenIRNQrCodeData ExtractedSignedQrCode { get; set; }
        public string JwtIssuer { get; set; }
    }

    internal class RespGenIRNQrCodeData
    {
        public string SellerGstin { get; set; }
        public string BuyerGstin { get; set; }
        public string DocNo { get; set; }
        public string DocTyp { get; set; }
        public string DocDt { get; set; }
        public Nullable<double> TotInvVal { get; set; }
        public string ItemCnt { get; set; }
        public string MainHsnCode { get; set; }
        public string Irn { get; set; }
    }

    internal class RespGenIRNInvData
    {
        public long AckNo { get; set; }
        public string AckDt { get; set; }
        public string Version { get; set; }
        public string Irn { get; set; }
        // For now, we only need the root details from the response to update SAP.
        // We can add the full deeply nested objects here if required later.
    }
}
