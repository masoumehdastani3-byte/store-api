using API.Models;
using DataAccess;
using System;
using System.Web.Http;

namespace API.Controllers
{
    public class InsertProductImageController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]
        public string Get(

           string Description
        , string Image_Url
        , bool? IsDefault
        , int? ProductID
        )
        {
            try
            {
                DataAccess.ProductImage model = new DataAccess.ProductImage();
                model.Description = Settings.SetNull(Description);
                model.Image_Url = Settings.SetNull(Image_Url);
                model.IsDefault = IsDefault;
                model.ProductID = ProductID;
                db.ProductImages.Add(model);
                var dd = db.SaveChanges();
                return "0";
            }
            catch (Exception ex)
            {
                Models.Log log = new Models.Log();
                log.WriteErrorLog(" InsertProductImage :" + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" InsertProductImage InnerException :" + ex.InnerException.Message);
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
