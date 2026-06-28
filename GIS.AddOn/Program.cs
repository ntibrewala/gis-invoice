using System;
using GIS.Framework.Helpers;

namespace GIS.AddOn
{
    class Program
    {
        static void Main(string[] args)
        {
            LoggerHelper.Log("=== Starting GIS SAP Add-On ===");

            var connectionManager = new ConnectionManager();
            if (connectionManager.Connect(args))
            {
                // Instantiate the dedicated UI Event Handler class
                var uiEventHandler = new UIEventHandler(connectionManager);

                // Start the standard Windows message loop which keeps the process alive
                // and responsive to SAP B1 COM events
                System.Windows.Forms.Application.Run();
            }
            else
            {
                LoggerHelper.Log("Failed to start Add-On due to connection errors.");
            }
            
            LoggerHelper.Log("=== Exiting GIS SAP Add-On ===");
        }
    }
}
