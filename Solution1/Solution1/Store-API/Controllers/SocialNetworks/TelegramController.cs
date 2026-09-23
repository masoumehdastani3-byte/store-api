using DataAccess;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace Api.Controllers.SocialNetworks
{
    public class TelegramController : ApiController
    {
        static TelegramBotClient bot;
        API.Models.Log log = new API.Models.Log();

        [HttpGet]
        public async Task<string> Get(int CompanyID)
        {
            try
            {
                //https://core.telegram.org/method/messages.getFullChat
                StoreEntities db = new StoreEntities();
                var comp = db.Companies.Where(a => a.ID == CompanyID).FirstOrDefault();
                if (comp.TelegramChanalName != null && comp.TelegramChatID != null)
                {

                    string Token = "1698691757:AAFZnAee1dmmbWGhRGknHQMaLEJDXK3JK1c";
                    if (comp.TelegramBotToken != null)
                    {
                        Token = comp.TelegramBotToken;
                    }
                    bot = new TelegramBotClient(Token);
                    string uploadPath = System.Configuration.ConfigurationManager.AppSettings["DocumentUrl"];
                    var list = db.Products.Where(a => a.IsViewTelegram == true && a.IsSendTelegram == false).ToList();
                    foreach (var item in list)
                    {
                        string urlBack = comp.WebsiteUrl + "/p/" + item.ID;
                        var FileUrl = uploadPath + item.ImageUrl;
                        using (var stream = System.IO.File.Open(FileUrl, FileMode.Open))
                        {
                            FileToSend fts = new FileToSend();
                            fts.Content = stream;
                            fts.Filename = FileUrl.Split('\\').Last();
                            // var test = await bot.SendPhotoAsync("@channel Name or chat_id", fts, "My Text");
                            var responce = await bot.SendPhotoAsync(comp.TelegramChatID, fts, item.SumerySpecification + "\r\n\r\n" + "جهت اطلاعات بیشتر و آگاهی از آخرین قیمت ها آدرس سایت زیر مراجعه نمایید" + "\r\n" + urlBack + "\r\n\r\n\r\n" + item.InstagramTag);
                            item.TelegramError = responce.Text;
                            item.IsSendTelegram = true;
                            item.TelegramTimeSend = DateTime.Now;
                            db.SaveChanges();
                        }
                    }

                }
                else
                {
                    return "نام کانال تلگرام در اطلاعات شرکت ثبت نشده است";
                }
            }
            catch (Exception ex)
            {

                log.WriteErrorLog(" Telegram CompanyID: " + CompanyID.ToString() + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" Telegram InnerException :" + ex.InnerException.Message);
                    return ex.InnerException.Message;
                }
                else
                {
                    return ex.Message;
                }
            }
            return "0";
        }
    }






}

