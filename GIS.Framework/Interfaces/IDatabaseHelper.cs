using System.Data;

namespace GIS.Framework.Interfaces
{
    /// <summary>
    /// Abstracts database connectivity so GIS.Framework does not need SAPbobsCOM or direct HANA drivers.
    /// The consumer (your SAP Add-On or Console App) implements this interface.
    /// </summary>
    public interface IDatabaseHelper
    {
        /// <summary>
        /// Executes a SQL query and returns the results in a standard DataTable.
        /// </summary>
        DataTable ExecuteQuery(string query);

        /// <summary>
        /// Executes an action query (INSERT/UPDATE/DELETE) that doesn't return data.
        /// </summary>
        void ExecuteNonQuery(string query);
    }
}
