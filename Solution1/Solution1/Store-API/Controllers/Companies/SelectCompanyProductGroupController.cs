using API.Models;
using DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class SelectCompanyProductGroupController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]
        public List<sp_CompanyProductGroup_Select_Result> Get(int? CompanyID, string Lang)
        {

            string WebSite = Settings.WebsiteName();

            if (CompanyID == null)
            {
                CompanyID = db.Companies.Where(a => a.WebsiteUrl == WebSite).Select(a => a.ID).FirstOrDefault();
            }
            var result = db.sp_CompanyProductGroup_Select(CompanyID, Lang).ToList();
            return result;
        } // List


    } // class
} //End namespace