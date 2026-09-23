using API.Models;
using DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
namespace API.Controllers.UserRole
{
    public class SelectUserRoleController : ApiController
    {
        StoreEntities db = new StoreEntities();
        [HttpGet]

        public List<sp_UserRole_Select_Result> Get(string Lang, string UserName, string RoleName, bool Active, int? ParentLangID)
        {
            int? ID = null;
            var list = db.sp_UserRole_Select(Settings.SetNull(Lang), Settings.SetNull(UserName), ID, Settings.SetNull(RoleName), ParentLangID, Active).ToList();
            return list;
        }
        [HttpGet]
        public List<sp_UserRole_Select_Result> Get(string Lang, string UserName, int? ID)
        {
            var list = db.sp_UserRole_Select(Lang, UserName, ID, null, null, null).ToList();
            return list;
        }
        [HttpGet]
        public List<sp_UserRole_Select_Result> Get(string Lang, string UserName)
        {
            var list = db.sp_UserRole_Select(Settings.SetNull(Lang), Settings.SetNull(UserName), null, null, null, null).ToList();
            return list;
        }
    }
}
