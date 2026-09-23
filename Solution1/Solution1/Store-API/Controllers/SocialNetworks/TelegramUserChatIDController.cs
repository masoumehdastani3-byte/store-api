using Api.Models;
using API.Models;
using DataAccess;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.Controllers.SocialNetworks
{
    public class TelegramUserChatIDController : ApiController
    {
        API.Models.Log log = new API.Models.Log();

        [HttpGet]
        public async Task<string> Get(int CompanyID)
        {
            try
            {

                StoreEntities db = new StoreEntities();
                string WebSite = Settings.WebsiteName();
                CompanyID = db.Companies.Where(a => a.WebsiteUrl == WebSite).Select(a => a.ID).FirstOrDefault();
                var comp = db.Companies.Where(a => a.ID == CompanyID).FirstOrDefault();
                if (comp.TelegramUserName != null && comp.TelegramUserChatID == null)
                {
                    if (comp.TelegramBotToken == null)
                    {
                        return "توکن بازوی بله در اطلاعات شرکت ثبت نشده است";
                    }
                    string Token = comp.TelegramBotToken;
                    string url = "https://tapi.bale.ai/bot" + Token + "/getUpdates";
                    string content;
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(url);
                        content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<Telegramobject>(content);


                        if (result.ok == true)
                        {
                            if (result.result.Count() > 0)
                            {
                                var list = result.result.Where(a => a.message != null);
                                var my_id = list.Where(a => a.message.chat.username == comp.TelegramUserName).Select(a => a.message.chat.id).FirstOrDefault();
                                if (my_id.ToString() != "0")
                                {
                                    comp.TelegramUserChatID = my_id.ToString();
                                    db.SaveChanges();
                                    await SendMessage(my_id.ToString(), Token);
                                    return " chat id : " + comp.TelegramUserChatID;
                                }
                                else
                                {
                                    return "هیچ پیامی اخیرا به ربات فرستاده نشده است";
                                }

                            }
                            else
                            {
                                return "هیچ پیامی اخیرا به ربات فرستاده نشده است";
                            }

                        }
                        else
                        {
                            log.WriteErrorLog(" GetBaleUserChatID CompanyID: " + CompanyID.ToString() + " " + result.error_code + " " + result.description);
                            return "Error " + result.error_code + " " + result.description;
                        }


                    }



                }
                else
                {
                    if (comp.TelegramUserName == null)
                    {
                        return "نام کاربری بله در اطلاعات شرکت ثبت نشده است";
                    }
                    if (comp.TelegramUserChatID != null)
                    {
                        return " chat id : " + comp.TelegramUserChatID;
                    }

                }
            }
            catch (Exception ex)
            {
                log.WriteErrorLog(" GetBaleUserChatID CompanyID: " + CompanyID.ToString() + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" GetBaleUserChatID InnerException :" + ex.InnerException.Message);
                    return ex.InnerException.Message;
                }
                else
                {
                    return ex.Message;
                }
            }
            return "";
        }
        private static async Task SendMessage(string id, string Token)
        {
            string url = "https://tapi.bale.ai/bot" + Token + "/sendMessage";
            var payload = new { chat_id = id, text = "Your Bale chat id : " + id };
            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                await client.PostAsync(url, content);
            }
        }

    }




}
