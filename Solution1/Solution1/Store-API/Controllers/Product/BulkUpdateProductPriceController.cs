using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Api.Controllers.Product
{
    /// <summary>
    /// ویرایش گروهی قیمت فروش براساس خصوصیات انتخاب‌شده (ProductSpecification).
    /// کالاهایی که همه Specهای انتخاب‌شده را دارند فیلتر می‌شوند.
    /// </summary>
    public class BulkUpdateProductPriceController : ApiController
    {
        StoreEntities db = new StoreEntities();

        /// <param name="CompanyID">شرکت (اجباری)</param>
        /// <param name="ProductGroupID">گروه کالا (اختیاری)</param>
        /// <param name="SpecificationIDs">شناسه‌های ProductSpecification با جداکننده کاما</param>
        /// <param name="PercentChange">درصد تغییر؛ مثلاً 10 = افزایش ۱۰٪، -5 = کاهش ۵٪</param>
        /// <param name="PreviewOnly">اگر true فقط تعداد کالاهای مشمول برگردانده می‌شود</param>
        /// <param name="UserName">کاربر لاگ</param>
        [HttpGet]
        public string Get(
            int CompanyID,
            string SpecificationIDs,
            decimal PercentChange,
            bool PreviewOnly = false,
            int? ProductGroupID = null,
            string UserName = null)
        {
            try
            {
                if (CompanyID <= 0)
                {
                    return "CompanyID is required";
                }

                if (string.IsNullOrWhiteSpace(SpecificationIDs))
                {
                    return "SpecificationIDs is required";
                }

                if (PercentChange == 0 && !PreviewOnly)
                {
                    return "PercentChange cannot be zero";
                }

                var specIds = SpecificationIDs
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => int.TryParse(s, out _))
                    .Select(int.Parse)
                    .Distinct()
                    .ToList();

                if (specIds.Count == 0)
                {
                    return "No valid SpecificationIDs";
                }

                // محصولات شرکت (و در صورت نیاز گروه) که همه Specهای انتخاب‌شده را دارند
                IQueryable<DataAccess.Product> query = db.Products
                    .Where(p => p.CompanyID == CompanyID);

                if (ProductGroupID.HasValue && ProductGroupID.Value > 0)
                {
                    int groupId = ProductGroupID.Value;
                    query = query.Where(p => p.ProductGroupID == groupId);
                }

                foreach (int specId in specIds)
                {
                    int id = specId;
                    query = query.Where(p =>
                        db.ProductDetails.Any(d =>
                            d.ProductID == p.ID &&
                            d.ProductSpecificationID == id));
                }

                List<DataAccess.Product> products = query.ToList();
                int count = products.Count;

                if (PreviewOnly)
                {
                    return "0|" + count.ToString();
                }

                if (count == 0)
                {
                    return "0|0";
                }

                decimal factor = 1m + (PercentChange / 100m);
                DateTime now = DateTime.Now;

                foreach (DataAccess.Product product in products)
                {
                    if (!product.PriceSales.HasValue)
                    {
                        continue;
                    }

                    decimal newPrice = Math.Round(product.PriceSales.Value * factor, 0, MidpointRounding.AwayFromZero);
                    if (newPrice < 0)
                    {
                        newPrice = 0;
                    }

                    product.PriceSales = newPrice;
                    product.UpdateDate = now;
                    if (!string.IsNullOrWhiteSpace(UserName))
                    {
                        product.LogUser = UserName;
                    }
                }

                db.SaveChanges();
                return "0|" + count.ToString();
            }
            catch (Exception ex)
            {
                API.Models.Log log = new API.Models.Log();
                log.WriteErrorLog(" BulkUpdateProductPrice :" + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" BulkUpdateProductPrice InnerException :" + ex.InnerException.Message);
                    return ex.InnerException.Message;
                }
                return ex.Message;
            }
        }
    }
}
