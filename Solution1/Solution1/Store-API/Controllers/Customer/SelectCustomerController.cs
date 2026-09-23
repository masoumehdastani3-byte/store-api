using API.Models;
using DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class SelectCustomerController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]

        public List<sp_Customer_Select_Result> Get(string Lang, string UserName, int? ID, int? CompanyID, string Mobile, string Email, string FullName, string CodeMelli)
        {
            var result = db.sp_Customer_Select(Settings.SetNull(Lang), Settings.SetNull(UserName), ID, CompanyID, Settings.SetNull(Mobile), Settings.SetNull(Email), Settings.SetNull(FullName), Settings.SetNull(CodeMelli)).ToList();
            return result;
        } // List
        [HttpGet]
        public List<sp_Customer_Select_Result> Get(string Lang, string UserName, int ID)
        {
            var list = db.sp_Customer_Select(Settings.SetNull(Lang), Settings.SetNull(UserName), ID, null, null, null, null, null).ToList();
            return list;
        }
    } // class
} //End namespace