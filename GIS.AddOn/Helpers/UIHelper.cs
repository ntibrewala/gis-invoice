using System;
using SAPbouiCOM;
using GIS.Framework.Helpers;

namespace GIS.AddOn.Helpers
{
    public static class UIHelper
    {
        public static void DrawLegacyButtonCombos(Form oForm)
        {
            try
            {
                oForm.Freeze(true);

                // Create btnEInv
                Item oItemEInv;
                ButtonCombo oComboEInv;
                try
                {
                    oItemEInv = oForm.Items.Item("btnEInv");
                    oComboEInv = (ButtonCombo)oItemEInv.Specific;
                }
                catch
                {
                    oItemEInv = oForm.Items.Add("btnEInv", BoFormItemTypes.it_BUTTON_COMBO);
                    // Legacy coordinate placement relative to "Copy To" button ("10000330")
                    oItemEInv.Left = oForm.Items.Item("10000330").Left - 110;
                    oItemEInv.Top = oForm.Items.Item("10000330").Top;
                    oItemEInv.Width = oForm.Items.Item("10000330").Width;
                    oItemEInv.Height = oForm.Items.Item("10000330").Height;
                    oItemEInv.LinkTo = "10000330";

                    oComboEInv = (ButtonCombo)oItemEInv.Specific;
                    oComboEInv.ValidValues.Add("E-Invoice", "E-Invoice");
                    oComboEInv.ValidValues.Add("Generate", "Generate E-Invoice");
                    oComboEInv.ValidValues.Add("Cancel", "Cancel E-Invoice");
                    oItemEInv.DisplayDesc = true;
                }
                oComboEInv.Caption = "E-Invoice";

                // Create btnEWay
                Item oItemEWay;
                ButtonCombo oComboEWay;
                try
                {
                    oItemEWay = oForm.Items.Item("btnEWay");
                    oComboEWay = (ButtonCombo)oItemEWay.Specific;
                }
                catch
                {
                    oItemEWay = oForm.Items.Add("btnEWay", BoFormItemTypes.it_BUTTON_COMBO);
                    oItemEWay.Left = oForm.Items.Item("btnEInv").Left - 110;
                    oItemEWay.Top = oForm.Items.Item("btnEInv").Top;
                    oItemEWay.Width = oForm.Items.Item("btnEInv").Width;
                    oItemEWay.Height = oForm.Items.Item("btnEInv").Height;
                    oItemEWay.LinkTo = "btnEInv";

                    oComboEWay = (ButtonCombo)oItemEWay.Specific;
                    oComboEWay.ValidValues.Add("EWayBill", "E-WayBill");
                    oComboEWay.ValidValues.Add("Generate", "Generate E-WayBill");
                    oComboEWay.ValidValues.Add("Cancel", "Cancel E-WayBill");
                    oComboEWay.ValidValues.Add("Inv Transfer", "Inventory Transfer");
                    oComboEWay.ValidValues.Add("UpdateVehicle", "Update Part B");
                    oComboEWay.ValidValues.Add("ExtendValidity", "Extend Validity");
                    oComboEWay.ValidValues.Add("UpdateTrans", "Update Transporter");
                    oItemEWay.DisplayDesc = true;
                }
                oComboEWay.Caption = "E-WayBill";

                // Create btnComb
                Item oItemComb;
                ButtonCombo oComboComb;
                try
                {
                    oItemComb = oForm.Items.Item("btnComb");
                    oComboComb = (ButtonCombo)oItemComb.Specific;
                }
                catch
                {
                    oItemComb = oForm.Items.Add("btnComb", BoFormItemTypes.it_BUTTON_COMBO);
                    oItemComb.Left = oForm.Items.Item("btnEWay").Left - 110;
                    oItemComb.Top = oForm.Items.Item("btnEWay").Top;
                    oItemComb.Width = oForm.Items.Item("btnEWay").Width;
                    oItemComb.Height = oForm.Items.Item("btnEWay").Height;
                    oItemComb.LinkTo = "btnEWay";

                    oComboComb = (ButtonCombo)oItemComb.Specific;
                    oComboComb.ValidValues.Add("Combined", "Combined");
                    oComboComb.ValidValues.Add("Generate", "Generate Combined");
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
    }
}
