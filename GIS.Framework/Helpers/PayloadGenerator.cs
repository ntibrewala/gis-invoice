using GIS.Framework.Interfaces;
using GIS.Framework.Models.EInvoice;
using Newtonsoft.Json;
using System;
using System.Data;

namespace GIS.Framework.Helpers
{
    public static class PayloadGenerator
    {
        public static Tuple<string, string> GenerateEInvoicePayload(IDatabaseHelper dbHelper, string objType, string docEntry)
        {
            LoggerHelper.Log($"Starting Standalone E-Invoice Payload Generation for ObjType: {objType}, DocEntry: {docEntry}...");

            string headerQuery = $"CALL \"GIS_EInvoice_Get_GenInvoiceDet_Split\"('{objType}','{docEntry}','Addon','Header')";
            DataTable headerTable = dbHelper.ExecuteQuery(headerQuery);
            if (headerTable == null || headerTable.Rows.Count == 0) throw new Exception("No header data returned from GIS_EInvoice_Get_GenInvoiceDet_Split.");

            string detailQuery = $"CALL \"GIS_EInvoice_Get_GenInvoiceDet_Split\"('{objType}','{docEntry}','Addon','Detail')";
            DataTable detailTable = dbHelper.ExecuteQuery(detailQuery);
            if (detailTable == null || detailTable.Rows.Count == 0) throw new Exception("No detail data returned from GIS_EInvoice_Get_GenInvoiceDet_Split.");

            LoggerHelper.Log("Mapping raw SAP data to Standalone E-Invoice Models...");
            EInvoicePayload payload = PayloadMappers.MapHeader(headerTable.Rows[0]);
            payload.ItemList = PayloadMappers.MapDetails(detailTable);
            
            // Explicitly do NOT set EwayBillDetails, so it gets omitted due to NullValueHandling.Ignore
            payload.EwayBillDetails = null;

            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            string jsonPayload = JsonConvert.SerializeObject(payload, settings);

            // Extract the Target URL directly from the database response
            string targetUrl = headerTable.Rows[0].Table.Columns.Contains("URL") ? headerTable.Rows[0]["URL"].ToString().Trim() : "https://gstsandbox.charteredinfo.com/eicore/dec/v1.03/Invoice";

            return new Tuple<string, string>(jsonPayload, targetUrl);
        }

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
                        string transId = ewbRow.Table.Columns.Contains("transporterId") ? ewbRow["transporterId"].ToString().Trim() : null;
                        string transName = ewbRow.Table.Columns.Contains("transporterName") ? ewbRow["transporterName"].ToString().Trim() : null;
                        string transMode = ewbRow.Table.Columns.Contains("transMode") ? ewbRow["transMode"].ToString().Trim() : null;
                        string transDocNo = ewbRow.Table.Columns.Contains("transDocNo") ? ewbRow["transDocNo"].ToString().Trim() : null;
                        string transDocDt = ewbRow.Table.Columns.Contains("transDocDate") ? ewbRow["transDocDate"].ToString().Trim() : null;
                        string vehNo = ewbRow.Table.Columns.Contains("vehicleNo") ? ewbRow["vehicleNo"].ToString().Trim() : null;
                        string vehType = ewbRow.Table.Columns.Contains("vehicleType") ? ewbRow["vehicleType"].ToString().Trim() : null;

                        payload.EwayBillDetails = new EwbDetails
                        {
                            TransporterId = string.IsNullOrEmpty(transId) ? null : transId,
                            TransporterName = string.IsNullOrEmpty(transName) ? null : transName,
                            TransporterMode = string.IsNullOrEmpty(transMode) ? null : transMode,
                            Distance = ewbRow.Table.Columns.Contains("transDistance") && ewbRow["transDistance"] != DBNull.Value && !string.IsNullOrEmpty(ewbRow["transDistance"].ToString()) ? Convert.ToInt32(ewbRow["transDistance"]) : 0,
                            TransporterDocNo = string.IsNullOrEmpty(transDocNo) ? null : transDocNo,
                            TransporterDocDate = string.IsNullOrEmpty(transDocDt) ? null : transDocDt,
                            VehicleNo = string.IsNullOrEmpty(vehNo) ? null : vehNo,
                            VehicleType = string.IsNullOrEmpty(vehType) ? "R" : vehType
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