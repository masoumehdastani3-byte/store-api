using API.Models;
using API.Models.Sepehr;
using DataAccess;
using Store_API.ir.mizbansms.my;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ConfirmBank.Controllers
{
    public class SepehrPaymentCallbackController : Controller
    {
        StoreEntities db = new StoreEntities();
        Log log = new Log();

        public ActionResult Index(SepehrCallbackModel model)
        {
            log.Url = "C:\\LogFiles\\Bank\\";
            log.WriteErrorLog("Sepehr PaymentCallback ورود");

            if (model == null)
            {
                log.WriteErrorLog("Sepehr PaymentCallback مدل خالی است");
                return View();
            }

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            log.WriteErrorLog("Sepehr PaymentCallback: " + jsonString);

            try
            {
                if (model.respcode != "0" || string.IsNullOrWhiteSpace(model.digitalreceipt))
                {
                    log.WriteErrorLog("Sepehr PaymentCallback ناموفق. respcode=" + model.respcode + " msg=" + model.respmsg);
                    return View();
                }

                long invoiceId;
                if (!long.TryParse(model.invoiceid, out invoiceId))
                {
                    log.WriteErrorLog("Sepehr PaymentCallback invoiceid نامعتبر: " + model.invoiceid);
                    return View();
                }

                var pay = db.Payments.Where(a => a.ID == invoiceId).FirstOrDefault();
                if (pay == null)
                {
                    log.WriteErrorLog("Sepehr PaymentCallback پرداخت یافت نشد: " + invoiceId);
                    return View();
                }

                if (pay.Status == 1)
                {
                    log.WriteErrorLog("Sepehr PaymentCallback قبلاً تایید شده: " + invoiceId);
                    return RedirectToPurchases(pay);
                }

                if (!string.IsNullOrWhiteSpace(model.digitalreceipt)
                    && db.Payments.Any(a => a.BankRefrence == model.digitalreceipt && a.Status == 1 && a.ID != pay.ID))
                {
                    log.WriteErrorLog("Sepehr PaymentCallback digitalreceipt تکراری: " + model.digitalreceipt);
                    return View();
                }

                long amountRial;
                if (long.TryParse(model.amount, out amountRial) && pay.Amount.HasValue && amountRial != Convert.ToInt64(pay.Amount.Value))
                {
                    log.WriteErrorLog("Sepehr PaymentCallback مبلغ ناسازگار. callback=" + amountRial + " db=" + pay.Amount);
                    return View();
                }

                long tid;
                if (!long.TryParse(model.terminalid, out tid))
                {
                    long.TryParse(SepehrPaymentHelper.DefaultTerminalId, out tid);
                }

                var advice = SepehrPaymentHelper.Advice(new SepehrAdviceRequest
                {
                    digitalreceipt = model.digitalreceipt,
                    Tid = tid,
                    InvoiceID = model.invoiceid
                });

                var adviceOk = string.Equals(advice.Status, "Ok", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(advice.Status, "Duplicate", StringComparison.OrdinalIgnoreCase);

                if (!adviceOk)
                {
                    pay.Error = "Advice: " + advice.Status + " " + advice.Message;
                    db.SaveChanges();
                    log.WriteErrorLog("Sepehr Advice ناموفق: " + advice.Status + " " + advice.Message);
                    return View();
                }

                pay.BankRefrence = model.digitalreceipt;
                if (!string.IsNullOrWhiteSpace(model.rrn))
                    pay.Description = (pay.Description ?? "") + "|RRN:" + model.rrn;
                pay.Status = 1;
                pay.Token = pay.Token ?? model.digitalreceipt;

                var orderDetails = db.PurchaseDetails.Where(a => a.PurchaseID == pay.PurchaseID).ToList();
                foreach (var item in orderDetails)
                {
                    var prod = db.Products.Where(a => a.ID == item.ProductID).FirstOrDefault();
                    if (prod != null)
                    {
                        prod.SalesCount = item.ShoppingCount + (prod.SalesCount == null ? 0 : prod.SalesCount);
                        prod.AvailableCount = prod.AvailableCount - item.ShoppingCount;
                        prod.UpdateDateAvailableCount = DateTime.Now;
                        if (prod.AvailableCount <= 0)
                        {
                            prod.IsAvailable = false;
                        }
                    }
                }

                var ord = db.Purchases.Where(a => a.ID == pay.PurchaseID).FirstOrDefault();
                if (ord != null)
                {
                    ord.Status = 1;
                }
                db.SaveChanges();

                var customer = db.Customers.Where(a => a.ID == ord.CustomerID).FirstOrDefault();
                // ارسال پیام در پس‌زمینه؛ ریدایرکت بلاک نشود
                QueueSendMessage(
                    customer != null ? customer.FullName : null,
                    customer != null ? customer.Mobile : null,
                    pay.CompanyID);
                return RedirectToPurchases(pay);
            }
            catch (Exception ex)
            {
                log.WriteErrorLog("Sepehr PaymentCallback Exception: " + ex.Message);
                if (ex.InnerException != null)
                    log.WriteErrorLog("Sepehr PaymentCallback Inner: " + ex.InnerException.Message);
                return View();
            }
        }

        private ActionResult RedirectToPurchases(DataAccess.Payment pay)
        {
            var ord = db.Purchases.Where(a => a.ID == pay.PurchaseID).FirstOrDefault();
            if (ord == null)
                return View();

            var customer = db.Customers.Where(a => a.ID == ord.CustomerID).FirstOrDefault();
            if (customer == null)
                return View();

            string url = db.CompanySettings.Where(a => a.SettingName == "UrlFull" && a.CompanyID == ord.CompanyID)
                .Select(a => a.SettingValue).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(url))
                return View();

            url = url + "/my-purchases?Id=" + customer.ID.ToString();
            log.WriteErrorLog("Sepehr Redirect PurchaseID:" + pay.PurchaseID + " URL:" + url);
            return Redirect(url);
        }

        private void QueueSendMessage(string fullName, string mobile, int? companyId)
        {
            Task.Run(() =>
            {
                try
                {
                    SendMessage(fullName, mobile, companyId);
                }
                catch (Exception ex)
                {
                    try
                    {
                        var bgLog = new Log();
                        bgLog.Url = "C:\\LogFiles\\Bank\\";
                        bgLog.WriteErrorLog("Sepehr QueueSendMessage Error: " + ex.Message);
                    }
                    catch { }
                }
            });
        }

        private void SendMessage(string fullName, string mobile, int? CompanyID)
        {
            if (CompanyID == null)
                return;

            string StorBody = (fullName ?? "") + " " + "یک خرید پرداخت شده در سیستم دارد." + Environment.NewLine;
            StorBody += " شماره مشتری :" + (mobile ?? "");
            try
            {
                try
                {
                    var baleResult = Settings.NotifySellerPurchasePaid(CompanyID, fullName, mobile)
                        .ConfigureAwait(false).GetAwaiter().GetResult();
                    log.WriteErrorLog("SendBale CompanyID:" + CompanyID + " result:" + baleResult);
                }
                catch (Exception bex)
                {
                    log.WriteErrorLog("SendBale CompanyID:" + CompanyID + " Error" + bex.Message);
                }

                using (var localDb = new StoreEntities())
                {
                    WSSMS v = new WSSMS();
                    var setting = localDb.CompanySettings.Where(a => a.CompanyID == CompanyID && a.Category == "SMS").ToList();
                    if (setting != null && setting.Count > 0)
                    {
                        string username = setting.Where(a => a.SettingName == "SMSUsername").Select(a => a.SettingValue).FirstOrDefault();
                        string password = setting.Where(a => a.SettingName == "Password").Select(a => a.SettingValue).FirstOrDefault();
                        string Body = setting.Where(a => a.SettingName == "SMSMessage").Select(a => a.SettingValue).FirstOrDefault();
                        string SMSNumber = setting.Where(a => a.SettingName == "SMSNumber").Select(a => a.SettingValue).FirstOrDefault();
                        string api = setting.Where(a => a.SettingName == "api").Select(a => a.SettingValue).FirstOrDefault();
                        string receiveMobile = setting.Where(a => a.SettingName == "SMSReciveNumber").Select(a => a.SettingValue).FirstOrDefault();
                        var result = v.sendsms(username, password, mobile, Body, SMSNumber, api);
                        v.sendsms(username, password, receiveMobile, StorBody, SMSNumber, api);
                        log.WriteErrorLog("SendSMS CompanyID:" + CompanyID + " result :" + (result != null && result.Any() ? result.First().ToString() : ""));
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteErrorLog("SendSMS CompanyID:" + CompanyID + " Error" + ex.Message);
            }
        }
    }
}
