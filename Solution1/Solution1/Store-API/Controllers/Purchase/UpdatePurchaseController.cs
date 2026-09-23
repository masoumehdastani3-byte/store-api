using DataAccess;
using Store_API.ir.mizbansms.my;
using System;
using System.Linq;
using System.Web.Http;
namespace Api.Controllers
{

    public class UpdatePurchaseController : ApiController
    {
        StoreEntities db = new StoreEntities();
        API.Models.PersianCulture pc = new API.Models.PersianCulture();
        API.Models.Log log = new API.Models.Log();
        [HttpGet]

        public string Get(
         int? ID
        , string Lang
        , string UserName
        , int? StatusID
        , string Description_Admin
        )
        {
            try
            {
                var list = db.Purchases.Where(a => a.ID == ID).FirstOrDefault();
                list.Status = StatusID;
                list.Description_Admin = Description_Admin;
                list.UpdateDate = DateTime.Now;
                list.LogUser = UserName;
                var dd = db.SaveChanges();
                SendMessage(ID, list.CustomerID, StatusID);
                return "0";
            }
            catch (Exception ex)
            {
                API.Models.Log log = new API.Models.Log();
                log.WriteErrorLog(" UpdatePurchase :" + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" UpdatePurchase InnerException :" + ex.InnerException.Message);
                    return ex.InnerException.Message;
                }
                else
                {
                    return ex.Message;
                }
            }
        }
        private void SendMessage(int? PurchasesID, int? CustomerID, int? StatusID)
        {
            var model = db.Customers.Where(a => a.ID == CustomerID).FirstOrDefault();

            WSSMS v = new WSSMS();

            try
            {
                string Body = "گالری فرش داستانی" + Environment.NewLine;
                Body += " خرید شما به شناسه " + PurchasesID.ToString() + Environment.NewLine;
                if (StatusID == 2)
                {
                    Body += "به وضعیت در حال ارسال رفت.";
                }
                if (StatusID == 3)
                {
                    Body += "به وضعیت ارسال شده رفت";
                }

                var result = v.sendsms("mpi_09125572091", "7840566", model.Mobile, Body, "50002123262933", "41");
            }
            catch (Exception ex)
            {
                log.WriteErrorLog($"SendSMS  PurchasesID:" + PurchasesID.ToString() + " Error" + ex.Message);
            }


        }
    }
}
