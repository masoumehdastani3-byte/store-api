using API.Models;
using DataAccess;
using System;
using System.Web.Http;

namespace API.Controllers
{
    public class InsertProductUserCommentController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();

        [HttpPost]
        public string Post(
         [FromBody] Root prod
        )
        {
            try
            {
                DataAccess.ProductUserComment model = new DataAccess.ProductUserComment();
                model.Active = true;
                model.CompanyID = prod.@params.CompanyID;
                model.CreateDate = DateTime.Now;
                model.Description = Settings.SetNull(prod.@params.Description);
                if (Settings.SetNull(prod.@params.FullName) != null)
                {
                    model.FullName = Settings.SetNull(prod.@params.FullName);
                }
                else
                {
                    model.FullName = Settings.SetNull(prod.@params.Lang) == "fa" ? "ناشناس" : "Unknown";
                }

                model.Lang = Settings.SetNull(prod.@params.Lang);
                model.LogIP = Settings.GetIP();
                model.ProductID = prod.@params.ProductID;
                db.ProductUserComments.Add(model);
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
        public class Params
        {
            public int CompanyID { get; set; }
            public string Description { get; set; }
            public string FullName { get; set; }
            public string Lang { get; set; }
            public int ProductID { get; set; }
        }

        public class Root
        {
            public Params @params { get; set; }
        }

    }
}
