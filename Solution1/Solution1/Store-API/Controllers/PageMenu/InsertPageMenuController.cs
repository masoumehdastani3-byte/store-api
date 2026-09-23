using API.Models;
using DataAccess;
using System;
using System.Web.Http;

namespace API.Controllers
{
    public class InsertPageMenuController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]
        public string Get(bool? Active
        , int? CompanyID
        , string MenuTitle
        , int? SystemCode
        , string SystemName
        )
        {
            try
            {
                DataAccess.PageMenu model = new DataAccess.PageMenu();
                model.Active = Active;
                model.CompanyID = CompanyID;
                model.MenuTitle = Settings.SetNull(MenuTitle);
                model.SystemCode = SystemCode;
                model.SystemName = Settings.SetNull(SystemName);
                db.PageMenus.Add(model);
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
