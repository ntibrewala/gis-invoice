using System;
using GIS.Framework;
using GIS.Framework.Helpers;
using GIS.AddOn.Helpers;

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
                // DRAW UI ON FORM LOAD FOR INVOICE (133) AND CREDIT MEMO (179)
                if ((pVal.FormTypeEx == "133" || pVal.FormTypeEx == "179") && pVal.EventType == SAPbouiCOM.BoEventTypes.et_FORM_LOAD && !pVal.BeforeAction)
                {
                    var oForm = _connectionManager.SboApplication.Forms.Item(FormUID);
                    UIHelper.DrawLegacyButtonCombos(oForm);
                    return;
                }

                // LISTEN FOR COMBOBOX SELECTION ON THE MAIN FORM (133 or 179) FOR OUR CUSTOM BUTTON COMBOS
                if ((pVal.FormTypeEx == "133" || pVal.FormTypeEx == "179") && pVal.EventType == SAPbouiCOM.BoEventTypes.et_COMBO_SELECT && !pVal.BeforeAction)
                {
                    if (pVal.ItemUID == "btnComb" || pVal.ItemUID == "btnEWay" || pVal.ItemUID == "btnEInv")
                    {
                        var oForm = _connectionManager.SboApplication.Forms.Item(FormUID);
                        var combo = (SAPbouiCOM.ButtonCombo)oForm.Items.Item(pVal.ItemUID).Specific;
                        
                        string selectedValue = "";
                        try { selectedValue = combo.Selected.Value.Trim(); } catch { return; }

                        if (string.IsNullOrEmpty(selectedValue) || selectedValue.ToLower() == "combined" || selectedValue.ToLower() == "ewaybill" || selectedValue.ToLower() == "e-invoice")
                        {
                            return; // Ignored default placeholder values
                        }

                        // Determine the Action Type based on the combobox and selected value
                        string actionName = "";
                        if (pVal.ItemUID == "btnComb" && selectedValue == "Generate") actionName = "Generate Combined";
                        if (pVal.ItemUID == "btnComb" && selectedValue == "Cancel") actionName = "Cancel Combined";
                        
                        if (pVal.ItemUID == "btnEWay" && selectedValue == "Generate") actionName = "Generate E-Way Bill";
                        if (pVal.ItemUID == "btnEWay" && selectedValue == "Cancel") actionName = "Cancel E-Way Bill";
                        
                        if (pVal.ItemUID == "btnEInv" && selectedValue == "Generate") actionName = "Generate E-Invoice";
                        if (pVal.ItemUID == "btnEInv" && selectedValue == "Cancel") actionName = "Cancel E-Invoice";

                        if (string.IsNullOrEmpty(actionName)) return;

                        // Reusable Confirmation Helper
                        if (!ConfirmAction(actionName))
                        {
                            // Reset the combobox back to default (0 index) if user clicks No
                            combo.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
                            return;
                        }

                        // Retrieve the internal DocEntry
                        string objType = (pVal.FormTypeEx == "133") ? "13" : "14";
                        string tableName = (objType == "13") ? "OINV" : "ORIN";
                        string docEntry = oForm.DataSources.DBDataSources.Item(tableName).GetValue("DocEntry", 0).Trim();

                        if (string.IsNullOrEmpty(docEntry))
                        {
                            _connectionManager.SboApplication.MessageBox("Please Add the document first!");
                            combo.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
                            return;
                        }

                        if (actionName == "Generate Combined")
                        {
                            LoggerHelper.Log($"GIS ButtonCombo Selected! Initializing Combined Generation for DocEntry: {docEntry}...");
                            var dbHelper = new DatabaseHelper(_connectionManager.Company);
                            _connectionManager.SboApplication.StatusBar.SetText("Generating E-Invoice, please wait...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                            
                            string apiRes = CombinedEngine.ProcessCombined(dbHelper, objType, docEntry);
                            bool isSuccess = apiRes.Contains("\"Status\":\"1\"") || apiRes.Contains("\"Status\":1") || apiRes.Contains("\"status_cd\":\"1\"") || apiRes.Contains("DUPIRN");

                            if (apiRes.Contains("DUPIRN"))
                            {
                                _connectionManager.SboApplication.StatusBar.SetText("Duplicate IRN Detected! Successfully recovered previous IRN details.", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            }
                            else if (isSuccess && !apiRes.Contains("\"error\""))
                            {
                                _connectionManager.SboApplication.StatusBar.SetText("Combined Generated Successfully!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                            }
                            else
                            {
                                _connectionManager.SboApplication.StatusBar.SetText("Combined Failed. Check Comments on Document.", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            }
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
