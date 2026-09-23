using DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class SelectProductGroupDetailController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]
        //        @UserName As varchar(150)='',
        //@Lang As Varchar(10)='',
        //@ID As INT=NULL,
        //@ProductID AS INT=NULL,
        //@BasicDataID AS INT=NULL


        public List<sp_ProductGroupDetail_Select_Result> Get(string UserName, string Lang, int? ID, int? ProductGroupID, int? CompanyID)
        {
            var result = db.sp_ProductGroupDetail_Select(UserName, Lang, ID, ProductGroupID, CompanyID).ToList();
            return result;

        }

        [HttpGet]
        public List<sp_ProductGroupDetail_Select_Result> Get(string Lang, string UserName, int ID)
        {
            var list = db.sp_ProductGroupDetail_Select(UserName, Lang, ID, null, null).ToList();
            return list;
        }
    } // class
} //End namespace