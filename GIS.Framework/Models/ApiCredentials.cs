using System;

namespace GIS.Framework.Models
{
    public class ApiCredentials
    {
        public string GSTIN { get; set; }
        public string AspId { get; set; }
        public string AspPassword { get; set; }
        public string UserName { get; set; }
        public string ApiPassword { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        // Token Info (Populated if a valid token exists in the DB)
        public string ExistingAuthToken { get; set; }
        public DateTime? ExistingTokenExpiry { get; set; }
    }
}
