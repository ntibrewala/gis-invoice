using GIS.Framework.Interfaces;
using Newtonsoft.Json.Linq;
using System;

namespace GIS.Framework.Helpers
{
    public class EWayBillResponseHandler : IResponseHandler
    {
        public void Handle(IDatabaseHelper dbHelper, string objType, string docEntry, string payload, string apiResponse)
        {
            LoggerHelper.Log($"Handling API Response for E-Way Bill Generation (DocEntry: {docEntry})...");

            string tableName = (objType == "13") ? "OINV" : (objType == "14") ? "ORIN" : "OWTR";
            
            LoggerHelper.Log($"Request Payload: {payload}");
            LoggerHelper.Log($"API Response: {apiResponse}");
            
            // 1. Log the RAW request/response into TEC_EW_LOG
            string safePayload = payload.Replace("'", "''");
            string safeResponse = apiResponse.Replace("'", "''");
            string logQuery = $"INSERT INTO \"TEC_EW_LOG\" VALUES('{docEntry}', '{objType}', '{safePayload}', '{safeResponse}', CURRENT_TIMESTAMP, 'System')";
            dbHelper.ExecuteNonQuery(logQuery);

            try
            {
                JObject jObj = JObject.Parse(apiResponse);
                
                string status = jObj["Status"]?.ToString() ?? "";
                if (status == "1" || status.ToLower() == "true")
                {
                    // Success! 
                    if (jObj["Data"] != null)
                    {
                        string ewbNo = jObj["Data"]["ewayBillNo"]?.ToString() ?? "";
                        string validUpto = jObj["Data"]["validUpto"]?.ToString() ?? "";
                        
                        if (!string.IsNullOrEmpty(ewbNo))
                        {
                            // 2. Update SAP Document (OINV / ORIN / OWTR)
                            string updateDocQ = $"UPDATE \"{tableName}\" SET \"U_ewayBNo\" = '{ewbNo}', \"U_EwbVal\" = '{validUpto}' WHERE \"DocEntry\" = {docEntry}";
                            dbHelper.ExecuteNonQuery(updateDocQ);
                            LoggerHelper.Log($"Successfully updated SAP Document {tableName} with E-Way Bill Number: {ewbNo}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Log($"Error while parsing JSON response for E-Way Bill: {ex.Message}");
            }
        }
    }

    public class EWayBillCancelResponseHandler : IResponseHandler
    {
        public void Handle(IDatabaseHelper dbHelper, string objType, string docEntry, string payload, string apiResponse)
        {
            LoggerHelper.Log($"Handling API Response for E-Way Bill Cancel (DocEntry: {docEntry})...");

            string tableName = (objType == "13") ? "OINV" : (objType == "14") ? "ORIN" : "OWTR";
            
            LoggerHelper.Log($"Cancel Request Payload: {payload}");
            LoggerHelper.Log($"Cancel API Response: {apiResponse}");
            
            // 1. Log the RAW request/response into TEC_EW_LOG
            string safePayload = payload.Replace("'", "''");
            string safeResponse = apiResponse.Replace("'", "''");
            string logQuery = $"INSERT INTO \"TEC_EW_LOG\" VALUES('{docEntry}', '{objType}', '{safePayload}', '{safeResponse}', CURRENT_TIMESTAMP, 'System')";
            dbHelper.ExecuteNonQuery(logQuery);

            try
            {
                JObject jObj = JObject.Parse(apiResponse);
                
                string status = jObj["Status"]?.ToString() ?? "";
                if (status == "1" || status.ToLower() == "true")
                {
                    // Success!
                    
                    // Fetch existing EWay Bill number so we can append "- Cancelled"
                    string query = $"SELECT \"U_ewayBNo\" FROM \"{tableName}\" WHERE \"DocEntry\"={docEntry}";
                    var dt = dbHelper.ExecuteQuery(query);
                    string ewbNo = dt?.Rows[0]["U_ewayBNo"]?.ToString() ?? "";

                    if (!ewbNo.Contains("Cancelled"))
                    {
                        ewbNo = ewbNo + " - Cancelled";
                    }

                    // 2. Update SAP Document (OINV / ORIN / OWTR)
                    string updateDocQ = $"UPDATE \"{tableName}\" SET \"U_ewayBNo\" = '{ewbNo}', \"U_CanclDte\" = CURRENT_DATE WHERE \"DocEntry\" = {docEntry}";
                    dbHelper.ExecuteNonQuery(updateDocQ);
                    LoggerHelper.Log($"Successfully updated SAP Document {tableName} for E-Way Bill Cancel!");
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Log($"Error while parsing JSON response for E-Way Bill Cancel: {ex.Message}");
            }
        }
    }
}
