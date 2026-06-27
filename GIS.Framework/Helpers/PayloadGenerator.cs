using GIS.Framework.Interfaces;
using GIS.Framework.Models.EInvoice;
using Newtonsoft.Json;
using System;
using System.Data;

namespace GIS.Framework.Helpers
{
    public static class PayloadGenerator
    {
        public static Tuple<string, string> GenerateCombinedPayload(IDatabaseHelper dbHelper, string objType, string docEntry)
        {
            LoggerHelper.Log($"Starting Combined Payload Generation for ObjType: {objType}, DocEntry: {docEntry}...");

            string headerQuery = $"CALL \"GIS_EInvoice_Get_GenInvoiceDet_Split\"('{objType}','{docEntry}','Addon','Header')";
            DataTable headerTable = dbHelper.ExecuteQuery(headerQuery);
            if (headerTable == null || headerTable.Rows.Count == 0) throw new Exception("No header data returned from GIS_EInvoice_Get_GenInvoiceDet_Split.");

            string detailQuery = $"CALL \"GIS_EInvoice_Get_GenInvoiceDet_Split\"('{objType}','{docEntry}','Addon','Detail')";
            DataTable detailTable = dbHelper.ExecuteQuery(detailQuery);
            if (detailTable == null || detailTable.Rows.Count == 0) throw new Exception("No detail data returned from GIS_EInvoice_Get_GenInvoiceDet_Split.");

            LoggerHelper.Log("Mapping raw SAP data to E-Invoice Models...");
            EInvoicePayload payload = PayloadMappers.MapHeader(headerTable.Rows[0]);
            payload.ItemList = PayloadMappers.MapDetails(detailTable);

            // Fetch E-Way Bill Details from the proper EWayBill Stored Procedure
            try
            {
                string sDocType = (objType == "13") ? "Invoice" : (objType == "14" ? "CreditMemo" : objType);
                string ewbQuery = $"CALL \"GIS_EwayBill_GetPostDet\"('{docEntry}', '', '', '', '', '', '', '', '', '', '{sDocType}', 'Header')";
                DataTable ewbTable = dbHelper.ExecuteQuery(ewbQuery);

                if (ewbTable != null && ewbTable.Rows.Count > 0)
                {
                    DataRow ewbRow = ewbTable.Rows[0];
                    if (ewbRow.Table.Columns.Contains("transporterId") || ewbRow.Table.Columns.Contains("vehicleNo"))
                    {
                        payload.EwayBillDetails = new EwbDetails
                        {
                            TransporterId = ewbRow.Table.Columns.Contains("transporterId") ? ewbRow["transporterId"].ToString() : "",
                            TransporterName = ewbRow.Table.Columns.Contains("transporterName") ? ewbRow["transporterName"].ToString() : "",
                            TransporterMode = ewbRow.Table.Columns.Contains("transMode") ? ewbRow["transMode"].ToString() : "",
                            Distance = ewbRow.Table.Columns.Contains("transDistance") ? Convert.ToInt32(ewbRow["transDistance"]) : 0,
                            TransporterDocNo = ewbRow.Table.Columns.Contains("transDocNo") ? ewbRow["transDocNo"].ToString() : "",
                            TransporterDocDate = ewbRow.Table.Columns.Contains("transDocDate") ? ewbRow["transDocDate"].ToString() : "",
                            VehicleNo = ewbRow.Table.Columns.Contains("vehicleNo") ? ewbRow["vehicleNo"].ToString() : "",
                            VehicleType = ewbRow.Table.Columns.Contains("vehicleType") ? ewbRow["vehicleType"].ToString() : "R"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Log("Warning: Failed to fetch E-Way Bill details, omitting EwbDetails from payload. " + ex.Message);
            }

            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            string jsonPayload = JsonConvert.SerializeObject(payload, settings);

            // Extract the Target URL directly from the database response!
            string targetUrl = headerTable.Rows[0].Table.Columns.Contains("URL") ? headerTable.Rows[0]["URL"].ToString().Trim() : "https://gstsandbox.charteredinfo.com/eicore/dec/v1.03/Invoice";

            return new Tuple<string, string>(jsonPayload, targetUrl);
        }
    }
}