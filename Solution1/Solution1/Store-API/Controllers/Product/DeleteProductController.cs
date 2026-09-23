using DataAccess;
using System;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class DeleteProductController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]
        public string Get(string UserName, string Lang, int ID)
        {
            try
            {
                var countPerchase = db.PurchaseDetails.Where(a => a.ProductID == ID).Count();
                if (countPerchase == 0)
                {
                    var des = db.ProductDescriptions.Where(a => a.ProductID == ID).ToList();
                    db.ProductDescriptions.RemoveRange(des);
                    var det = db.ProductDetails.Where(a => a.ProductID == ID).ToList();
                    db.ProductDetails.RemoveRange(det);
                    var img = db.ProductImages.Where(a => a.ProductID == ID).ToList();
                    db.ProductImages.RemoveRange(img);
                    DataAccess.Product model = db.Products.Where(a => a.ID == ID).FirstOrDefault();

                    db.Products.Remove(model);
                    var dd = db.SaveChanges();
                    return "0";
                }
                else
                {
                    return Lang == "fa" ? "این محصول به فروش رسیده قابل حذف نیست" : "This sold product cannot be removed";
                }

            }
            catch (Exception ex)
            {
                Models.Log log = new Models.Log();
                log.WriteErrorLog(" DeleteProduct :" + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" DeleteProduct InnerException :" + ex.InnerException.Message);
                    return ex.InnerException.Message;
                }
                else
                {
                    return ex.Message;
                }
            }
        }
    }
}
