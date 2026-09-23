using API.Models;
using DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class SelectRestaurantSettingController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]
        public List<CompanySettings> Get(string Lang, string UserName, int? CompanyID, string Category)
        {
            var result = db.sp_CompanySettings_Select(Lang, UserName, CompanyID, Category).ToList();

            List<CompanySettings> dd = new List<CompanySettings>();
            CompanySettings ff = new CompanySettings();
            List<Models.Company_Settings> Settings = new List<Models.Company_Settings>();
            foreach (var item2 in result)
            {
                Models.Company_Settings ant_Se = new Models.Company_Settings();
                ant_Se.Active = item2.Active;
                ant_Se.Category = item2.Category;

                ant_Se.CompanyID = item2.CompanyID;
                ant_Se.SettinCaption = item2.SettinCaption;
                ant_Se.SettingName = item2.SettingName;
                ant_Se.SettingValue = item2.SettingValue;
                ant_Se.Descriptions = item2.Descriptions;


                Settings.Add(ant_Se);
            }
            ff.Company_Settings = Settings;

            dd.Add(ff);


            return dd;

        } // List
    }
}
