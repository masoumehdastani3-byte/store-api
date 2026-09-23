using API.Models;
using DataAccess;
using System;
using System.Data;
using System.Linq;
using System.Web.Http;
namespace API.Controllers.Companies
{
    public class UpdateCompanyController : ApiController
    {
        private StoreEntities db = new StoreEntities();

        public string Get(
             int ID,
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
             Nullable<int> CompanyGroupID_BaseData = null,
             string ContractName = null,
             string Description = null,
             string Address = "",
             string LogoUrl = null,
             string BackgroudUrl = null,
             string TelegramChanalName = null,
             string TelegramUserName = null,
             string InstagramUserName = null,
             string InstagramPassword = null,
             string TelegramBotToken = null,
             string NamadTag = null,
             string LicenseTag1 = null,
             string LicenseTag2 = null,
             string VideoDefault_ImageUrl = null,
             string SocialNetworks1_LogUrl = null,
             string SocialNetworks2_LogUrl = null,
             string SocialNetworks1_Url = null,
             string SocialNetworks2_Url = null,
             string HeaderColor = null,
             string ButtonColor = null,
             string HeaderFontColor = null,
             string ButtonFontColor = null,
             string PriceColor = null,
             string PriceFontColor = null


            )
        {
            try
            {
                Company model = db.Companies.Where(a => a.ID == ID).FirstOrDefault();
                model.UpdateDate = DateTime.Now;
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
                model.CompanyGroupID_BaseData = CompanyGroupID_BaseData;
                model.ContractName = Settings.SetNull(ContractName);
                model.Description = Settings.SetNull(Description);
                model.Address = Settings.SetNull(Address);
                model.LogoUrl = Settings.SetNull(LogoUrl);
                model.BackgroudUrl = Settings.SetNull(BackgroudUrl);
                model.TelegramChanalName = Settings.SetNull(TelegramChanalName);
                //model.TelegramChatID = Settings.SetNull(TelegramChatID);
                model.InstagramUserName = Settings.SetNull(InstagramUserName);
                model.InstagramPassword = Settings.SetNull(InstagramPassword);
                model.TelegramUserName = Settings.SetNull(TelegramUserName) != null ? TelegramUserName.Replace("@", "") : null;
                model.TelegramBotToken = Settings.SetNull(TelegramBotToken);
                model.NamadTag = Settings.SetNull(NamadTag);
                model.LicenseTag1 = Settings.SetNull(LicenseTag1);
                model.LicenseTag2 = Settings.SetNull(LicenseTag2);
                model.VideoDefault_ImageUrl = Settings.SetNull(VideoDefault_ImageUrl);
                model.SocialNetworks1_LogUrl = Settings.SetNull(SocialNetworks1_LogUrl);
                model.SocialNetworks2_LogUrl = Settings.SetNull(SocialNetworks2_LogUrl);
                model.SocialNetworks1_Url = Settings.SetNull(SocialNetworks1_Url);
                model.SocialNetworks2_Url = Settings.SetNull(SocialNetworks2_Url);
                model.HeaderColor = Settings.SetNull(HeaderColor);
                model.ButtonColor = Settings.SetNull(ButtonColor);
                model.ButtonFontColor = Settings.SetNull(ButtonFontColor);
                model.HeaderFontColor = Settings.SetNull(HeaderFontColor);
                model.PriceColor = Settings.SetNull(PriceColor);
                model.PriceFontColor = Settings.SetNull(PriceFontColor);

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