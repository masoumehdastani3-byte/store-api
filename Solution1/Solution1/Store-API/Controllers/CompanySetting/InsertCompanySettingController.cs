using API.Models;
using DataAccess;
using System;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class InsertRestauranSettingController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();

        [HttpPost]
        public string Post([FromBody] Rootobject setting)
        {
            string Type = "Update";
            try
            {
                foreach (var item in setting.Postdata)
                {
                    foreach (var item2 in item.Company_Settings)
                    {
                        Type = "Update";
                        DataAccess.CompanySetting model = db.CompanySettings.Where(a => a.SettingName == item2.SettingName && a.CompanyID == item2.CompanyID).FirstOrDefault();
                        if (model == null)
                        {
                            Type = "Insert";
                            model = new DataAccess.CompanySetting();
                        }
                        model.Active = item2.Active;
                        if (Type == "Insert")
                        {

                            model.CompanyID = item2.CompanyID;
                            model.SettinCaption = Settings.SetNull(item2.SettinCaption);
                            model.SettingName = Settings.SetNull(item2.SettingName);
                            model.Category = Settings.SetNull(item2.Category);

                        }
                        model.SettingValue = Settings.SetNull(item2.SettingValue);
                        model.Descriptions = Settings.SetNull(item2.Descriptions);


                        if (Type == "Insert")
                        {
                            db.CompanySettings.Add(model);
                        }
                    }


                }

                var dd = db.SaveChanges();
                return "0";

            }
            catch (Exception ex)
            {
                Models.Log log = new Models.Log();
                log.WriteErrorLog(" InsertCompany_Settings :" + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" InsertCompany_Settings InnerException :" + ex.InnerException.Message);
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
            public int PageShow { get; set; }
            public int MaxPageShow { get; set; }
            public string PageDescription { get; set; }
            public Company_Settings[] Company_Settings { get; set; }
        }

        public class Company_Settings
        {
            public int CompanyID { get; set; }
            public string SettingName { get; set; }
            public string SettinCaption { get; set; }
            public string SettingValue { get; set; }
            public string Descriptions { get; set; }
            public string Category { get; set; }
            public int PageShow { get; set; }
            public bool Active { get; set; }
            public string PageDescription { get; set; }

        }

    }
}
