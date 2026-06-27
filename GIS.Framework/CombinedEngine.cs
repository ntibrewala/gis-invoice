using GIS.Framework.Interfaces;
using GIS.Framework.Models;
using System;

namespace GIS.Framework
{
    public static class CombinedEngine
    {
        public static string ProcessCombined(IDatabaseHelper dbHelper, string objType, string docEntry)
        {
            try
            {
                Helpers.LoggerHelper.Log($"--- STARTING COMBINED PROCESS FOR DOC: {docEntry} ---");

                // Step 1: Fetch Credentials for this specific document
                Helpers.LoggerHelper.Log("Fetching API Credentials...");
                var credentials = Helpers.CredentialManager.GetCredentialsForDocument(dbHelper, objType, docEntry);

                if (credentials == null) throw new Exception("Could not resolve credentials for this document.");

                // Step 2: Check Token Cache / Fetch New Token
                Helpers.LoggerHelper.Log("Fetching Auth Token...");
                var tokenResponse = Helpers.AuthHelper.GetCharteredAuthTokenSync(dbHelper, objType, docEntry);

                if (tokenResponse.Data == null || string.IsNullOrEmpty(tokenResponse.Data.AuthToken)) throw new Exception("Failed to acquire valid AuthToken. Process aborted.");

                // Step 3: Generate the Combined JSON Payload & Extract Target URL
                Helpers.LoggerHelper.Log("Generating JSON Payload...");
                var payloadTuple = Helpers.PayloadGenerator.GenerateCombinedPayload(dbHelper, objType, docEntry);
                string jsonPayload = payloadTuple.Item1;
                string targetUrl = payloadTuple.Item2;

                // Step 4: Run the API
                Helpers.LoggerHelper.Log("Executing API POST...");
                string apiResponse = Helpers.ApiRunner.PostInvoice(
                    jsonPayload,
                    tokenResponse.Data.AuthToken,
                    credentials.GSTIN,
                    credentials.UserName,
                    credentials.AspPassword,
                    credentials.AspId,
                    targetUrl
                );

                // Step 5: Write the response to SAP (OINV/ORIN) and log to TEC_EI_LOG
                Helpers.LoggerHelper.Log("Executing Database Write-Back...");
                Helpers.ResponseHandler.RouteResponse("combined", dbHelper, objType, docEntry, jsonPayload, apiResponse);

                Helpers.LoggerHelper.Log($"--- COMPLETED COMBINED PROCESS FOR DOC: {docEntry} ---");
                return apiResponse;
            }
            catch (Exception ex)
            {
                Helpers.LoggerHelper.Log($"CRITICAL ENGINE FAILURE: {ex.Message}");
                throw;
            }
        }
    }
}