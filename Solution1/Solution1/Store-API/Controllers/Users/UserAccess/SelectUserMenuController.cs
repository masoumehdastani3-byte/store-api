using API.Models;
using DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
namespace API.Controllers.Users.UserAccess
{
    public class SelectUserMenuController : ApiController
    {
        StoreEntities db = new StoreEntities();

        [HttpGet]

        public List<sp_UserMenu_Select_Result> Get(string Lang, string UserName, string Title, bool? Active, bool? IsShow)
        {
            int? ID = null;
            int? CompanyID = db.UserProfiles.Where(a => a.UserName == UserName).Select(a => a.CompanyID).FirstOrDefault();

            var list = db.sp_UserMenu_Select(Settings.SetNull(Lang), Settings.SetNull(UserName), ID, CompanyID, Settings.SetNull(Title), Active, IsShow).ToList();
            return list;
        }
        [HttpGet]
        public List<sp_UserMenu_Select_Result> Get(string Lang, string UserName, int ID)
        {


            var list = db.sp_UserMenu_Select(Settings.SetNull(Lang), Settings.SetNull(UserName), ID, null, null, null, null).ToList();
            return list;
        }
    }
}
