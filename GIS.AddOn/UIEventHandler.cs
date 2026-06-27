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

        private void DrawLegacyButtonCombos(SAPbouiCOM.Form oForm)
        {
            try
            {
                oForm.Freeze(true);

                // Create btnEInv
                SAPbouiCOM.Item oItemEInv;
                SAPbouiCOM.ButtonCombo oComboEInv;
                try
                {
                    oItemEInv = oForm.Items.Item("btnEInv");
                    oComboEInv = (SAPbouiCOM.ButtonCombo)oItemEInv.Specific;
                }
                catch
                {
                    oItemEInv = oForm.Items.Add("btnEInv", SAPbouiCOM.BoFormItemTypes.it_BUTTON_COMBO);
                    // Legacy coordinate placement relative to "Copy To" button ("10000330")
                    oItemEInv.Left = oForm.Items.Item("10000330").Left - 110;
                    oItemEInv.Top = oForm.Items.Item("10000330").Top;
                    oItemEInv.Width = oForm.Items.Item("10000330").Width;
                    oItemEInv.Height = oForm.Items.Item("10000330").Height;
                    oItemEInv.LinkTo = "10000330";

                    oComboEInv = (SAPbouiCOM.ButtonCombo)oItemEInv.Specific;
                    oComboEInv.ValidValues.Add("E-Invoice", "E-Invoice");
                    oComboEInv.ValidValues.Add("Generate", "Generate E-Invoice");
                    oComboEInv.ValidValues.Add("Cancel", "Cancel E-Invoice");
                    oItemEInv.DisplayDesc = true;
                }
                oComboEInv.Caption = "E-Invoice";

                // Create btnEWay
                SAPbouiCOM.Item oItemEWay;
                SAPbouiCOM.ButtonCombo oComboEWay;
                try
                {
                    oItemEWay = oForm.Items.Item("btnEWay");
                    oComboEWay = (SAPbouiCOM.ButtonCombo)oItemEWay.Specific;
                }
                catch
                {
                    oItemEWay = oForm.Items.Add("btnEWay", SAPbouiCOM.BoFormItemTypes.it_BUTTON_COMBO);
                    oItemEWay.Left = oForm.Items.Item("btnEInv").Left - 110;
                    oItemEWay.Top = oForm.Items.Item("btnEInv").Top;
                    oItemEWay.Width = oForm.Items.Item("btnEInv").Width;
                    oItemEWay.Height = oForm.Items.Item("btnEInv").Height;
                    oItemEWay.LinkTo = "btnEInv";

                    oComboEWay = (SAPbouiCOM.ButtonCombo)oItemEWay.Specific;
                    oComboEWay.ValidValues.Add("EWayBill", "E-WayBill");
                    oComboEWay.ValidValues.Add("Generate", "Generate E-WayBill");
                    oComboEWay.ValidValues.Add("Cancel", "Cancel E-WayBill");
                    oItemEWay.DisplayDesc = true;
                }
                oComboEWay.Caption = "E-WayBill";

                // Create btnComb
                SAPbouiCOM.Item oItemComb;
                SAPbouiCOM.ButtonCombo oComboComb;
                try
                {
                    oItemComb = oForm.Items.Item("btnComb");
                    oComboComb = (SAPbouiCOM.ButtonCombo)oItemComb.Specific;
                }
                catch
                {
                    oItemComb = oForm.Items.Add("btnComb", SAPbouiCOM.BoFormItemTypes.it_BUTTON_COMBO);
                    oItemComb.Left = oForm.Items.Item("btnEWay").Left - 110;
                    oItemComb.Top = oForm.Items.Item("btnEWay").Top;
                    oItemComb.Width = oForm.Items.Item("btnEWay").Width;
                    oItemComb.Height = oForm.Items.Item("btnEWay").Height;
                    oItemComb.LinkTo = "btnEWay";

                    oComboComb = (SAPbouiCOM.ButtonCombo)oItemComb.Specific;
                    oComboComb.ValidValues.Add("Combined", "Combined");
                    oComboComb.ValidValues.Add("Generate", "Generate Combined");
                    oComboComb.ValidValues.Add("Cancel", "Cancel Combined");
                    oItemComb.DisplayDesc = true;
                }
                oComboComb.Caption = "Combined";

                oForm.Freeze(false);
            }
            catch (Exception ex)
            {
                if (oForm != null) oForm.Freeze(false);
                LoggerHelper.Log($"Error drawing UI: {ex.Message}");
            }
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
                    DrawLegacyButtonCombos(oForm);
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
