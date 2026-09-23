using API.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class SelectPaymentController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]
        public List<sp_Payment_Select_Result> Get(string Lang, string UserName, int? ID, int? CompanyID, int? PurchaseID, string FullName, int? Status, DateTime? FromDate, DateTime? ToDate)
        {
            var result = db.sp_Payment_Select(Lang, UserName, ID, CompanyID, PurchaseID, FromDate, ToDate, FullName, Status).ToList();
            return result;
        } // List
        [HttpGet]
        public List<sp_Payment_Select_Result> Get(string Lang, string UserName, int ID)
        {
            var list = db.sp_Payment_Select(Settings.SetNull(Lang), Settings.SetNull(UserName), ID, null, null, null, null, null, null).ToList();
            return list;
        }
    } // class
} //End namespace