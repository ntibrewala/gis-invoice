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

        private bool ConfirmAction(string buttonCaption)
        {
            string msg = $"Are you sure you want to run the {buttonCaption} generation?";
            int userResponse = _connectionManager.SboApplication.MessageBox(msg, 1, "Yes", "No", "");
            return userResponse == 1;
        }

        private void SboApplication_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                // LISTEN FOR COMBOBOX SELECTION ON THE UDF FORM (-133 or -179)
                if ((pVal.FormTypeEx == "-133" || pVal.FormTypeEx == "-179") && pVal.EventType == SAPbouiCOM.BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    if (pVal.ItemUID == "U_btnComb" || pVal.ItemUID == "U_btnEWay" || pVal.ItemUID == "U_btnEInv")
                    {
                        var oUDFForm = _connectionManager.SboApplication.Forms.Item(FormUID);
                        var combo = (SAPbouiCOM.ComboBox)oUDFForm.Items.Item(pVal.ItemUID).Specific;
                        
                        string selectedValue = "";
                        try { selectedValue = combo.Selected.Value.Trim(); } catch { return; }

                        if (string.IsNullOrEmpty(selectedValue) || selectedValue.ToLower() == "combined" || selectedValue.ToLower() == "eway" || selectedValue.ToLower() == "einv")
                        {
                            return; // Ignored default placeholder values
                        }

                        // Determine the Action Type based on the combobox and selected value
                        string actionName = "";
                        if (pVal.ItemUID == "U_btnComb" && selectedValue == "1") actionName = "Generate Combined";
                        if (pVal.ItemUID == "U_btnComb" && selectedValue == "2") actionName = "Cancel Combined";
                        
                        // (You will provide the values for EWay and EInv later, I'm setting up placeholders)
                        if (pVal.ItemUID == "U_btnEWay" && selectedValue == "1") actionName = "Generate E-Way Bill";
                        if (pVal.ItemUID == "U_btnEWay" && selectedValue == "2") actionName = "Cancel E-Way Bill";
                        
                        if (pVal.ItemUID == "U_btnEInv" && selectedValue == "1") actionName = "Generate E-Invoice";
                        if (pVal.ItemUID == "U_btnEInv" && selectedValue == "2") actionName = "Cancel E-Invoice";

                        if (string.IsNullOrEmpty(actionName)) return;

                        // Reusable Confirmation Helper
                        if (!ConfirmAction(actionName))
                        {
                            // Reset the combobox back to default (0 index) if user clicks No
                            combo.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
                            return;
                        }

                        // Retrieve the internal DocEntry
                        string objType = (pVal.FormTypeEx == "-133") ? "13" : "14";
                        string tableName = (objType == "13") ? "OINV" : "ORIN";
                        string docEntry = oUDFForm.DataSources.DBDataSources.Item(tableName).GetValue("DocEntry", 0).Trim();

                        if (string.IsNullOrEmpty(docEntry))
                        {
                            _connectionManager.SboApplication.MessageBox("Please Add the document first!");
                            combo.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
                            return;
                        }

                        if (actionName == "Generate Combined")
                        {
                            LoggerHelper.Log($"GIS UDF Selected! Initializing Combined Generation for DocEntry: {docEntry}...");
                            var dbHelper = new DatabaseHelper(_connectionManager.Company);
                            _connectionManager.SboApplication.StatusBar.SetText("Generating E-Invoice, please wait...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                            
                            string apiRes = CombinedEngine.ProcessCombined(dbHelper, objType, docEntry);
                            bool isSuccess = apiRes.Contains("\"Status\":\"1\"") || apiRes.Contains("\"Status\":1") || apiRes.Contains("\"status_cd\":\"1\"") || apiRes.Contains("DUPIRN");

                            if (isSuccess && !apiRes.Contains("\"error\""))
                                _connectionManager.SboApplication.StatusBar.SetText("Combined Generated Successfully!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                            else
                                _connectionManager.SboApplication.StatusBar.SetText("Combined Failed. Check Comments on Document.", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }
                        else
                        {
                            _connectionManager.SboApplication.MessageBox($"The action '{actionName}' is not implemented yet!");
                        }

                        // Reset the combobox back to default after execution
                        combo.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
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