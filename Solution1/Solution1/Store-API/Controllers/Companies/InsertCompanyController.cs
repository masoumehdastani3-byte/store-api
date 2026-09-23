using API.Models;
using DataAccess;
using System;
using System.Web.Http;
namespace API.Controllers.Companies
{
    public class InsertCompanyController : ApiController
    {
        StoreEntities db = new StoreEntities();
        [HttpGet]
        public string Get(

             string Name = "",
             string Tell = "",
             string Fax = "",
             string WatsUpNumber = "",
             string SMSNumber = "",
             string Email = "",
             string WebsiteUrl = "",
             string TelegramUrl = "",
             string InstagramUrl = "",
             string Mobile = "",
             Nullable<bool> Active = null,
             string LogUser = "",
             Nullable<int> CountryID_BasicData = null,
             Nullable<int> CityID_BasicData = null,
             Nullable<int> CompanyGroupID_ProductSpecification = null,
             string ContractName = null,
             string Description = null,
             string Address = "",
             string LogoUrl = null,
             string BackgroudUrl = null,
             string TelegramChanalName = null,
             string InstagramUserName = null,
             string InstagramPassword = null,
             string TelegramUserName = null,
             string TelegramBotToken = null
            )
        {
            try
            {
                Company model = new Company();
                model.CreateDate = DateTime.Now;
                model.Name = Settings.SetNull(Name);
                model.Tell = Settings.SetNull(Tell);
                model.Fax = Settings.SetNull(Fax);
                model.WatsUpNumber = Settings.SetNull(WatsUpNumber);
                model.SMSNumber = Settings.SetNull(SMSNumber);
                model.Email = Settings.SetNull(Email);
                model.WebsiteUrl = Settings.SetNull(WebsiteUrl) != null ? Settings.SetNull(WebsiteUrl.Replace("www.", "").Replace("http://", "")) : null;
                model.TelegramUrl = Settings.SetNull(TelegramUrl);
                model.InstagramUrl = Settings.SetNull(InstagramUrl);
                model.Mobile = Settings.SetNull(Mobile);
                model.Active = Active;
                model.LogUser = Settings.SetNull(LogUser);
                model.CountryID_BasicData = CountryID_BasicData;
                model.CityID_BasicData = CityID_BasicData;
                model.CountryID_BasicData = CityID_BasicData;
                model.ContractName = Settings.SetNull(ContractName);
                model.Description = Settings.SetNull(Description);
                model.Address = Settings.SetNull(Address);
                model.LogoUrl = Settings.SetNull(LogoUrl);
                model.BackgroudUrl = Settings.SetNull(BackgroudUrl);
                model.TelegramChanalName = Settings.SetNull(TelegramChanalName);
                //model.TelegramChatID =Settings.SetNull(TelegramChatID);
                model.InstagramUserName = Settings.SetNull(InstagramUserName);
                model.InstagramPassword = Settings.SetNull(InstagramPassword);
                model.TelegramUserName = Settings.SetNull(TelegramUserName);
                model.TelegramBotToken = Settings.SetNull(TelegramBotToken);
                db.Companies.Add(model);
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
