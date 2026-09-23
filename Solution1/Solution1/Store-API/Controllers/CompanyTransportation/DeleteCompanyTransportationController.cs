using DataAccess;
using System;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class DeleteCompanyTransportationController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]
        public string Get(string UserName, string Lang, int ID)
        {
            try
            {
                DataAccess.CompanyTransportation model = db.CompanyTransportations.Where(a => a.ID == ID).FirstOrDefault();

                db.CompanyTransportations.Remove(model);
                var dd = db.SaveChanges();
                return "0";
            }
            catch (Exception ex)
            {
                Models.Log log = new Models.Log();
                log.WriteErrorLog(" InsertCompanyTransportation :" + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" InsertCompanyTransportation InnerException :" + ex.InnerException.Message);
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
