using System;
using System.Data;
using Newtonsoft.Json;
using GIS.Framework.Models.EInvoice;

namespace PayloadTester
{
    class Program
    {
        static void Main(string[] args)
        {
            DataTable headerTable = new DataTable();
            headerTable.Columns.Add("ObjType");
            headerTable.Columns.Add("Version");
            headerTable.Columns.Add("TaxSch");
            headerTable.Columns.Add("RegRev");
            headerTable.Columns.Add("IgstOnIntra");
            headerTable.Columns.Add("DocEntry");
            headerTable.Columns.Add("SupTyp");
            headerTable.Columns.Add("Type");
            headerTable.Columns.Add("DocNum");
            headerTable.Columns.Add("Dt");
            headerTable.Columns.Add("Seller_GSTIN");
            headerTable.Columns.Add("Seller_Trade_Name");
            headerTable.Columns.Add("Seller_Addr1");
            headerTable.Columns.Add("Seller_Addr2");
            headerTable.Columns.Add("Seller_Loc");
            headerTable.Columns.Add("Seller_Pin");
            headerTable.Columns.Add("Seller_State");
            headerTable.Columns.Add("Seller_Ph");
            headerTable.Columns.Add("Seller_Em");
            headerTable.Columns.Add("Buyer_GSTIN");
            headerTable.Columns.Add("Buyer_LglNm");
            headerTable.Columns.Add("Buyer_TrdNm");
            headerTable.Columns.Add("Buyer_Pos");
            headerTable.Columns.Add("Buyer_Addr1");
            headerTable.Columns.Add("Buyer_Addr2");
            headerTable.Columns.Add("Buyer_Loc");
            headerTable.Columns.Add("Buyer_Pin");
            headerTable.Columns.Add("Buyer_State");
            headerTable.Columns.Add("Buyer_Ph");
            headerTable.Columns.Add("Buyer_Em");
            headerTable.Columns.Add("AssVal");
            headerTable.Columns.Add("SgstVal");
            headerTable.Columns.Add("CgstVal");
            headerTable.Columns.Add("IgstVal");
            headerTable.Columns.Add("CesVal");
            headerTable.Columns.Add("StCesVal");
            headerTable.Columns.Add("RndOffAmt");
            headerTable.Columns.Add("TotInvVal");
            headerTable.Columns.Add("TCSVal");
            headerTable.Columns.Add("DiscSum");

            DataRow hRow = headerTable.NewRow();
            hRow["ObjType"] = "13";
            hRow["Version"] = "1.1";
            hRow["TaxSch"] = "GST";
            hRow["RegRev"] = "N";
            hRow["IgstOnIntra"] = "N";
            hRow["DocEntry"] = "10815";
            hRow["SupTyp"] = "B2B";
            hRow["Type"] = "INV";
            hRow["DocNum"] = "GUJ-26/178";
            hRow["Dt"] = "26/06/2026";
            hRow["Seller_GSTIN"] = "34AACCC1596Q002";
            hRow["Seller_Trade_Name"] = "TEST_BHAVYA_TEST8";
            hRow["Seller_Addr1"] = "At - Aslali, Ta- Dascroi,";
            hRow["Seller_Addr2"] = "Godown No.14";
            hRow["Seller_Loc"] = "Ahmedabad";
            hRow["Seller_Pin"] = "605001";
            hRow["Seller_State"] = "34";
            hRow["Seller_Ph"] = "9000310598";
            hRow["Seller_Em"] = "info@bhavyapolyfilms.in";
            hRow["Buyer_GSTIN"] = "37ABEFA9072D1ZH";
            hRow["Buyer_LglNm"] = "AMARAVATHI POLY PACK";
            hRow["Buyer_TrdNm"] = "AMARAVATHI POLY PACK";
            hRow["Buyer_Pos"] = "37";
            hRow["Buyer_Addr1"] = "25-38";
            hRow["Buyer_Addr2"] = "4TH CROSS KABELA ROAD";
            hRow["Buyer_Loc"] = "VIJAYWADA";
            hRow["Buyer_Pin"] = "520012";
            hRow["Buyer_State"] = "37";
            hRow["Buyer_Ph"] = "9491545178";
            hRow["Buyer_Em"] = "test1@gmail.com";
            hRow["AssVal"] = "122.00";
            hRow["SgstVal"] = "0.00";
            hRow["CgstVal"] = "0.00";
            hRow["IgstVal"] = "21.96";
            hRow["CesVal"] = "0.00";
            hRow["StCesVal"] = "0.00";
            hRow["RndOffAmt"] = "0.04";
            hRow["TotInvVal"] = "144.00";
            hRow["TCSVal"] = "0";
            hRow["DiscSum"] = "0.000000";
            headerTable.Rows.Add(hRow);

            DataTable detailsTable = new DataTable();
            detailsTable.Columns.Add("LineNum");
            detailsTable.Columns.Add("PrdDesc");
            detailsTable.Columns.Add("IsServc");
            detailsTable.Columns.Add("HsnCd");
            detailsTable.Columns.Add("Qty");
            detailsTable.Columns.Add("Unit");
            detailsTable.Columns.Add("UnitPrice");
            detailsTable.Columns.Add("TotAmt");
            detailsTable.Columns.Add("Discount");
            detailsTable.Columns.Add("AssAmt");
            detailsTable.Columns.Add("GstRt");
            detailsTable.Columns.Add("IgstAmt");
            detailsTable.Columns.Add("CgstAmt");
            detailsTable.Columns.Add("SgstAmt");
            detailsTable.Columns.Add("CesRt");
            detailsTable.Columns.Add("CesAmt");
            detailsTable.Columns.Add("CesNonAdvlAmt");
            detailsTable.Columns.Add("StateCesRt");
            detailsTable.Columns.Add("StateCesAmt");
            detailsTable.Columns.Add("StateCesNonAdvlAmt");
            detailsTable.Columns.Add("OthChrg");
            detailsTable.Columns.Add("TotItemVal");

            DataRow dRow = detailsTable.NewRow();
            dRow["LineNum"] = "0";
            dRow["PrdDesc"] = "PBAT (COMPOSTABLE POLYESTERS) KB100NC901";
            dRow["IsServc"] = "N";
            dRow["HsnCd"] = "39079900";
            dRow["Qty"] = "1.00";
            dRow["Unit"] = "NOS";
            dRow["UnitPrice"] = "122";
            dRow["TotAmt"] = "122.00";
            dRow["Discount"] = "0";
            dRow["AssAmt"] = "122.00";
            dRow["GstRt"] = "18.00";
            dRow["IgstAmt"] = "21.96";
            dRow["CgstAmt"] = "0.000";
            dRow["SgstAmt"] = "0.000";
            dRow["CesRt"] = "0.00";
            dRow["CesAmt"] = "0.000";
            dRow["CesNonAdvlAmt"] = "0.00";
            dRow["StateCesRt"] = "0.00";
            dRow["StateCesAmt"] = "0.00";
            dRow["StateCesNonAdvlAmt"] = "0.00";
            dRow["OthChrg"] = "0.00";
            dRow["TotItemVal"] = "143.96";
            detailsTable.Rows.Add(dRow);

            var payload = PayloadMappers.MapHeader(hRow);
            payload.ItemList = PayloadMappers.MapDetails(detailsTable);

            string json = JsonConvert.SerializeObject(payload, Formatting.Indented);
            System.IO.File.WriteAllText("payload.json", json);
            Console.WriteLine("PAYLOAD_GENERATED_SUCCESSFULLY");
        }
    }
}
