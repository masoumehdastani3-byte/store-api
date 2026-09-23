using API.Models;
using DataAccess;
using Store_API.ir.mizbansms.my;
using Store_API.ir.shaparak.pec1;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace ConfirmBank.Controllers
{
    public class PaymentCallbackController : Controller
    {

        StoreEntities db = new StoreEntities();

        Log log = new Log();



        public ActionResult Index(PaymentCallbackModel model)
        {
            log.Url = "C:\\LogFiles\\Bank\\";
            log.WriteErrorLog($"Parsian PGW service call PaymentCallback ورود مدل");
            if (model == null)
            {
                log.WriteErrorLog($"Parsian PGW service call PaymentCallback مدل خالی است");
                return View();
            }
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(model);

            log.WriteErrorLog($"Parsian PGW service call PaymentCallback : {jsonString}");
            var callBackViewModel = new PaymentCallBackViewModel()
            {
                Amount = model.Amount,
                HashCardNumber = model.HashCardNumber,
                OrderId = model.OrderId,
                RRN = model.RRN,
                status = model.status,
                TerminalNo = model.TerminalNo,
                Token = model.Token,
                TspToken = model.TspToken
            };
            log.WriteErrorLog($"Parsian PGW service call PaymentCallback 1");
            if (model.status == Constants.ParsianPaymentGateway.Successful && (model.Token > 0))
            {
                //var shouldConfirmAfterPay = true;
                log.WriteErrorLog($"Parsian PGW service call PaymentCallback 2");
                //if (shouldConfirmAfterPay)
                //{
                try
                {
                    var pay = db.Payments.Where(a => a.Token == model.Token.ToString()).FirstOrDefault();
                    var jsonpay = Newtonsoft.Json.JsonConvert.SerializeObject(pay);
                    log.WriteErrorLog($"Parsian PGW service call PaymentCallback 3 " + jsonpay);
                    var Password = db.BankAcounts.Where(a => a.CompanyID == pay.CompanyID).Select(a => a.Password).FirstOrDefault();
                    log.WriteErrorLog($"Parsian PGW service call PaymentCallback -3 " + Password);
                    //ایجاد یک نمونه از سرویس تایید پرداخت
                    using (var confirmSvc = new ConfirmService())
                    {
                        log.WriteErrorLog($"Parsian PGW service call PaymentCallback 4");
                        confirmSvc.Url = API.Common.ConfigHelper.ParsianPGWConfirmServiceUrl;


                        //ایجاد یک نمونه از نوع پارامتر سرویس تایید پرداخت
                        var confirmRequestData = new ClientConfirmRequestData();

                        //شناسه پذیرندگی باید در فراخوانی سرویس تایید تراکنش پرداخت ارائه شود
                        confirmRequestData.LoginAccount = Password;

                        //توکن باید ارائه شود
                        confirmRequestData.Token = model.Token ?? -1;

                        //فراخوانی سرویس و دریافت نتیجه فراخوانی
                        var confirmResponse = confirmSvc.ConfirmPayment(confirmRequestData);
                        callBackViewModel.ConfirmResponseStatus = confirmResponse.Status;

                        //کنترل کد وضعیت نتیجه فراخوانی
                        //درصورتی که موفق باشد، باید خدمات یا کالا به کاربر پرداخت کننده ارائه شود
                        if (confirmResponse.Status == Constants.ParsianPaymentGateway.Successful)
                        {
                            pay.BankRefrence = model.RRN.ToString();
                            pay.Status = 1;
                            var order = db.PurchaseDetails.Where(a => a.PurchaseID == pay.PurchaseID).ToList();

                            foreach (var item in order)
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


                            log.WriteErrorLog($"Redirect 500 PurchaseID:" + pay.PurchaseID.ToString());
                            var ord = db.Purchases.Where(a => a.ID == pay.PurchaseID).FirstOrDefault();
                            ord.Status = 1;
                            db.SaveChanges();

                            var customer = db.Customers.Where(a => a.ID == ord.CustomerID).FirstOrDefault();
                            var CustomerID = customer.ID;
                            string url = db.CompanySettings.Where(a => a.SettingName == "UrlFull" && a.CompanyID == ord.CompanyID).Select(a => a.SettingValue).FirstOrDefault();
                            url = url + "/my-purchases?Id=" + CustomerID.ToString();
                            log.WriteErrorLog($"Redirect 500 PurchaseID:" + pay.PurchaseID.ToString() + " URL :" + url);

                            // ارسال پیام در پس‌زمینه تا ریدایرکت بلاک / قفل نشود
                            QueueSendMessage(customer != null ? customer.FullName : null,
                                customer != null ? customer.Mobile : null,
                                pay.CompanyID);

                            return Redirect(url);
                        }
                    }
                    log.WriteErrorLog($"Parsian PGW service call PaymentCallback 4");
                }
                catch (Exception ex)
                {
                    log.WriteErrorLog("Parsian PGW service call PaymentCallback {ex.Message}" + ex.Message);
                    if (ex.InnerException != null)
                    {
                        log.WriteErrorLog("Parsian PGW service call PaymentCallback {ex.InnerException}" + ex.InnerException.Message);
                    }
                }


                //}

                //بررسی اینکه آیا کاربر باید خدمات یا کالای مورد نظر خود را در قبال وجهی که پرداخت کرده است، دریافت کند
                //یا دریافت نکند.
                //در صورتی که مثلاً کاربر سفارش خود را پس از پرداخت وجه، لغو نماید، برای برگشت وجه پرداخت شده اش به حسابش
                //باید از سرویس برگشت وجه استفاده شود
                //حداکثر زمان مجاز برای فراخوانی سرویس برگشت وجه به حساب مشتری
                //که شروع آن پس از دریافت توکن از سرویس درخواست پرداخت می باشد
                //یک ساعت است

                //if(no service or product will be delivered to the customer)
                //call ReversalService.ReversalRequest() method to indicate return of money to customer.
                //ReversePayment(model.Token ?? 0L);
            }







            return View();
        }
        public ActionResult About()
        {
            //WSSMS v = new WSSMS();
            //var a=  v.sendsms2("mpi_09125572091", "7840566", "09126766025", "dffdf", "50002123262933");
            //var b=  v.sendsms("mpi_09125572091", "7840566", "09126766025", "dffdf", "50002123262933","41");

            return View();
        }
        /// <summary>
        /// پیام بله/پیامک را در پس‌زمینه می‌فرستد تا ریدایرکت کاربر منتظر نماند و قفل نشود.
        /// </summary>
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
                        bgLog.WriteErrorLog("QueueSendMessage Error: " + ex.Message);
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
                // پیام بله به فروشنده (پس از تأیید پرداخت) — بدون GetResult روی UI thread
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
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            WSSMS v = new WSSMS();

            var setting = db.CompanySettings.Where(a => a.CompanyID == 1 && a.Category == "SMS").ToList();

            if (setting != null)
            {
                string username = setting.Where(a => a.SettingName == "SMSUsername").Select(a => a.SettingValue).FirstOrDefault();
                string password = setting.Where(a => a.SettingName == "Password").Select(a => a.SettingValue).FirstOrDefault();
                string Body = setting.Where(a => a.SettingName == "SMSMessage").Select(a => a.SettingValue).FirstOrDefault();
                string SMSNumber = setting.Where(a => a.SettingName == "SMSNumber").Select(a => a.SettingValue).FirstOrDefault();
                string api = setting.Where(a => a.SettingName == "api").Select(a => a.SettingValue).FirstOrDefault();
                string Mobile = setting.Where(a => a.SettingName == "SMSReciveNumber").Select(a => a.SettingValue).FirstOrDefault();
                var result = v.sendsms(username, password, "09126766025", Body, SMSNumber, api);


            }

            return View();
        }
    }
}