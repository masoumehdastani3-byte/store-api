using API.Models;
using DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
namespace API.Controllers
{
    public class ProductSpecificationController : ApiController
    {
        StoreEntities db = new StoreEntities();

        [HttpGet]

        public List<sp_ProductSpecification_Select_Result> Get(string Lang, string UserName, int? GroupType, string GroupTypeName, int? companyID)
        {


            string WebSite = Settings.WebsiteName();
            if (companyID == null)
            {
                companyID = db.Companies.Where(a => a.WebsiteUrl == WebSite).Select(a => a.ID).FirstOrDefault();
            }
            if (GroupType == null)
            {
                var proclist = db.sp_CompanyProductGroup_Select(companyID, Lang);
                GroupType = proclist.Select(a => a.ID).FirstOrDefault();
            }

            int? ID = null;
            var list = db.sp_ProductSpecification_Select(Settings.SetNull(Lang), Settings.SetNull(UserName), ID, null, GroupType, companyID, Settings.SetNull(GroupTypeName)).ToList();
            return list;
        }
        [HttpGet]
        public List<sp_ProductSpecification_Select_Result> Get(string Lang, string UserName, int ID)
        {
            var list = db.sp_ProductSpecification_Select(Settings.SetNull(Lang), Settings.SetNull(UserName), ID, null, null, null, null).ToList();
            return list;
        }
        [HttpGet]
        public List<sp_ProductSpecification_Select_Result> Get(string Lang, string UserName)
        {
            var list = db.sp_ProductSpecification_Select(Settings.SetNull(Lang), Settings.SetNull(UserName), null, null, null, null, null).ToList();
            return list;
        }
    }
}
