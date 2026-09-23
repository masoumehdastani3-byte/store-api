using API.Models;
using DataAccess;
using System;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class UpdatePageMenuController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpPost]
        public string Post([FromBody] Rootobject setting
        )
        {
            try
            {
                foreach (var item in setting.Postdata)
                {
                    DataAccess.PageMenu model = db.PageMenus.Where(a => a.SystemCode == item.SystemCode && a.CompanyID == item.CompanyID).FirstOrDefault();
                    if (model == null)
                    {
                        model = new PageMenu();
                        model.Active = item.Active;
                        model.CompanyID = item.CompanyID;
                        model.MenuTitle = Settings.SetNull(item.MenuTitle);
                        model.SystemCode = item.SystemCode;
                        model.SystemName = Settings.SetNull(item.SystemName);
                        db.PageMenus.Add(model);
                    }
                    else
                    {
                        model.Active = item.Active;
                        model.CompanyID = item.CompanyID;
                        model.MenuTitle = Settings.SetNull(item.MenuTitle);

                    }

                }

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

        public class Rootobject
        {
            public Postdata[] Postdata { get; set; }
        }

        public class Postdata
        {
            public bool Active { get; set; }
            public int CompanyID { get; set; }
            public int SystemCode { get; set; }
            public int ID { get; set; }
            public string SystemName { get; set; }
            public string MenuTitle { get; set; }

        }
    }
}
