using GIS.Framework.Interfaces;
using System;

namespace GIS.Framework.Helpers
{
    internal class CombinedResponseHandler : IResponseHandler
    {
        public void Handle(IDatabaseHelper dbHelper, string objType, string docEntry, string rawJsonRequest, string rawJsonResponse)
        {
            LoggerHelper.Log("Executing CombinedResponseHandler logic...");

            try
            {
                string safeRequest = rawJsonRequest?.Replace("'", "''") ?? "";
                string safeResponse = rawJsonResponse?.Replace("'", "''") ?? "";
                string sDocTypeLog = (objType == "13") ? "IN" : (objType == "14" ? "CM" : objType);
                string sDocType = (objType == "13") ? "Invoice" : (objType == "14" ? "CreditMemo" : "Transfer");

                // 1. Insert into TEC_EI_LOG (Legacy Logging)
                string logQuery = $"INSERT INTO \"TEC_EI_LOG\" VALUES('{docEntry}', '{sDocTypeLog}', '{safeRequest}', '{safeResponse}', CURRENT_TIMESTAMP, 'Addon')";
                dbHelper.ExecuteNonQuery(logQuery);
                LoggerHelper.Log("Combined Payload successfully written to TEC_EI_LOG.");

                // 2. Parse the response and update SAP directly
                if (rawJsonResponse != null && rawJsonResponse.Contains("\"status_cd\":\"1\""))
                {
                    try
                    {
                        var jObj = Newtonsoft.Json.Linq.JObject.Parse(rawJsonResponse);
                        string irn = jObj["Data"]?["Irn"]?.ToString() ?? "";
                        string ackNo = jObj["Data"]?["AckNo"]?.ToString() ?? "";
                        string ackDt = jObj["Data"]?["AckDt"]?.ToString() ?? "";
                        string signedInvoice = jObj["Data"]?["SignedInvoice"]?.ToString() ?? "";
                        string signedQRCode = jObj["Data"]?["SignedQRCode"]?.ToString() ?? "";
                        string ewbNo = jObj["Data"]?["EwbNo"]?.ToString() ?? "";
                        string ewbValidTill = jObj["Data"]?["EwbValidTill"]?.ToString() ?? "";

                        // Extract Distance from Request payload
                        string distance = "";
                        try
                        {
                            var reqObj = Newtonsoft.Json.Linq.JObject.Parse(rawJsonRequest);
                            distance = reqObj["EwayBillDetails"]?["Distance"]?.ToString() ?? "";
                        }
                        catch { }

                        if (!string.IsNullOrEmpty(irn))
                        {
                            string tableName = (objType == "13") ? "OINV" : (objType == "14" ? "ORIN" : "OWTR");

                            // Update SAP Document
                            string updateQuery = $"UPDATE \"{tableName}\" SET \"Comments\"='SUCCESS', \"U_IRN\"='{irn}', \"U_ACKNo\"='{ackNo}', \"U_ACKDt\"='{ackDt}', \"U_SgnInv\"='{signedInvoice}', \"U_SgnQrC\"='{signedQRCode}', \"U_EwbNo\"='{ewbNo}', \"U_EwbVal\"='{ewbValidTill}'";
                            if (!string.IsNullOrEmpty(distance)) updateQuery += $", \"U_TotalDist\"='{distance}'";
                            updateQuery += $" WHERE \"DocEntry\"={docEntry}";

                            dbHelper.ExecuteNonQuery(updateQuery);
                            LoggerHelper.Log($"Successfully updated U_IRN and details on {tableName} {docEntry}");

                            // Insert into GIS_EI_ORES
                            string insertOres = $"INSERT INTO \"GIS_EI_ORES\" (\"DocEntry\", \"DocType\", \"Error_Message\", \"Status\", \"AckNo\", \"AckDt\", \"Irn\", \"SignedInvoice\", \"SignedQRCode\", \"EWBNo\", \"EWBValidTill\") VALUES ('{docEntry}', '{sDocType}', 'SUCCESS', 'Success', '{ackNo}', '{ackDt}', '{irn}', '{signedInvoice}', '{signedQRCode}', '{ewbNo}', '{ewbValidTill}')";
                            dbHelper.ExecuteNonQuery(insertOres);
                            LoggerHelper.Log($"Successfully inserted record into GIS_EI_ORES for DocEntry {docEntry}.");
                        }
                    }
                    catch (Exception parseEx)
                    {
                        LoggerHelper.Log("Failed to parse IRN from success response: " + parseEx.Message);
                    }
                }
                else
                {
                    LoggerHelper.Log("API returned an error. Logged to TEC_EI_LOG. Will update Comments with error.");
                    string tableName = (objType == "13") ? "OINV" : (objType == "14" ? "ORIN" : "OWTR");

                    // Extract a clean error message
                    string errMsg = "Unknown Error";
                    try
                    {
                        var errObj = Newtonsoft.Json.Linq.JObject.Parse(rawJsonResponse);
                        if (errObj["error"] != null && errObj["error"]["message"] != null)
                        {
                            errMsg = errObj["error"]["message"].ToString();
                        }
                        else if (errObj["ErrorDetails"] != null && errObj["ErrorDetails"].HasValues)
                        {
                            errMsg = errObj["ErrorDetails"][0]["ErrorMessage"]?.ToString() ?? errMsg;
                        }
                    }
                    catch { }

                    string updateQuery = $"UPDATE \"{tableName}\" SET \"Comments\" = '{errMsg.Replace("'", "''")}' WHERE \"DocEntry\" = {docEntry}";
                    dbHelper.ExecuteNonQuery(updateQuery);
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Log($"CRITICAL ERROR in CombinedResponseHandler: {ex.Message}");
                throw;
            }
        }
    }
}