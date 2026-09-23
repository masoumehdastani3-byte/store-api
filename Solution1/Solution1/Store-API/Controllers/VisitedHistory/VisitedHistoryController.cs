using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class VisitedHistoryController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();

        [HttpGet]
        public List<sp_VisitedHistory_GroupBy_Select_Result> Get(int CompanyID, int durationDay)
        {
            DateTime to = DateTime.Now;
            DateTime from = DateTime.Now.AddDays(-1 * durationDay);
            var result = db.sp_VisitedHistory_GroupBy_Select(from, to, CompanyID).ToList();
            return result;
        } // List

    } // class
} //End namespace