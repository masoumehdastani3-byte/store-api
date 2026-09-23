using DataAccess;
using System;
using System.Web.Http;
namespace API.Controllers.Users.UserAccess
{
    public class InsertUserAccessController : ApiController
    {
        StoreEntities db = new StoreEntities();

        [HttpGet]
        public string Get(string Lang, string UserName, string SelectorID, int RoleID)
        {
            try
            {
                DataAccess.UserAccess model = new DataAccess.UserAccess();
                model.SelectorID = SelectorID;
                model.RoleID = RoleID;
                model.LogUser = UserName;
                model.CreateDate = DateTime.Now;
                db.UserAccesses.Add(model);
                db.SaveChanges();
                var dd = db.SaveChanges();

                return "0";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
