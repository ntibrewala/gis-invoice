using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GIS.Framework.Configuration;
using GIS.Framework.Models;
using GIS.Framework.Interfaces;

namespace GIS.Framework.Helpers
{
    /// <summary>
    /// Orchestrates the authentication flow by binding credentials to the HttpHelper.
    /// </summary>
    public static class AuthHelper
    {
        private const string AUTH_URL = "https://gstsandbox.charteredinfo.com/eivital/dec/v1.04/auth";

        public static async Task<TokenResponse> GetCharteredAuthTokenAsync(IDatabaseHelper dbHelper, string objType, string docEntry)
        {
            LoggerHelper.Log($"Starting CharteredAuthToken fetch process for ObjType: {objType}, DocEntry: {docEntry}...");
            
            // 1. Fetch Credentials & Check Cache
            var credentials = CredentialManager.GetCredentialsForDocument(dbHelper, objType, docEntry);
            
            // Check if token exists and is valid for at least the next 5 minutes (buffer)
            bool isTokenValid = false;
            if (!string.IsNullOrEmpty(credentials.ExistingAuthToken) && credentials.ExistingTokenExpiry.HasValue)
            {
                if (credentials.ExistingTokenExpiry.Value > DateTime.Now.AddMinutes(5))
                {
                    isTokenValid = true;
                }
                else
                {
                    LoggerHelper.Log($"Token exists but expires soon ({credentials.ExistingTokenExpiry.Value}). Fetching new token...");
                }
            }

            if (isTokenValid)
            {
                LoggerHelper.Log($"Found valid, unexpired AuthToken in database cache for GSTIN {credentials.GSTIN}. Skipping HTTP API call.");
                return new TokenResponse 
                { 
                    Status = 1, 
                    Data = new TokenData 
                    { 
                        AuthToken = credentials.ExistingAuthToken,
                        TokenExpiry = credentials.ExistingTokenExpiry?.ToString("yyyy-MM-dd HH:mm:ss")
                    } 
                };
            }

            // 2. Map DB Credentials to HTTP Headers
            var headers = new Dictionary<string, string>
            {
                { "aspid", credentials.AspId },
                { "password", credentials.AspPassword },
                { "gstin", credentials.GSTIN },
                { "user_name", credentials.UserName },
                { "eInvPwd", credentials.ApiPassword }
            };
            
            LoggerHelper.Log("No valid token in cache. Making live HTTP call to CharteredInfo...");
            var tokenResponse = await HttpHelper.GetAsync<TokenResponse>(AUTH_URL, headers);
            
            if (tokenResponse.Status == 1 && tokenResponse.Data != null)
            {
                LoggerHelper.Log("Live fetch successful. Saving new token back to database cache...");
                CredentialManager.SaveTokenToCache(
                    dbHelper, 
                    credentials.GSTIN, 
                    tokenResponse.Data.ClientId, 
                    tokenResponse.Data.UserName, 
                    tokenResponse.Data.AuthToken, 
                    tokenResponse.Data.TokenExpiry
                );
            }

            return tokenResponse;
        }

        public static TokenResponse GetCharteredAuthTokenSync(IDatabaseHelper dbHelper, string objType, string docEntry)
        {
            return GetCharteredAuthTokenAsync(dbHelper, objType, docEntry).GetAwaiter().GetResult();
        }
    }
}
