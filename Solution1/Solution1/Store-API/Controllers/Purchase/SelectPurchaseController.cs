using API.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class SelectPurchaseController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]
        //Lang=fa&UserName=&ID=null&CompanyID=1&CustomerID=6&FromDate=null&ToDate=null&FullName=null&Status=null
        public List<sp_Purchase_Select_Result> Get(string Lang, string UserName, int? ID, int? CompanyID, int? CustomerID, DateTime? FromDate, DateTime? ToDate, string FullName, int? Status)
        {
            var result = db.sp_Purchase_Select(Settings.SetNull(Lang), Settings.SetNull(UserName), ID, CompanyID, CustomerID, FromDate, ToDate, Settings.SetNull(FullName), Status).ToList();
            return result.OrderByDescending(a => a.ID).ToList();
        } // List
        [HttpGet]
        public List<sp_Purchase_Select_Result> Get(string Lang, string UserName, int ID)
        {
            var list = db.sp_Purchase_Select(Settings.SetNull(Lang), Settings.SetNull(UserName), ID, null, null, null, null, null, null).ToList();
            return list;
        }
    } // class
} //End namespace