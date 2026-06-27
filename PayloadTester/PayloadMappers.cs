using System;
using System.Collections.Generic;
using System.Data;

namespace GIS.Framework.Models.EInvoice
{
    internal static class PayloadMappers
    {
        private static decimal GetDecimal(DataRow row, string colName)
        {
            if (row.Table.Columns.Contains(colName) && row[colName] != DBNull.Value && decimal.TryParse(row[colName].ToString(), out decimal val))
            {
                return Math.Round(val, 2, MidpointRounding.AwayFromZero);
            }
            return 0m;
        }

        private static string GetString(DataRow row, string colName)
        {
            if (row.Table.Columns.Contains(colName) && row[colName] != DBNull.Value)
            {
                return row[colName].ToString().Trim();
            }
            return "";
        }

        public static EInvoicePayload MapHeader(DataRow headerRow)
        {
            string ecm = GetString(headerRow, "EcmGstin");
            var payload = new EInvoicePayload
            {
                Version = "1.1",
                TransactionDetails = new TransactionDetails
                {
                    TaxScheme = "GST",
                    SupplyType = GetString(headerRow, "SupTyp") != "" ? GetString(headerRow, "SupTyp") : "B2B",
                    ReverseCharge = GetString(headerRow, "RegRev") != "" ? GetString(headerRow, "RegRev") : "N",
                    ECommerceGstin = string.IsNullOrEmpty(ecm) ? null : ecm,
                    IgstOnIntra = GetString(headerRow, "IgstOnInt") != "" ? GetString(headerRow, "IgstOnInt") : "N"
                },
                DocumentDetails = new DocumentDetails
                {
                    Type = GetString(headerRow, "Type") != "" ? GetString(headerRow, "Type") : "INV",
                    Number = GetString(headerRow, "DocNum"),
                    Date = GetString(headerRow, "Dt")
                },
                SellerDetails = new PartyDetails
                {
                    Gstin = GetString(headerRow, "Seller_GSTIN"),
                    LegalName = GetString(headerRow, "Seller_Trade_Name"),
                    TradeName = GetString(headerRow, "Seller_Trade_Name"),
                    Address1 = GetString(headerRow, "Seller_Addr1"),
                    Address2 = GetString(headerRow, "Seller_Addr2"),
                    Location = GetString(headerRow, "Seller_Loc"),
                    Pincode = GetString(headerRow, "Seller_Pin") != "" ? Convert.ToInt32(GetString(headerRow, "Seller_Pin")) : 0,
                    StateCode_Address = GetString(headerRow, "Seller_State"),
                    Phone = GetString(headerRow, "Seller_Ph"),
                    Email = GetString(headerRow, "Seller_Em")
                },
                BuyerDetails = new PartyDetails
                {
                    Gstin = GetString(headerRow, "Buyer_GSTIN"),
                    LegalName = GetString(headerRow, "Buyer_LglNm"),
                    TradeName = GetString(headerRow, "Buyer_LglNm"),
                    StateCode = GetString(headerRow, "Buyer_Pos"),
                    Address1 = GetString(headerRow, "Buyer_Addr1"),
                    Address2 = GetString(headerRow, "Buyer_Addr2"),
                    Location = GetString(headerRow, "Buyer_Loc"),
                    Pincode = GetString(headerRow, "Buyer_Pin") != "" ? Convert.ToInt32(GetString(headerRow, "Buyer_Pin")) : 0,
                    StateCode_Address = GetString(headerRow, "Buyer_State"),
                    Phone = GetString(headerRow, "Buyer_Ph"),
                    Email = GetString(headerRow, "Buyer_Em")
                },
                ValueDetails = new ValueDetails
                {
                    AssessableValue = GetDecimal(headerRow, "AssVal"),
                    CgstValue = GetDecimal(headerRow, "CgstVal"),
                    SgstValue = GetDecimal(headerRow, "SgstVal"),
                    IgstValue = GetDecimal(headerRow, "IgstVal"),
                    CessValue = GetDecimal(headerRow, "CesVal"),
                    StateCessValue = GetDecimal(headerRow, "StCesVal"),
                    Discount = GetDecimal(headerRow, "DiscSum"),
                    OtherCharges = GetDecimal(headerRow, "OthChrg"),
                    RoundOffAmount = GetDecimal(headerRow, "RndOffAmt"),
                    TotalInvoiceValue = GetDecimal(headerRow, "TotInvVal")
                }
            };

            return payload;
        }

        public static List<ItemDetails> MapDetails(DataTable detailsTable)
        {
            var items = new List<ItemDetails>();
            foreach (DataRow row in detailsTable.Rows)
            {
                items.Add(new ItemDetails
                {
                    SlNo = GetString(row, "LineNum"),
                    ProductDescription = GetString(row, "PrdDesc"),
                    IsService = GetString(row, "IsServc") != "" ? GetString(row, "IsServc") : "N",
                    HsnCode = GetString(row, "HsnCd"),
                    Quantity = GetDecimal(row, "Qty"),
                    Unit = GetString(row, "Unit"),
                    UnitPrice = GetDecimal(row, "UnitPrice"),
                    TotalAmount = GetDecimal(row, "TotAmt"),
                    Discount = GetDecimal(row, "Discount"),
                    AssessableAmount = GetDecimal(row, "AssAmt"),
                    GstRate = GetDecimal(row, "GstRt"),
                    IgstAmount = GetDecimal(row, "IgstAmt"),
                    CgstAmount = GetDecimal(row, "CgstAmt"),
                    SgstAmount = GetDecimal(row, "SgstAmt"),
                    CessRate = GetDecimal(row, "CesRt"),
                    CessAmount = GetDecimal(row, "CesAmt"),
                    CessNonAdvolAmount = GetDecimal(row, "CesNonAdvlAmt"),
                    StateCessRate = GetDecimal(row, "StateCesRt"),
                    StateCessAmount = GetDecimal(row, "StateCesAmt"),
                    StateCessNonAdvolAmount = GetDecimal(row, "StateCesNonAdvlAmt"),
                    OtherCharges = GetDecimal(row, "OthChrg"),
                    TotalItemValue = GetDecimal(row, "TotItemVal")
                });
            }
            return items;
        }
    }
}
