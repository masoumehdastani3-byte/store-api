using API.Models;
using DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
namespace API.Controllers
{

    public class ParamShowSearchController : ApiController
    {
        DataAccess.StoreEntities db = new StoreEntities();
        Models.Log log = new Models.Log();
        [HttpGet]
        public List<DataAccess.sp_ProductDatail_Select_Result> Get(string Lang, string UserName, string IP, int ProductGroupID, bool? IsShowSearch)
        {

            var list = db.sp_ProductDatail_Select(Lang, UserName, IP, ProductGroupID, IsShowSearch).ToList();
            return list;
        }
        public List<DataAccess.sp_ProductDatail_Select_Result> Get(string Lang, string UserName, string IP, string WebSite)
        {
            WebSite = Settings.WebsiteName();
            var companyID = db.Companies.Where(a => a.WebsiteUrl == WebSite).Select(a => a.ID).FirstOrDefault();

            var proclist = db.sp_CompanyProductGroup_Select(companyID, Lang);
            int? procID = proclist.Select(a => a.ID).FirstOrDefault();

            var list = db.sp_ProductDatail_Select(Lang, UserName, IP, procID, true).ToList();
            return list;
        }
    }
}
