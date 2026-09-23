using API.Models;
using DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class SelectProductGroupController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]
        public List<sp_ProductGroup_Select_Result> Get(string UserName, string Lang, string Title, string ParentTitle, int? ID, int? ParentID, bool? IsLastChid)
        {
            var result = db.sp_ProductGroup_Select(UserName, Lang, ID, ParentID, Settings.SetNull(ParentTitle), Settings.SetNull(Title), IsLastChid).ToList();
            return result;
        } // List
        [HttpGet]
        public List<sp_ProductGroup_Select_Result> Get(string Lang, string UserName, int ID)
        {
            var list = db.sp_ProductGroup_Select(UserName, Lang, ID, null, null, null, null).ToList();
            return list;
        }
    } // class
} //End namespace