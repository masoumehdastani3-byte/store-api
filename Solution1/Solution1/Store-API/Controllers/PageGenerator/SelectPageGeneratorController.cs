using API.Models;
using DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class SelectPageGeneratorController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]

        public List<sp_PageGenerator_Select_Result> Get(string Lang, string LogUser, int? CompanyID, int? ID, string PageContent, int? PageLocationID, bool? Active)
        {
            if (CompanyID == null)
            {
                string WebSite = Settings.WebsiteName();
                CompanyID = db.Companies.Where(a => a.WebsiteUrl == WebSite).Select(a => a.ID).FirstOrDefault();
            }
            var result = db.sp_PageGenerator_Select(Settings.SetNull(Lang), Settings.SetNull(LogUser), ID, CompanyID, PageLocationID, Settings.SetNull(PageContent), Active).ToList();

            return result;
        } // List
        [HttpGet]
        public List<sp_PageGenerator_Select_Result> Get(string Lang, string UserName, int ID)
        {
            var list = db.sp_PageGenerator_Select(Settings.SetNull(Lang), Settings.SetNull(UserName), ID, null, null, null, null).ToList();
            return list;
        }
        [HttpGet]
        public List<sp_PageGenerator_Select_Result> Get(int? CompanyID)
        {
            string WebSite = Settings.WebsiteName();
            CompanyID = db.Companies.Where(a => a.WebsiteUrl == WebSite).Select(a => a.ID).FirstOrDefault();
            var list = db.sp_PageGenerator_Select("", "", null, CompanyID, null, null, true).ToList();
            return list.ToList();
        }
    } // class
} //End namespace