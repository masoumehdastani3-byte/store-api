using API.Models;
using DataAccess;
using System;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{

    public class UpdatePageGeneratorController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpPost]
        public string Post(input input)
        {
            try
            {
                string WebSite = Settings.WebsiteName();
                input.PageContent = input.PageContent.Replace("http://" + WebSite + "/", "");
                input.PageContent = input.PageContent.Replace("ckfinder/userfiles/images/", "http://" + WebSite + "/ckfinder/userfiles/images/");
                input.PageContent = input.PageContent.Replace("/http:", "http:");
                DataAccess.PageGenerator model = db.PageGenerators.Where(a => a.ID == input.ID).FirstOrDefault();
                model.CompanyID = input.CompanyID;
                model.PageContent = Settings.SetNull(input.PageContent);
                model.PageLocation = Settings.SetNull(input.PageLocation);
                model.PageLocationID = input.PageLocationID;
                model.PageTitle = Settings.SetNull(input.PageTitle);
                model.PageSystemCode = input.PageSystemCode;
                model.UpdateDate = DateTime.Now;
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



        [HttpGet]
        public string Get(

         int? ID
        , int? Sort
        )
        {
            try
            {
                DataAccess.PageGenerator model = db.PageGenerators.Where(a => a.ID == ID).FirstOrDefault();

                model.Sort = Sort;
                model.UpdateDate = DateTime.Now;
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
        [HttpGet]
        public string Get(
         bool? Active
        , int? ID
        )
        {
            try
            {
                DataAccess.PageGenerator model = db.PageGenerators.Where(a => a.ID == ID).FirstOrDefault();

                model.Active = Active;
                model.UpdateDate = DateTime.Now;
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
        public class input
        {
            public int? CompanyID { get; set; }
            public int? ID { get; set; }
            public string PageContent { get; set; }
            public string PageLocation { get; set; }
            public int? PageLocationID { get; set; }
            public string PageTitle { get; set; }
            public int? PageSystemCode { get; set; }
        }

    }
}
