using GIS.Framework.Interfaces;
using GIS.Framework.Helpers;
using System;
using System.Data;
using System.Runtime.InteropServices;

namespace GIS.AddOn
{
    public class DatabaseHelper : IDatabaseHelper
    {
        private SAPbobsCOM.Company _company;

        public DatabaseHelper(SAPbobsCOM.Company company)
        {
            _company = company;
        }

        public DataTable ExecuteQuery(string query)
        {
            DataTable dt = new DataTable();
            SAPbobsCOM.Recordset recordset = null;
            try
            {
                recordset = (SAPbobsCOM.Recordset)_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                recordset.DoQuery(query);

                // Create columns
                for (int i = 0; i < recordset.Fields.Count; i++)
                {
                    dt.Columns.Add(recordset.Fields.Item(i).Name);
                }

                // Populate rows
                while (!recordset.EoF)
                {
                    DataRow row = dt.NewRow();
                    for (int i = 0; i < recordset.Fields.Count; i++)
                    {
                        row[i] = recordset.Fields.Item(i).Value;
                    }
                    dt.Rows.Add(row);
                    recordset.MoveNext();
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Log("Database Query Error: " + ex.Message);
            }
            finally
            {
                if (recordset != null)
                {
                    Marshal.ReleaseComObject(recordset);
                }
            }
            return dt;
        }

        public void ExecuteNonQuery(string query)
        {
            SAPbobsCOM.Recordset recordset = null;
            try
            {
                recordset = (SAPbobsCOM.Recordset)_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                recordset.DoQuery(query);
            }
            catch (Exception ex)
            {
                LoggerHelper.Log("Database NonQuery Error: " + ex.Message);
            }
            finally
            {
                if (recordset != null)
                {
                    Marshal.ReleaseComObject(recordset);
                }
            }
        }

        public void UpdateDocumentQRCode(string objType, string docEntry, string encryptedQRCode)
        {
            try
            {
                SAPbobsCOM.Documents odocument = null;
                if (objType == "19")
                {
                    odocument = (SAPbobsCOM.Documents)_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes);
                }
                else if (objType == "14")
                {
                    odocument = (SAPbobsCOM.Documents)_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes);
                }
                else if (objType == "13")
                {
                    odocument = (SAPbobsCOM.Documents)_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                }

                if (odocument != null && odocument.GetByKey(Convert.ToInt32(docEntry)))
                {
                    // SAP native QR code generation
                    odocument.CreateQRCodeFrom = encryptedQRCode;
                    int result = odocument.Update();
                    
                    if (result != 0)
                    {
                        LoggerHelper.Log($"Failed to update SAP QR Code. Error: {_company.GetLastErrorDescription()}");
                    }
                    else
                    {
                        LoggerHelper.Log("Successfully bound QR Code to SAP Document via DI API.");
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Log("DI API QR Code Error: " + ex.Message);
            }
        }
    }
}