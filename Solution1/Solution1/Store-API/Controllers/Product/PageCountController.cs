using API.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
namespace Api.Controllers
{
    public class PageCountController : ApiController
    {
        DataAccess.StoreEntities db = new StoreEntities();


        [HttpGet]
        public List<Pagging> Get(int CompanyID, int ProductGroupID, string PageSize = null,
            string UserName = null, string Lang = null, int? ID = null, string BarCode = null, string Name = null,
            bool? IsAvailable = null, bool? Acive = null, bool? IsViewTelegram = null, bool? IsViewInstagram = null,
            bool? IsSendInstagram = null, bool? IsSendTelegram = null, bool? IsSpecialSales = null, string Specification = null)
        {
            int pageSize;
            int totalCount;

            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Lang))
            {
                pageSize = 50;
                int parsedPageSize;
                if (int.TryParse(PageSize, out parsedPageSize) && parsedPageSize > 0)
                {
                    pageSize = parsedPageSize;
                }

                totalCount = db.sp_Product_Panel_Select(UserName, Lang, ID, CompanyID, ProductGroupID,
                    Settings.SetNull(BarCode), Settings.SetNull(Name), IsAvailable, Acive, IsViewTelegram,
                    IsViewInstagram, IsSendInstagram, IsSendTelegram, IsSpecialSales, Settings.SetNull(Specification)).Count();
            }
            else
            {
                var settingPageCount = db.Settings.Where(a => a.SettingKey == "PageCount").Select(a => a.SettingValue).FirstOrDefault();
                pageSize = Convert.ToInt32(settingPageCount);
                totalCount = db.Products.Where(a => a.CompanyID == CompanyID && a.ProductGroupID == ProductGroupID && a.Acive == true).Count();
            }

            return BuildPaggingList(totalCount, pageSize);
        }

        private static List<Pagging> BuildPaggingList(int totalCount, int pageSize)
        {
            List<Pagging> list = new List<Pagging>();
            if (totalCount <= 0 || pageSize <= 0)
            {
                return list;
            }

            int pageCount = (int)Math.Ceiling((double)totalCount / pageSize);
            for (int i = 1; i <= pageCount; i++)
            {
                Pagging p = new Pagging();
                p.pageNumber = i;
                p.Active = i == 1;
                p.FirstPage = i == 1;
                p.LastPage = i == pageCount;
                list.Add(p);
            }
            return list;
        }

        public class Pagging
        {
            public int pageNumber { get; set; }
            public bool Active { get; set; }
            public bool LastPage { get; set; }
            public bool FirstPage { get; set; }
        }
    }
}
