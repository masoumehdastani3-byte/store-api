using API.Models;
using DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
namespace API.Controllers
{

    public class UserProfileController : ApiController
    {
        StoreEntities db = new StoreEntities();

        [HttpGet]
        public List<sp_UserProfile_Select_Result> Get(string Lang, string UserName, string UserID, string Name, string Family, int? CompanyID, int? RoleID, string Mobile, string TokenID, string Email, string NationalCode)
        {
            int? ID = null;

            var list = db.sp_UserProfile_Select(Settings.SetNull(Lang), Settings.SetNull(UserName), Settings.SetNull(UserID), ID, CompanyID, Settings.SetNull(Name), Settings.SetNull(Family), RoleID, Settings.SetNull(Mobile), Settings.SetNull(TokenID), Settings.SetNull(Email), Settings.SetNull(NationalCode), null).ToList();
            return list;
        }
        [HttpGet]
        public List<sp_UserProfile_Select_Result> Get(string Lang, string UserName, int? ID, bool? isDefault)
        {

            var list = db.sp_UserProfile_Select(Lang, UserName, null, ID, null, null, null, null, null, null, null, null, isDefault).ToList();
            return list;
        }
        [HttpGet]
        public List<sp_UserProfile_Select_Result> Get(string Lang, string UserName, int? ID)
        {

            var list = db.sp_UserProfile_Select(Lang, UserName, null, ID, null, null, null, null, null, null, null, null, null).ToList();
            return list;
        }
        //[HttpGet]
        //public List<sp_UserProfile_Select_Result> Get(string Lang, string UserName)
        //{

        //    var list = db.sp_UserProfile_Select(Settings.SetNull(Lang), Settings.SetNull(UserName), null, null, null, null, null, null, null, null, null,isDefault).ToList();
        //    return list;
        //}
    }
}
