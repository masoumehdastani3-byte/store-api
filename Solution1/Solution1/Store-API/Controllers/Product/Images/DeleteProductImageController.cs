using API.Models;
using DataAccess;
using System;
using System.IO;
using System.Linq;
using System.Web.Http;

namespace API.Controllers
{
    public class DeleteProductImageController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        [HttpGet]
        public string Get(string UserName, string Lang, int ID)
        {
            try
            {
                DataAccess.ProductImage model = db.ProductImages.Where(a => a.ID == ID).FirstOrDefault();
                if (model == null)
                {
                    return "NotFound";
                }

                // تصویر پیش‌فرض کالا از گالری قابل حذف نیست
                if (model.IsDefault == true)
                {
                    return Lang != null && Lang.ToLower().Contains("fa")
                        ? "تصویر پیش‌فرض کالا قابل حذف نیست. برای تغییر آن از صفحه ویرایش کالا استفاده کنید."
                        : "Default product image cannot be deleted. Change it from the product edit form.";
                }

                DeleteImageFiles(model.Image_Url);

                db.ProductImages.Remove(model);
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

        private void DeleteImageFiles(string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl) || relativeUrl.ToLower().Contains(".mp4"))
            {
                return;
            }

            try
            {
                string largePath = ProductImageThumbHelper.ToPhysicalPath(relativeUrl);
                string smallPath = ProductImageThumbHelper.ToPhysicalPath(ProductImageThumbHelper.GetSmallRelativeUrl(relativeUrl));

                // سازگاری با فایل‌های قدیمی _s
                string legacySmall = relativeUrl.Replace('\\', '/');
                int lastDot = legacySmall.LastIndexOf('.');
                if (lastDot > 0)
                {
                    legacySmall = legacySmall.Substring(0, lastDot) + "_s" + legacySmall.Substring(lastDot);
                }
                string legacySmallPath = ProductImageThumbHelper.ToPhysicalPath(legacySmall);

                if (File.Exists(largePath))
                {
                    File.Delete(largePath);
                }
                if (File.Exists(smallPath))
                {
                    File.Delete(smallPath);
                }
                if (File.Exists(legacySmallPath))
                {
                    File.Delete(legacySmallPath);
                }
            }
            catch (Exception ex)
            {
                Models.Log log = new Models.Log();
                log.WriteErrorLog(" DeleteProductImage files :" + ex.Message);
            }
        }
    }
}
