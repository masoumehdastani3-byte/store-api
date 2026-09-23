using API.Models;
using DataAccess;
using System;
using System.Linq;
using System.Web.Http;
namespace API.Controllers.Users
{
    public class InsertUserProfileController : ApiController
    {
        private StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        public string Get(
         string Lang,
         Nullable<int> CompanyID,
         string UserName,
         string Password,
         Nullable<bool> Active,
         string Name,
         string Family,
         string Mobile,
         string Email,
         string Tell,
         string LogUser,
         string NationalCode,
         Nullable<int> Gender,
         Nullable<int> RoleID

         )
        {
            try
            {
                int count = 0;
                if (Settings.SetNull(UserName) != null)
                {
                    count = db.UserProfiles.Where(a => a.UserName == UserName).Count();
                }
                if (count == 0)
                {
                    DataAccess.UserProfile model = new UserProfile();
                    model.CreateDate = DateTime.Now;
                    model.UserName = Settings.SetNull(UserName);
                    model.Password = Settings.SetNull(Password);
                    model.Name = Settings.SetNull(Name);
                    model.Family = Settings.SetNull(Family);
                    model.Mobile = Settings.SetNull(Mobile);
                    model.Email = Settings.SetNull(Email);
                    model.NationalCode = Settings.SetNull(NationalCode);
                    model.Tell = Settings.SetNull(Tell);
                    model.LogUser = Settings.SetNull(LogUser);
                    model.CompanyID = CompanyID;
                    model.RoleID = RoleID;
                    model.Gender = Gender;
                    model.Active = Active;
                    db.UserProfiles.Add(model);
                    var dd = db.SaveChanges();
                    return "0";
                }
                else
                {
                    string ms = Lang == "fa" ? "نام کاربری تکراری" : "Duplicate username";

                    return ms;
                }
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
