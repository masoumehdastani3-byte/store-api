using API.Models;
using DataAccess;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class SelectProductController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]


        public List<sp_Product_Panel_Select_Result> Get(string UserName, string Lang, int? ID, int? CompanyID, int? ProductGroupID, string BarCode, string Name,
           bool? IsAvailable, bool? Acive, bool? IsViewTelegram, bool? IsViewInstagram, bool? IsSendInstagram, bool? IsSendTelegram, bool? IsSpecialSales, string Specification, string PageNumber  = "1", string PageSize  = "50")
        {
            var result = db.sp_Product_Panel_Select(UserName, Lang, ID, CompanyID, ProductGroupID
                , Settings.SetNull(BarCode), Settings.SetNull(Name), IsAvailable, Acive, IsViewTelegram,
                IsViewInstagram, IsSendInstagram, IsSendTelegram, IsSpecialSales, Settings.SetNull(Specification)).ToList();

            int pageNumber = 1;
            int pageSize = 50;
            int parsedPageNumber;
            int parsedPageSize;
            if (int.TryParse(PageNumber, out parsedPageNumber) && parsedPageNumber > 0)
            {
                pageNumber = parsedPageNumber;
            }
            if (int.TryParse(PageSize, out parsedPageSize) && parsedPageSize > 0)
            {
                pageSize = parsedPageSize;
            }

            int skip = (pageNumber - 1) * pageSize;
            return result.Skip(skip).Take(pageSize).ToList();
        } // List
        [HttpGet]
        public List<sp_Product_Panel_Select_Result> Get(string Lang, string UserName, int ID)
        {
            var list = db.sp_Product_Panel_Select(UserName, Lang, ID, null, null, null, null, null, null, null, null, null, null, null, null).ToList();
            return list;
        }
    } // class
} //End namespace