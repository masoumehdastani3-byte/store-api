using DataAccess;
using System;
using System.Linq;
using System.Web.Http;
namespace API.Controllers.Users
{
    public class DeleteUserProfileController : ApiController
    {
        StoreEntities db = new StoreEntities();
        [HttpGet]

        public string Get(string UserName, int ID)
        {
            try
            {
                var gg = db.UserProfiles.Where(a => a.UserID == ID).FirstOrDefault();
                db.UserProfiles.Remove(gg);
                var dd = db.SaveChanges();
                return "0";

            }
            catch (Exception ex)
            {
                Models.Log log = new Models.Log();

                log.WriteErrorLog(" InsertWebsites :" + ex.Message);

                if (ex.InnerException != null)
                {

                    log.WriteErrorLog(" InsertWebsites InnerException :" + ex.InnerException.Message);
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
