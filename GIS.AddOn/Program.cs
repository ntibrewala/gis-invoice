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

                // Keep the console open (In a real Add-On, this would be System.Windows.Forms.Application.Run)
                Console.WriteLine("Add-On is running. Press Enter to exit...");
                Console.ReadLine();
            }
            else
            {
                LoggerHelper.Log("Failed to start Add-On due to connection errors.");
            }
            
            LoggerHelper.Log("=== Exiting GIS SAP Add-On ===");
        }
    }
}
