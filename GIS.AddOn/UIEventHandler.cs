using System;
using GIS.Framework;
using GIS.Framework.Helpers;

namespace GIS.AddOn
{
    internal class UIEventHandler
    {
        // Reference to the ConnectionManager to get the Application object
        private ConnectionManager _connectionManager;

        public UIEventHandler(ConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;

            // Subscribe to SAP UI events
            _connectionManager.SboApplication.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SboApplication_ItemEvent);
            LoggerHelper.Log("UIEventHandler initialized and listening for SAP events.");
        }

        private void SboApplication_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                // 1. ADD THE BUTTON ON FORM LOAD
                if ((pVal.FormTypeEx == "133" || pVal.FormTypeEx == "179") && pVal.EventType == SAPbouiCOM.BoEventTypes.et_FORM_LOAD && !pVal.BeforeAction)
                {
                    try
                    {
                        var oForm = _connectionManager.SboApplication.Forms.Item(FormUID);

                        // Add a button next to the 'Cancel' button (Item UID '2' on standard SAP forms)
                        var oItem = oForm.Items.Add("btnGISComb", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                        var cancelBtn = oForm.Items.Item("2");

                        oItem.Left = cancelBtn.Left - 100;
                        oItem.Top = cancelBtn.Top;
                        oItem.Width = 90;
                        oItem.Height = cancelBtn.Height;

                        var oButton = (SAPbouiCOM.Button)oItem.Specific;
                        oButton.Caption = "GIS: Combined";
                    }
                    catch
                    {
                        // Ignore if button already exists
                    }
                }

                // 2. LISTEN FOR THE BUTTON CLICK
                if ((pVal.FormTypeEx == "133" || pVal.FormTypeEx == "179") && pVal.EventType == SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    if (pVal.ItemUID == "btnGISComb")
                    {
                        LoggerHelper.Log("GIS Button Clicked! Initializing framework...");

                        // Retrieve the internal DocEntry from the Form's DataSource
                        var oForm = _connectionManager.SboApplication.Forms.Item(FormUID);
                        string objType = (pVal.FormTypeEx == "133") ? "13" : "14";
                        string tableName = (objType == "13") ? "OINV" : "ORIN";

                        string docEntry = oForm.DataSources.DBDataSources.Item(tableName).GetValue("DocEntry", 0).Trim();

                        if (string.IsNullOrEmpty(docEntry))
                        {
                            _connectionManager.SboApplication.MessageBox("Please Add the document first!");
                            return;
                        }

                        // Execute the Framework
                        var dbHelper = new DatabaseHelper(_connectionManager.Company);

                        _connectionManager.SboApplication.StatusBar.SetText("Generating E-Invoice, please wait...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                        string apiRes = CombinedEngine.ProcessCombined(dbHelper, objType, docEntry);

                        if (apiRes.Contains("\"status_cd\":\"1\"") && !apiRes.Contains("\"error\""))
                        {
                            _connectionManager.SboApplication.StatusBar.SetText("E-Invoice Generated Successfully!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                        }
                        else
                        {
                            _connectionManager.SboApplication.StatusBar.SetText("E-Invoice Failed. Check Comments on Document.", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Log($"Error in UI Event: {ex.Message}");
                _connectionManager?.SboApplication?.StatusBar?.SetText($"Error: {ex.Message}", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
        }
    }
}