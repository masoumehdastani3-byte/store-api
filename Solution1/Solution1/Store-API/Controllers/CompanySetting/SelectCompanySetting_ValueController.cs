using DataAccess;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class SelectCompanySetting_ValueController : ApiController
    {
        StoreEntities db = new StoreEntities();
        public string Get(int CompanyID, string SettingName)
        {
            string list = db.CompanySettings.Where(a => a.SettingName == SettingName && a.CompanyID == CompanyID).Select(a => a.SettingValue).FirstOrDefault();
            return list;
        }
    }
}
