using API.Models;
using DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
namespace API.Controllers.Users.UserAccess
{
    public class SelectUserNotAcessMenuController : ApiController
    {
        StoreEntities db = new StoreEntities();

        //Lang=fa&UserName=4&Title=&ControlID=companies&Active=true&IsShow=null&UserID=1

        [HttpGet]
        public List<sp_UserNotAcessMenu_Select_Result> Get(string Lang, string UserName, int RoleID, string ControlID, bool? Active, bool? IsShow)
        {

            var list = db.sp_UserNotAcessMenu_Select(Settings.SetNull(Lang), Settings.SetNull(UserName), RoleID, Settings.SetNull(ControlID), Active, IsShow).ToList();
            return list;
        }
        [HttpGet]
        public List<sp_UserNotAcessMenu_Select_Result> Get(string Lang, string UserName, int RoleID)
        {

            var list = db.sp_UserNotAcessMenu_Select(Settings.SetNull(Lang), Settings.SetNull(UserName), RoleID, null, null, null).ToList();
            return list;
        }
    }
}
