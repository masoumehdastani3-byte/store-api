using API.Models;
using DataAccess;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Logger;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
namespace Api.Controllers.SocialNetworks
{
    public class InstagramController : ApiController
    {
        private static UserSessionData user;
        API.Models.Log log = new API.Models.Log();
        [HttpGet]
        public async Task<string> Get(int CompanyID)
        {
            try
            {
                StoreEntities db = new StoreEntities();
                user = new UserSessionData();
                var comp = db.Companies.Where(a => a.ID == CompanyID).FirstOrDefault();
                if (comp != null)
                {
                    log.WriteErrorLog(" Instagram CompanyID: " + CompanyID.ToString() + " comp.InstagramUserName " + comp.InstagramUserName + " comp.InstagramPassword " + comp.InstagramPassword);
                }

                if (comp.InstagramUserName != null && comp.InstagramPassword != null)
                {


                    user.UserName = comp.InstagramUserName;
                    user.Password = comp.InstagramPassword;


                    string uploadPath = System.Configuration.ConfigurationManager.AppSettings["DocumentUrl"];
                    var list = db.Products.Where(a => a.IsViewInstagram == true && a.IsSendInstagram == false).ToList();
                    log.WriteErrorLog(" Instagram CompanyID: " + CompanyID.ToString() + " list " + list.ToArray().ToString());


                    ctx.api = InstaApiBuilder.CreateBuilder()
                        .SetUser(user)
                        .UseLogger(new DebugLogger(LogLevel.All))
                        .SetRequestDelay(RequestDelay.FromSeconds(0, 1))
                        .Build();

                    var loginRequest = await ctx.api.LoginAsync();

                    log.WriteErrorLog(" Instagram CompanyID: " + CompanyID.ToString() + " loginRequest " + loginRequest.ToString());
                    string Message = "";
                    if (loginRequest.Succeeded)
                    {
                        foreach (var item in list)
                        {

                            if (item.ImageUrl != null && item.ImageUrl != "")
                            {

                                string urlBack = comp.WebsiteUrl + "/p/" + item.ID;
                                System.Drawing.Image img = System.Drawing.Image.FromFile(uploadPath + item.ImageUrl);

                                img = Settings.resizeImage(img, 1350, 1080);
                                string NewName = uploadPath + @"\Instagram\" + item.ID.ToString() + ".jpg";
                                var media = new InstaImageUpload(NewName, 1080, 1080);



                                media.UserTags.Add(new InstaUserTagUpload()
                                {
                                    Username = comp.InstagramUserName,
                                    X = 0.5,
                                    Y = 0.5
                                });
                                var res = await ctx.api.MediaProcessor
                                    .UploadPhotoAsync(media, item.SumerySpecification + "\r\n\r\n" + "جهت اطلاعات بیشتر و آگاهی از آخرین قیمت ها آدرس سایت زیر مراجعه نمایید" + "\r\n" + urlBack + "\r\n\r\n\r\n" + item.InstagramTag);

                                log.WriteErrorLog(" Instagram CompanyID: " + CompanyID.ToString() + " res.Succeeded " + res.Succeeded.ToString());
                                log.WriteErrorLog(" Instagram CompanyID: " + CompanyID.ToString() + " res.Succeeded " + res.Info.Message);

                                if (res.Succeeded == true)
                                {
                                    item.IsSendInstagram = true;
                                }
                                else
                                {
                                    if (!res.Info.Message.Contains("error"))
                                    {
                                        item.IsSendInstagram = true;
                                    }
                                    else
                                    {
                                        item.IsSendInstagram = false;
                                    }
                                }



                                item.InstagramError = res.Info.Message;
                                item.IstagramTimeSend = DateTime.Now;
                                Message = res.Info.Message;

                                db.SaveChanges();

                            }
                        }
                        return Message;

                    }
                    else
                    {
                        string ff = loginRequest.Info.Message;
                        log.WriteErrorLog(" Instagram CompanyID: " + CompanyID.ToString() + " " + loginRequest.Info.Message);
                        return "نام کاربری و رمز عبور  اینستاگرام اشتباه است";
                    }
                }
                else
                {
                    return "نام کاربری و رمز عبور اینستاگرام در اطلاعات شرکت ثبت نشده است";
                }
            }



            catch (Exception ex)
            {
                log.WriteErrorLog(" Instagram CompanyID: " + CompanyID.ToString() + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" Instagram InnerException :" + ex.InnerException.Message);
                    return ex.InnerException.Message;
                }
                else
                {
                    return ex.Message;
                }
            }
            return "0";
        }
        public static class ctx
        {
            public static IInstaApi api;
        }

    }
}
