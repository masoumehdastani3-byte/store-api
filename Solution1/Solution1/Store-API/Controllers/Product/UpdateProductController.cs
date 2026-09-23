using DataAccess;
using System;
using System.Linq;
using System.Web.Http;
namespace Api.Controllers.Product
{
    public class UpdateProductController : ApiController
    {
        StoreEntities db = new StoreEntities();
        API.Models.PersianCulture pc = new API.Models.PersianCulture();
        [HttpGet]
        public string Get(int ID, decimal PriceSales)
        {
            try
            {
                DataAccess.Product model = db.Products.Where(a => a.ID == ID).FirstOrDefault();
                model.PriceSales = PriceSales;
                var dd = db.SaveChanges();

                return "0";
            }
            catch (Exception ex)
            {
                API.Models.Log log = new API.Models.Log();
                log.WriteErrorLog(" UpadateProduct :" + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" UpadateProduct InnerException :" + ex.InnerException.Message);
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
