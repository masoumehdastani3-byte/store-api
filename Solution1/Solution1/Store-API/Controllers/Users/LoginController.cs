using DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
namespace API.Controllers
{

    public class LoginController : ApiController
    {

        StoreEntities db = new StoreEntities();
        [HttpGet]
        public List<sp_UserProfile_Login_Result> Get(string Lang, string UserName, string Password)
        {
            var list = db.sp_UserProfile_Login(Lang, UserName, Password).ToList();
            return list;
        }

    }

}
