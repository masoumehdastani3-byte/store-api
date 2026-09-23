using API.Models;
using DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class SelectCompanyTransportationController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]
        public List<sp_CompanyTransportation_Select_Result> Get(string Lang, string UserName, int? CompanyID, int? TransportationCalculate_BasicData)
        {
            var result = db.sp_CompanyTransportation_Select(Lang, UserName, CompanyID, TransportationCalculate_BasicData, null).ToList();
            return result;
        } // List
        [HttpGet]
        public List<sp_CompanyTransportation_Select_Result> Get(string Lang, string UserName, int ID)
        {
            var list = db.sp_CompanyTransportation_Select(Settings.SetNull(Lang), Settings.SetNull(UserName), null, null, ID).ToList();
            return list;
        }
    } // class
} //End namespace