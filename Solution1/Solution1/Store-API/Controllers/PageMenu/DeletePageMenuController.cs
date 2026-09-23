using DataAccess;
using System;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class DeletePageMenuController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]
        public string Get(string UserName, string Lang, int ID)
        {
            try
            {
                DataAccess.PageMenu model = db.PageMenus.Where(a => a.ID == ID).FirstOrDefault();

                db.PageMenus.Remove(model);
                var dd = db.SaveChanges();
                return "0";
            }
            catch (Exception ex)
            {
                Models.Log log = new Models.Log();
                log.WriteErrorLog(" InsertPageMenu :" + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" InsertPageMenu InnerException :" + ex.InnerException.Message);
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
