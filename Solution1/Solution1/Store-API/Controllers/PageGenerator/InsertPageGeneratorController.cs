using API.Models;
using DataAccess;
using System;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class InsertPageGeneratorController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpPost]
        public string Post(input input)
        {
            try
            {
                string typ = "inset";//فرم آپدیت جداست ، اینجا صرفا جهت عدم ثبت مجدد است
                DataAccess.PageGenerator model = new DataAccess.PageGenerator();
                //if (db.PageGenerators.Where(a => a.PageLocationID == input.PageLocationID && a.CompanyID == input.CompanyID && a.Active==true).Count() == 0)
                //{
                model.Active = true;
                var sort = db.PageGenerators.Where(a => a.CompanyID == input.CompanyID && a.PageLocationID == 0).Max(a => a.Sort);
                model.Sort = sort == null ? 1 : sort + 1;
                //}
                //else
                //{
                //    model = db.PageGenerators.Where(a => a.PageLocationID == input.PageLocationID && a.CompanyID == input.CompanyID && a.Active == true).FirstOrDefault();
                //    typ = "update";
                //}

                model.CompanyID = input.CompanyID;
                string WebSite = Settings.WebsiteName();
                input.PageContent = input.PageContent.Replace("http://" + WebSite + "/", "");
                input.PageContent = input.PageContent.Replace("ckfinder/userfiles/images/", "http://" + WebSite + "/ckfinder/userfiles/images/");
                input.PageContent = input.PageContent.Replace("/http:", "http:");
                model.PageContent = Settings.SetNull(input.PageContent);
                model.PageLocation = Settings.SetNull(input.PageLocation);
                model.PageLocationID = input.PageLocationID;
                model.PageTitle = Settings.SetNull(input.PageTitle);
                model.PageSystemCode = input.PageSystemCode;
                model.CreateDate = DateTime.Now;
                //if (typ == "inset")
                //{
                db.PageGenerators.Add(model);
                //}

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

            public string PageContent { get; set; }
            public string PageLocation { get; set; }
            public int? PageLocationID { get; set; }
            public string PageTitle { get; set; }
            public int? PageSystemCode { get; set; }

        }
    }
}
