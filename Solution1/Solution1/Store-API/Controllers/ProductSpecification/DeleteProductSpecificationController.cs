using DataAccess;
using System;
using System.Linq;
using System.Web.Http;

namespace Api.Controllers
{
    public class DeleteProductSpecificationController : ApiController
    {
        StoreEntities db = new StoreEntities();
        [HttpGet]
        public string Get(int ID, string Lang)
        {
            try
            {
                DataAccess.ProductSpecification model = db.ProductSpecifications.Where(a => a.ID == ID).FirstOrDefault();
                var countUse = db.ProductDetails.Where(a => a.ProductSpecificationID == ID).Count();
                if (countUse > 0)
                {
                    return Lang.ToLower().Contains("fa") ? "از این خصوصیت در تعریف محصولات استفاده شده است" : "This feature is used in the product definition";
                }
                db.ProductSpecifications.Remove(model);
                var dd = db.SaveChanges();
                return "0";
            }
            catch (Exception ex)
            {
                API.Models.Log log = new API.Models.Log();
                log.WriteErrorLog(" DeleteProductSpecification :" + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" DeleteProductSpecification InnerException :" + ex.InnerException.Message);
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
