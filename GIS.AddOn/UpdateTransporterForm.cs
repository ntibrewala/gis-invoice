using GIS.Framework;
using GIS.Framework.Helpers;
using Newtonsoft.Json.Linq;
using SAPbobsCOM;
using System;

namespace GIS.AddOn
{
    class UpdateTransporterForm
    {
        public static void LoadForm(SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany, string docEntry, string docType, string ewayNo)
        {
            try
            {
                SAPbouiCOM.FormCreationParams fcp = (SAPbouiCOM.FormCreationParams)oApplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);
                fcp.UniqueID = "frmUpdTrn" + DateTime.Now.ToString("mmss");
                fcp.FormType = "GIS_OUPT";
                fcp.BorderStyle = SAPbouiCOM.BoFormBorderStyle.fbs_Fixed;
                SAPbouiCOM.Form oForm = oApplication.Forms.AddEx(fcp);

                oForm.Title = "Update Transporter";
                oForm.Width = 400;
                oForm.Height = 150;

                // Hidden context fields
                SAPbouiCOM.Item itmDocE = oForm.Items.Add("txtDocE", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                itmDocE.Left = -100; itmDocE.Top = -100; itmDocE.Width = 10; itmDocE.Height = 10;
                itmDocE.Visible = false;
                ((SAPbouiCOM.EditText)itmDocE.Specific).Value = docEntry;

                SAPbouiCOM.Item itmDocT = oForm.Items.Add("txtDocT", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                itmDocT.Left = -100; itmDocT.Top = -100; itmDocT.Width = 10; itmDocT.Height = 10;
                itmDocT.Visible = false;
                ((SAPbouiCOM.EditText)itmDocT.Specific).Value = docType;

                SAPbouiCOM.Item itmEWay = oForm.Items.Add("txtEWay", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                itmEWay.Left = -100; itmEWay.Top = -100; itmEWay.Width = 10; itmEWay.Height = 10;
                itmEWay.Visible = false;
                ((SAPbouiCOM.EditText)itmEWay.Specific).Value = ewayNo;

                AddStatic(oForm, "lblEWay", 10, 10, 110, 14, "E-Way Bill No");
                AddStatic(oForm, "lblEwb", 125, 10, 200, 14, ewayNo);

                // Transporter Id
                AddStatic(oForm, "lblTrnId", 10, 30, 110, 14, "Transporter GSTIN");
                AddEdit(oForm, "txtTrnId", 125, 30, 190, 14);

                // Buttons
                AddButton(oForm, "btnSubmit", 120, 70, 80, 20, "Submit");
                AddButton(oForm, "btnCancel", 210, 70, 80, 20, "Cancel");

                oForm.Visible = true;
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
        }

        private static void AddStatic(SAPbouiCOM.Form f, string uid, int l, int t, int w, int h, string caption)
        {
            SAPbouiCOM.Item i = f.Items.Add(uid, SAPbouiCOM.BoFormItemTypes.it_STATIC);
            i.Left = l; i.Top = t; i.Width = w; i.Height = h;
            ((SAPbouiCOM.StaticText)i.Specific).Caption = caption;
        }

        private static void AddEdit(SAPbouiCOM.Form f, string uid, int l, int t, int w, int h)
        {
            SAPbouiCOM.Item i = f.Items.Add(uid, SAPbouiCOM.BoFormItemTypes.it_EDIT);
            i.Left = l; i.Top = t; i.Width = w; i.Height = h;
        }

        private static void AddButton(SAPbouiCOM.Form f, string uid, int l, int t, int w, int h, string caption)
        {
            SAPbouiCOM.Item i = f.Items.Add(uid, SAPbouiCOM.BoFormItemTypes.it_BUTTON);
            i.Left = l; i.Top = t; i.Width = w; i.Height = h;
            ((SAPbouiCOM.Button)i.Specific).Caption = caption;
        }

        public static void ItemEvent(ref SAPbouiCOM.Application oApplication, ref SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.BeforeAction == false)
            {
                if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED)
                {
                    if (pVal.ItemUID == "btnCancel")
                    {
                        oForm.Close();
                    }
                    else if (pVal.ItemUID == "btnSubmit")
                    {
                        SubmitData(oApplication, oCompany, oForm);
                    }
                }
            }
        }

        private static void SubmitData(SAPbouiCOM.Application oApplication, SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm)
        {
            try
            {
                oForm.ActiveItem = "btnCancel";

                oApplication.StatusBar.SetText("Processing Update Transporter...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                string ewayNo = ((SAPbouiCOM.EditText)oForm.Items.Item("txtEWay").Specific).Value;
                string trnId = ((SAPbouiCOM.EditText)oForm.Items.Item("txtTrnId").Specific).Value;
                string sourceDocEntry = ((SAPbouiCOM.EditText)oForm.Items.Item("txtDocE").Specific).Value;
                string sourceDocType = ((SAPbouiCOM.EditText)oForm.Items.Item("txtDocT").Specific).Value;

                if (string.IsNullOrEmpty(trnId))
                {
                    oApplication.StatusBar.SetText("Transporter GSTIN is required.", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    return;
                }

                string sLocQuery = "";
                if (sourceDocType == "Invoice")
                {
                    sLocQuery = "SELECT TOP 1 D.\"GSTRegnNo\", A4.\"GSTCode\" FROM INV1 A2 INNER JOIN OLCT D ON D.\"Code\"=A2.\"LocCode\" INNER JOIN OCST A4 ON A4.\"Code\"=D.\"State\" AND A4.\"Country\"=D.\"Country\" WHERE A2.\"DocEntry\" = " + sourceDocEntry;
                }
                else if (sourceDocType == "Transfer")
                {
                    sLocQuery = "SELECT TOP 1 D.\"GSTRegnNo\", A4.\"GSTCode\" FROM WTR1 A2 INNER JOIN OLCT D ON D.\"Code\"=A2.\"LocCode\" INNER JOIN OCST A4 ON A4.\"Code\"=D.\"State\" AND A4.\"Country\"=D.\"Country\" WHERE A2.\"DocEntry\" = " + sourceDocEntry;
                }
                else
                {
                    sLocQuery = "SELECT TOP 1 D.\"GSTRegnNo\", A4.\"GSTCode\" FROM RIN1 A2 INNER JOIN OLCT D ON D.\"Code\"=A2.\"LocCode\" INNER JOIN OCST A4 ON A4.\"Code\"=D.\"State\" AND A4.\"Country\"=D.\"Country\" WHERE A2.\"DocEntry\" = " + sourceDocEntry;
                }

                var dbHelper = new DatabaseHelper(oCompany);
                System.Data.DataTable dtLoc = dbHelper.ExecuteQuery(sLocQuery);
                if (dtLoc == null || dtLoc.Rows.Count == 0) return;

                string locGSTIN = dtLoc.Rows[0]["GSTRegnNo"].ToString().Trim();
                string locState = dtLoc.Rows[0]["GSTCode"].ToString().Trim();
                string objType = (sourceDocType == "Invoice") ? "13" : "14";

                var tokenResp = AuthHelper.GetCharteredAuthTokenSync(dbHelper, objType, sourceDocEntry);
                string sAuthToken = tokenResp.Data?.AuthToken;
                if (string.IsNullOrEmpty(sAuthToken)) return;

                var creds = CredentialManager.GetCredentialsForDocument(dbHelper, objType, sourceDocEntry);

                dynamic trnReq = new System.Dynamic.ExpandoObject();
                trnReq.ewbNo = Convert.ToInt64(ewayNo);
                trnReq.transporterId = trnId;
                
                string sJSON = Newtonsoft.Json.JsonConvert.SerializeObject(trnReq);

                string sUrlQ = "CALL \"TEC_EWAYLoginURL\"('UpdateTransporter','" + locState + "')";
                System.Data.DataTable oADT1 = dbHelper.ExecuteQuery(sUrlQ);
                string UpdURL = oADT1.Rows[0]["URL"].ToString().Trim();

                string XML_Final = ApiRunner.PostInvoice(sJSON, sAuthToken, creds.GSTIN, creds.UserName, creds.AspPassword, creds.AspId, UpdURL);

                string strQueryLog = oCompany.DbServerType == BoDataServerTypes.dst_HANADB
                    ? "INSERT INTO TEC_EW_LOG VALUES('" + sourceDocEntry + "','UpdateTrn','" + sJSON.Replace("'", "''") + "','" + XML_Final.Replace("'", "''") + "',CURRENT_TIMESTAMP,'" + oCompany.UserSignature + "')"
                    : "INSERT INTO TEC_EW_LOG VALUES('" + sourceDocEntry + "','UpdateTrn','" + sJSON.Replace("'", "''") + "','" + XML_Final.Replace("'", "''") + "',GETDATE(),'" + oCompany.UserSignature + "')";
                dbHelper.ExecuteNonQuery(strQueryLog);

                if (XML_Final.Contains("vehUpdDate") || XML_Final.Contains("\"status\":\"1\"") || XML_Final.ToLower().Contains("success"))
                {
                    oApplication.StatusBar.SetText("Transporter Updated Successfully!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    oForm.Close();
                }
                else
                {
                    oApplication.StatusBar.SetText("Error updating transporter: " + XML_Final, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                }
            }
            catch (Exception ex)
            {
                oApplication.StatusBar.SetText("Error: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
        }
    }
}
