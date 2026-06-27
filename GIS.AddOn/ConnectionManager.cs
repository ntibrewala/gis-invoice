using System;
using GIS.Framework.Helpers;

namespace GIS.AddOn
{
    internal class ConnectionManager
    {
        // SAPbouiCOM and SAPbobsCOM objects
        // When building in Visual Studio on Windows, add references to SAPbouiCOM and SAPbobsCOM COM libraries.
        public SAPbouiCOM.Application SboApplication { get; private set; }
        public SAPbobsCOM.Company Company { get; private set; }

        public bool Connect(string[] args)
        {
            LoggerHelper.Log("Attempting to connect to SAP UI API...");
            try
            {
                SAPbouiCOM.SboGuiApi SboGuiApi = new SAPbouiCOM.SboGuiApi();

                // The magic development connection string for debugging in Visual Studio
                string connectionString = "0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056";

                // If SAP launches the Add-On via Extension Manager, it passes the real string as the first argument
                if (args != null && args.Length > 0)
                {
                    connectionString = args[0];
                }

                SboGuiApi.Connect(connectionString);
                SboApplication = SboGuiApi.GetApplication(-1);
                LoggerHelper.Log("Successfully connected to SAP UI API.");

                LoggerHelper.Log("Attempting to connect to SAP DI API...");
                Company = (SAPbobsCOM.Company)SboApplication.Company.GetDICompany();
                LoggerHelper.Log("Successfully connected to SAP DI API.");

                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Log($"Connection failed: {ex.Message}");
                return false;
            }
        }
    }
}
