using API.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace API.Controllers.Order
{
    public class productsController : ApiController
    {
        DataAccess.StoreEntities db = new StoreEntities();
        [HttpPost]
        public productsList post()
        {
            productsList product = new productsList();
            Models.Log log = new Models.Log();
            try
            {
                string WebSite = HttpContext.Current.Request.Url.Host;

                log.WriteErrorLog(" WebSite ProducList :" + WebSite);


                //var comp  = db.Companies.Where(a => WebSite.Contains(a.WebsiteUrl)).FirstOrDefault();
                var comp = db.Companies.Where(a => a.ID == 1).FirstOrDefault();
                var companyID = comp.ID;
                //var companyID = 1;
                var httpRequest = System.Web.HttpContext.Current.Request;
                int? page_unique = null;
                if (!String.IsNullOrEmpty(httpRequest.Form["page_unique"]))
                {
                    page_unique = Convert.ToInt32(httpRequest.Form["page_unique"]);
                }

                string page_url = httpRequest.Form["page_url"];
                int page = String.IsNullOrEmpty(httpRequest.Form["page"]) ? 1 : Convert.ToInt32(httpRequest.Form["page"]);


                List<products> pro = new List<products>();


                var list = db.sp_Product_Select_Torob("", "", Settings.GetIP(), null, page_unique, companyID, page, page_url).ToList();


                foreach (var item in list)
                {
                    products pr = new products();
                    pr.image_link = item.Image_Url + db.Products.Where(a => a.ID == item.ID).Select(a => a.ImageUrl).FirstOrDefault();
                    pr.image_links = db.ProductImages.Where(a => a.ProductID == item.ID).Select(a => item.Image_Url + a.Image_Url).ToArray();// item.Image_Url;
                    pr.category_name = item.ProductGroup_Title;
                    pr.page_url = item.PageUrl;
                    pr.page_unique = item.ID;
                    pr.current_price = Convert.ToInt64(item.PriceSales) - Convert.ToInt64(item.DiscountPrice);
                    pr.old_price = Convert.ToInt64(item.PriceSales);
                    pr.short_desc = "";
                    pr.availability = item.AvailableCount > 0 ? "instock" : "0";
                    pr.title = item.Name;
                    string[] arry = item.SumerySpecification.Split('\r');
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    foreach (var sp in arry)
                    {
                        if (sp != "")
                        {
                            string[] per = sp.Split(':');
                            if (per.Length > 0)
                            {
                                dic.Add(per[0].Replace("\n", ""), per[1].Replace("\n", ""));
                            }
                        }


                    }
                    pr.spec = dic;
                    pro.Add(pr);

                }
                if (pro.Count() > 0)
                {
                    product.products = pro;
                }

                product.count = db.Products.Where(a => a.Acive == true).Count();
                product.max_pages = product.count / 100;

                log.WriteErrorLog(" WebSite Torob product.products :" + pro.Count().ToString());
                log.WriteErrorLog(" WebSite Torob product.count :" + product.count.ToString());
                log.WriteErrorLog(" WebSite Torob product.max_pages :" + product.max_pages.ToString());
            }
            catch (Exception ex)
            {

                log.WriteErrorLog(" ProducList :" + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" ProducList InnerException :" + ex.InnerException.Message);

                }


            }
            return product;
        }
        public class productFilter
        {
            public string page_unique { get; set; }
            public string page_url { get; set; }
            public int page { get; set; }

        }

        //        {
        // "count": "150",
        // "max_pages": "2",
        // "products": [
        // {
        // "title": "شائومی موبایل گوشی Note 10 Pro",
        // "subtitle": "Xiaomi Mi Note 10 Pro"
        // "page_unique": "12412",
        // "current_price": "5000000",
        // "old_price": "5500000",
        // "availability": "instock",
        // "category_name": "mobile",
        // "image_link": "https://domain.com/images/test.jpg",
        // "page_url": "https://domain.com/product/34/",

        // "spec": {
        // "memory": "4GB",
        // "camera": "12 مگاپیکسل,"
        //,"سفید" :"color "

        // },

        // },

        //} 
        public class productsList
        {
            public int count { get; set; }
            public int max_pages { get; set; }

            public List<products> products { get; set; }
        }

        public class products
        {
            public string[] image_links { get; set; }
            public string image_link { get; set; }
            public string category_name { get; set; }
            public string title { get; set; }
            public string subtitle { get; set; }
            public int page_unique { get; set; }
            public Nullable<long> current_price { get; set; }
            public Nullable<long> old_price { get; set; }
            public string short_desc { get; set; }
            public string page_url { get; set; }
            public string availability { get; set; }
            public Dictionary<string, string> spec { get; set; }

        }
    }
}
