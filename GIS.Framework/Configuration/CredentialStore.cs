using System.Collections.Generic;

namespace GIS.Framework.Configuration
{
    /// <summary>
    /// Phase 1 static credential store.
    /// In future phases, this will read from the SAP B1 database or config files.
    /// </summary>
    public static class CredentialStore
    {
        public static Dictionary<string, string> GetCharteredAuthCredentials()
        {
            return new Dictionary<string, string>
            {
                { "aspid", "1805053626" },
                { "password", "Bhavya2026@" },
                { "gstin", "34AACCC1596Q002" },
                { "user_name", "TaxProEnvPON" },
                { "eInvPwd", "abc34*" }
            };
        }
    }
}
