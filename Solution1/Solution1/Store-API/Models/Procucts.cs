using System;

namespace API.Models
{

    public class ProductObject
    {
        public Postdata PostData { get; set; }
    }

    public class Postdata
    {
        public Product product { get; set; }

        public detail[] detail { get; set; }
    }


    public class detail
    {
        public string ParamSearch { get; set; }
        public string ParamName { get; set; }
        public string Specification { get; set; }
        public string Specification_new { get; set; }
        public int? ID { get; set; }
        public PeropertyItems[] PeropertyItems { get; set; }
        public listDeatils[] listDeatils { get; set; }
    }
    public class listDeatils
    {
        public int ID { get; set; }
        public string GroupType { get; set; }
        public string Title { get; set; }
        public string GroupTypeName_En { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<bool> Show { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string LogUser { get; set; }
        public string Lang { get; set; }
        public Nullable<bool> IsDefault { get; set; }
    }
    public class Product
    {
        public int ID { get; set; }
        public Nullable<int> CompanyID { get; set; }
        public Nullable<int> TransportationID { get; set; }
        public Nullable<int> ProductGroupID { get; set; }
        public Nullable<decimal> PriceBuy { get; set; }
        public Nullable<int> OffPercent { get; set; }
        public Nullable<decimal> PriceSales { get; set; }
        public string Currency { get; set; }
        public string LogUser { get; set; }
        public Nullable<bool> Acive { get; set; }
        public Nullable<bool> IsAvailable { get; set; }
        public Nullable<int> AvailableCount { get; set; }
        public Nullable<int> SalesCount { get; set; }
        public Nullable<bool> IsSpecialSales { get; set; }
        public Nullable<bool> IsOffPercent { get; set; }
        public string ToDateSpecialSales { get; set; }
        public string FromDateSpecialSales { get; set; }
        public string BarCode { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Lang { get; set; }
        public Nullable<bool> IsViewInstagram { get; set; }
        public Nullable<bool> IsViewTelegram { get; set; }

        public string InstagramTag { get; set; }
    }
    public class Details
    {
        public int ID { get; set; }
        public string GroupTypeName { get; set; }
        public bool? IsShowSearch { get; set; }
        public string Sort { get; set; }

    }
    public class PeropertyItems
    {
        public int ID { get; set; }
        public string Text { get; set; }
    }
}