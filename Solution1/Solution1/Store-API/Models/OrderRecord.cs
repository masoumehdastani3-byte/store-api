namespace API.Models
{
    public class order
    {
        public string Address { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string CodeMelli { get; set; }
        public string Lang { get; set; }
        public string LogUser { get; set; }
        public string City { get; set; }
        public int CustomerID { get; set; }
        public int CompanyID { get; set; }
        public decimal Total { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalDiscount { get; set; }
        /// <summary>1 = ایران‌زمین/پارسیان (فعلی)، 2 = صادرات/سپهر</summary>
        public int PaymentGateway { get; set; }
        public Orderdetail[] OrderDetails { get; set; }

    }

    public class Orderdetail
    {
        public int ProductID { get; set; }
        public string ProducName { get; set; }
        public int ShoppingCount { get; set; }
        public int AvalaibleCount { get; set; }
        public int UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal Total { get; set; }
        public int OrderID { get; set; }
    }

}