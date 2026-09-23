using API.Models;
using DataAccess;
using System;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class UpdateProductUserCommentController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]
        public string Get(
        bool? Active
        , int? CompanyID
        , string Description
        , string FullName
        , int? ID
        , string Lang
        , string LogUser
        , int? ProductID
        )
        {
            try
            {
                DataAccess.ProductUserComment model = db.ProductUserComments.Where(a => a.ID == ID).FirstOrDefault();
                model.Active = Active;
                model.CompanyID = CompanyID;
                model.CreateDate = DateTime.Now;
                model.Description = Settings.SetNull(Description);
                model.FullName = Settings.SetNull(FullName);
                model.Lang = Settings.SetNull(Lang);
                model.LogIP = Settings.GetIP();
                model.LogUser = Settings.SetNull(LogUser);
                model.ProductID = ProductID;
                var dd = db.SaveChanges();
                return "0";
            }
            catch (Exception ex)
            {
                Models.Log log = new Models.Log();
                log.WriteErrorLog(" InsertProductUserComment :" + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" InsertProductUserComment InnerException :" + ex.InnerException.Message);
                    return ex.InnerException.Message;
                }
                else
                {
                    return ex.Message;
                }
            }
        }
        [HttpGet]


        public string Get(
          bool? Active
        , int? ID
        , string Lang
        , string UserName
        )
        {
            try
            {
                DataAccess.ProductUserComment model = db.ProductUserComments.Where(a => a.ID == ID).FirstOrDefault();
                model.Active = Active;

                model.CreateDate = DateTime.Now;

                model.LogUser = Settings.SetNull(UserName);

                var dd = db.SaveChanges();
                return "0";
            }
            catch (Exception ex)
            {
                Models.Log log = new Models.Log();
                log.WriteErrorLog(" InsertProductUserComment :" + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" InsertProductUserComment InnerException :" + ex.InnerException.Message);
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
