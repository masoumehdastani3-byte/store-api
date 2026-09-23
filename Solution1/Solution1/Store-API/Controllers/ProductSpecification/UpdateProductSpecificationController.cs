using API.Models;
using DataAccess;
using System;
using System.Linq;
using System.Web.Http;

namespace Api.Controllers
{
    public class UpdateProductSpecificationController : ApiController
    {
        StoreEntities db = new StoreEntities();
        [HttpGet]
        public string Get(string Title, int? ID, string UserName, string Lang)
        {
            try
            {
                DataAccess.ProductSpecification model = db.ProductSpecifications.Where(a => a.ID == ID).FirstOrDefault();
                var count = db.ProductSpecifications.Where(a => a.GroupType == model.GroupType && a.Title == Title).Count();
                if (count == 0)
                {


                    var det = db.ProductDetails.Where(a => a.ProductSpecificationID == ID).Select(a => a.ProductID).ToList();
                    var proc = db.Products.Where(a => det.Any(y => y.Value == a.ID)).ToList();
                    if (proc.Count() > 0)
                    {
                        foreach (var item in proc)
                        {

                            item.SumerySpecification = item.SumerySpecification.Replace(model.Title, Title);
                        }
                    }
                    model.Title = Settings.SetNull(Title);
                    var dd = db.SaveChanges();
                    return "0";
                }
                else
                {
                    return Lang.ToLower().Contains("fa") ? "ویژگی تکراری" : "Duplicate feature";
                }
            }
            catch (Exception ex)
            {
                API.Models.Log log = new API.Models.Log();
                log.WriteErrorLog(" UpdateProductSpecification :" + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" UpdateProductSpecification InnerException :" + ex.InnerException.Message);
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
