using GIS.Framework.Interfaces;

namespace GIS.Framework.Helpers
{
    internal interface IResponseHandler
    {
        void Handle(IDatabaseHelper dbHelper, string objType, string docEntry, string rawJsonRequest, string rawJsonResponse);
    }
}
