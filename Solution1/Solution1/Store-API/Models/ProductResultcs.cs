using API.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Api.Models
{
    public class ProductResult
    {
        DataAccess.StoreEntities db = new StoreEntities();
        public List<ProductSelect> SelectProduct(string lang = null, string loguser = null, string logIP = null, Nullable<bool> isSpecialSales = null,
           Nullable<bool> IsNewCreate = null, Nullable<int> productGroupID = null, Nullable<int> productID = null, Nullable<int> companyID = null,
           string stringParamGroupType = null, string stringParamGroupName = null, Nullable<int> PageNumber = null, string OrderByColumn = null, Nullable<int> OrderDirection = null)
        {
            List<ProductSelect> pro;
            string WebSite = Settings.WebsiteName();
            companyID = db.Companies.Where(a => a.WebsiteUrl == WebSite).Select(a => a.ID).FirstOrDefault();


            var list = db.sp_Product_Select(lang, Settings.SetNull(loguser), Settings.GetIP(), isSpecialSales, IsNewCreate, productGroupID, productID, companyID,
                Settings.SetNull(stringParamGroupType), Settings.SetNull(stringParamGroupName), PageNumber, Settings.SetNull(OrderByColumn), OrderDirection, null).ToList();
            pro = new List<ProductSelect>();
            if (stringParamGroupName != null)
            {
                string[] authorsList = stringParamGroupName.Split(',');
                foreach (var item in authorsList)
                {
                    if (item.Length > 1)
                    {
                        list = list.Where(a => a.SumerySpecification.Contains(item)).ToList();
                    }

                }

            }

            foreach (var item in list)
            {
                ProductSelect pr = new ProductSelect();
                pr.Image_Url = item.Image_Url;
                pr.ImageDescription = item.ImageDescription;
                pr.GroupTypeName = item.GroupTypeName;
                pr.Title = item.Title;
                pr.ProductGroup_Title = item.ProductGroup_Title;
                pr.ProductDescription = item.ProductDescription;
                pr.ID = item.ID;
                pr.PriceSales = item.PriceSales;
                pr.IsAvailable = item.IsAvailable;
                pr.OffPercent = item.OffPercent;
                pr.CurrencyCode = item.CurrencyCode;
                pr.CourrencyName = item.CourrencyName;
                pr.SumerySpecification = item.SumerySpecification;
                pr.DiscountPrice = item.DiscountPrice;
                pr.AvailableCount = item.AvailableCount;
                pr.Name = item.Name;
                pr.Grad = item.Grad;
                pr.Sort = item.Sort;
                pr.IsOffPercent = item.IsOffPercent;
                pr.ProductGroupID = item.ProductGroupID;
                pr.VisitedCount = item.VisitedCount;
                pr.IsSpecialSales = item.IsSpecialSales;
                pr.DefaultImageVideo = item.DefaultImageVideo;
                pr.InstagramTag = item.InstagramTag;
                pr.BarCode = item.BarCode;
                var result = db.sp_ProductImage_Select(Settings.SetNull(lang), Settings.SetNull(loguser), pr.ID, null).ToList();
                List<ProductImage> img = new List<ProductImage>();
                foreach (var im in result)
                {
                    ProductImage i = new ProductImage();
                    i.Image_Url = im.Image_Url;
                    i.ID = im.ID;
                    i.ProductID = im.ProductID;
                    i.IsDefault = im.IsDefault;
                    i.Description = im.Description;
                    img.Add(i);
                }
                pr.ProductImage = img;
                pro.Add(pr);

            }
            return pro;
        }


    }
    public class ProductSelect
    {
        public string Image_Url { get; set; }
        public string ImageDescription { get; set; }
        public string GroupTypeName { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string ProductGroup_Title { get; set; }
        public string ProductDescription { get; set; }
        public int ID { get; set; }
        public Nullable<decimal> PriceSales { get; set; }
        public Nullable<decimal> DiscountPrice { get; set; }
        public Nullable<bool> IsAvailable { get; set; }
        public Nullable<bool> IsSpecialSales { get; set; }
        public Nullable<int> AvailableCount { get; set; }

        public Nullable<bool> IsOffPercent { get; set; }
        public Nullable<int> OffPercent { get; set; }
        public Nullable<int> Sort { get; set; }
        public Nullable<int> Grad { get; set; }
        public Nullable<int> ProductGroupID { get; set; }
        public string CurrencyCode { get; set; }
        public string BarCode { get; set; }
        public string CourrencyName { get; set; }
        public string SumerySpecification { get; set; }
        public string DefaultImageVideo { get; set; }
        public string InstagramTag { get; set; }
        public Nullable<int> VisitedCount { get; set; }
        public List<ProductImage> ProductImage { get; set; }
        public Nullable<Boolean> Comparison { get; set; }
    }

    public partial class ProductImage
    {
        public int ID { get; set; }
        public Nullable<int> ProductID { get; set; }
        public string Image_Url { get; set; }
        public string Description { get; set; }
        public Nullable<bool> IsDefault { get; set; }
    }
}