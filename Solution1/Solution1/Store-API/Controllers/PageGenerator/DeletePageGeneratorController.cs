using DataAccess;
using System;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class DeletePageGeneratorController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]
        public string Get(string UserName, string Lang, int ID)
        {
            try
            {
                DataAccess.PageGenerator model = db.PageGenerators.Where(a => a.ID == ID).FirstOrDefault();

                db.PageGenerators.Remove(model);
                var dd = db.SaveChanges();
                return "0";
            }
            catch (Exception ex)
            {
                Models.Log log = new Models.Log();
                log.WriteErrorLog(" InsertPageGenerator :" + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" InsertPageGenerator InnerException :" + ex.InnerException.Message);
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
