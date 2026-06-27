using System;

namespace GIS.Framework.Models.EInvoice
{
    public class ReqPlCancelIRN
    {
        public string Irn { get; set; }
        public string CnlRsn { get; set; }
        public string CnlRem { get; set; }
    }
    
    public class RespPlCancelIRN
    {
        public string Irn { get; set; }
        public string CancelDate { get; set; }
    }
}
