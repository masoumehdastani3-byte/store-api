using Api.Models;
using API.Models;
using DataAccess;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Telegram.Bot;

namespace Api.Controllers.SocialNetworks
{
    public class TelegramGroupChatIDController : ApiController
    {
        static TelegramBotClient bot;
        API.Models.Log log = new API.Models.Log();
        string url = "https://api.telegram.org/bot1698691757:AAFZnAee1dmmbWGhRGknHQMaLEJDXK3JK1c/getUpdates";
        [HttpGet]
        public async Task<string> Get(int CompanyID)
        {
            try
            {
                //https://core.telegram.org/method/messages.getFullChat
                StoreEntities db = new StoreEntities();
                string WebSite = Settings.WebsiteName();
                CompanyID = db.Companies.Where(a => a.WebsiteUrl == WebSite).Select(a => a.ID).FirstOrDefault();
                var comp = db.Companies.Where(a => a.ID == CompanyID).FirstOrDefault();
                if (comp.TelegramChanalName != null && comp.TelegramChatID == null)
                {
                    string Token = "1698691757:AAFZnAee1dmmbWGhRGknHQMaLEJDXK3JK1c";
                    if (comp.TelegramBotToken != null)
                    {
                        Token = comp.TelegramBotToken;
                    }
                    string url = "https://api.telegram.org/bot" + Token + "/getUpdates";
                    Uri baseAddress = new Uri(url);
                    string content;
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = baseAddress;
                        client.DefaultRequestHeaders.Accept.Clear();
                        var w = client.GetAsync(url);
                        w.Wait();
                        HttpResponseMessage response = w.Result;
                        content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<Telegramobject>(content);
                        if (result.ok == true)
                        {
                            if (result.result.Count() > 0)
                            {
                                //var id = result.result.Where(a => a.my_chat_member.chat.username == comp.TelegramChanalName).Select(a => a.my_chat_member.chat.id).FirstOrDefault();
                                var list = result.result.Where(a => a.my_chat_member != null);
                                var id = list.Where(a => a.my_chat_member.chat.username == comp.TelegramChanalName).Select(a => a.my_chat_member.chat.id).FirstOrDefault();

                                if (id.ToString() != "0")
                                {
                                    comp.TelegramChatID = id.ToString();
                                    db.SaveChanges();
                                    return " chat id : " + comp.TelegramChatID;
                                }
                                else
                                {
                                    return "هیچ پیامی اخیرا در کانال گذاشته نشده است";
                                }
                            }
                            else
                            {
                                return "هیچ پیامی اخیرا در کانال گذاشته نشده است";
                            }

                        }
                        else
                        {
                            log.WriteErrorLog(" GetTelegramGroupChatID CompanyID: " + CompanyID.ToString() + " " + result.error_code + " " + result.description);
                            return "Error " + result.error_code + " " + result.description;
                        }
                    }



                }
                else
                {
                    if (comp.TelegramChanalName == null)
                    {
                        return "نام کانال تلگرام در اطلاعات شرکت ثبت نشده است";
                    }
                    if (comp.TelegramChatID != null)
                    {
                        return " chat id : " + comp.TelegramChatID;
                    }

                }
            }
            catch (Exception ex)
            {

                log.WriteErrorLog(" GetTelegramGroupChatID CompanyID: " + CompanyID.ToString() + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" GetTelegramGroupChatID InnerException :" + ex.InnerException.Message);
                    return ex.InnerException.Message;
                }
                else
                {
                    return ex.Message;
                }
            }
            return "";
        }


    }




}

