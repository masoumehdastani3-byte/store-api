using DataAccess;
using System;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class DeleteProductGroupController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]
        public string Get(string UserName, string Lang, int ID)
        {
            try
            {
                var productCount = db.Products.Where(a => a.ProductGroupID == ID).Count();
                if (productCount == 0)
                {
                    DataAccess.ProductGroup model = db.ProductGroups.Where(a => a.ID == ID).FirstOrDefault();

                    db.ProductGroups.Remove(model);
                    var dd = db.SaveChanges();
                    return "0";
                }
                else
                {
                    return Lang == "fa" ? "از این گروه در تعریف محصول استفاده شده است قابل حذف نیست" : "This group used in product definition cannot be deleted";
                }

            }
            catch (Exception ex)
            {
                Models.Log log = new Models.Log();
                log.WriteErrorLog(" InsertProductGroup :" + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" InsertProductGroup InnerException :" + ex.InnerException.Message);
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
