using DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class SelectPageMenuController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]
        public List<sp_PageMenu_Select_Result> Get(string Lang, string UserName, int? CompanyID)
        {
            var result = db.sp_PageMenu_Select(Lang, UserName, CompanyID).ToList();
            return result;
        } // List

    } // class
} //End namespace