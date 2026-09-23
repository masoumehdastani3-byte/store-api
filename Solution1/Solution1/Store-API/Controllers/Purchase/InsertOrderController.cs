using API.Common;
using API.Models;
using DataAccess;
using Store_API.ir.shaparak.pec;
using System;
using System.Linq;
using System.Web.Http;
namespace API.Controllers.Order
{

    public class InsertOrderController : ApiController
    {
        StoreEntities db = new StoreEntities();
        Models.PersianCulture pc = new Models.PersianCulture();
        API.Models.Log log = new API.Models.Log();
        [HttpPost]

        public dynamic Post([FromBody] order model)
        {
            log.Url = "C:\\LogFiles\\Bank\\";
            PaymentRequestResponseModel ret = new PaymentRequestResponseModel();
            try
            {


                var customer = db.Customers.Where(a => a.CodeMelli == model.CodeMelli && a.CompanyID == model.CompanyID).FirstOrDefault();
                log.WriteErrorLog(" customer :");
                DataAccess.Customer cust = new Customer();
                if (customer == null)
                {
                    cust.Address = model.Address;
                    cust.Email = model.Email;
                    cust.FullName = model.FullName;
                    cust.Mobile = model.Mobile;
                    cust.City = model.City;
                    cust.CompanyID = model.CompanyID;
                    cust.CodeMelli = model.CodeMelli;
                    db.Customers.Add(cust);

                    db.SaveChanges();
                    model.CustomerID = cust.ID;
                }
                else
                {
                    model.CustomerID = customer.ID;
                }
                DataAccess.Purchase order = new DataAccess.Purchase();
                order.CompanyID = model.CompanyID;
                order.CustomerID = model.CustomerID;
                order.CreateDate = DateTime.Now;
                order.DateShamsi = pc.MiladiToShamsi(DateTime.Now);
                order.Description = model.Description;
                order.Status = 0;
                order.Total = model.Total;
                order.TotalDiscount = model.TotalDiscount;

                db.Purchases.Add(order);
                db.SaveChanges();
                log.WriteErrorLog(" Purchase :" + order.ID.ToString());

                foreach (var item in model.OrderDetails)
                {
                    var prod = db.Products.Where(a => a.ID == item.ProductID).FirstOrDefault();

                    DataAccess.PurchaseDetail datails = new PurchaseDetail();
                    datails.AvalaibleCount = item.AvalaibleCount;
                    datails.ProducName = item.ProducName;
                    datails.ProductID = item.ProductID;
                    datails.PurchaseID = order.ID;
                    datails.ShoppingCount = item.ShoppingCount;
                    datails.TotalPrice = item.TotalPrice;
                    datails.UnitPrice = item.UnitPrice;
                    if (prod != null)
                    {
                        datails.UnitDiscount = prod.OffPercent;
                    }

                    datails.TotalUnitDisount = item.TotalDiscount;
                    db.PurchaseDetails.Add(datails);
                }

                db.SaveChanges();
                log.WriteErrorLog(" InsertOrder order.ID :" + order.ID.ToString());
                //--------------------------------ورود به درگاه بانک------------------------------------

                int gateway = model.PaymentGateway <= 0 ? 1 : model.PaymentGateway;
                log.WriteErrorLog("InsertOrder PaymentGateway=" + gateway);

                long amountRial = Convert.ToInt64(model.Total * 10);
                if (amountRial <= 0)
                {
                    log.WriteErrorLog(" InsertOrder invalid Amount. Total=" + model.Total + " Discount=" + model.TotalDiscount);
                    return Json(new
                    {
                        Message = "مبلغ قابل پرداخت نامعتبر است",
                        status = -1,
                    });
                }

                // 2 = بانک صادرات (سپهر)
                if (gateway == 2)
                {
                    return SendToSepehrBank(model, order.ID, amountRial);
                }

                // 1 = بانک ایران‌زمین / درگاه فعلی پارسیان
                SalePaymentRequestModel model_bank = new SalePaymentRequestModel();
                var bankAccounts = db.BankAcounts.Where(a => a.CompanyID == model.CompanyID && a.Active == true).ToList();
                var info = bankAccounts.FirstOrDefault(a => !SepehrPaymentHelper.IsSaderatBankName(a.BankName));
                if (info == null)
                {
                    info = bankAccounts.FirstOrDefault();
                }
                if (info == null)
                {
                    return Json(new { status = 10 });
                }

                model_bank.Amount = amountRial;
                model_bank.LoginAccount = info.Password;
                model_bank.ConfirmAfterPayment = true;
                model_bank.AdditionalData = order.ID.ToString() + "," + model.CompanyID.ToString() + "," + model.FullName;
                model_bank.Originator = model.Mobile;
                model_bank.CallBackUrl = info.ReturnURL;

                log.WriteErrorLog("Send to bank InsertOrder order.ID" + order.ID.ToString());
                return SendToBank(model_bank);



            }
            catch (Exception ex)
            {

                log.WriteErrorLog(" InsertOrder :" + ex.Message);
                if (ex.InnerException != null)
                {
                    log.WriteErrorLog(" InsertOrder InnerException :" + ex.InnerException.Message);
                    return Json(new
                    {
                        Message = ex.InnerException.Message,
                        status = -1,

                    });
                }
                else
                {
                    return Json(new
                    {
                        Message = ex.Message,
                        status = -1,

                    });
                }
            }

        }

        private dynamic SendToSepehrBank(order model, int purchaseId, long amountRial)
        {
            var info = db.BankAcounts.Where(a => a.CompanyID == model.CompanyID).ToList()
                .FirstOrDefault(a => SepehrPaymentHelper.IsSaderatBankName(a.BankName));

            long terminalId = 0;
            string callbackUrl = null;

            if (info != null)
            {
                callbackUrl = info.ReturnURL;
                if (!string.IsNullOrWhiteSpace(info.Terminal))
                    long.TryParse(info.Terminal.Trim(), out terminalId);
            }

            if (terminalId <= 0 && !long.TryParse(SepehrPaymentHelper.DefaultTerminalId, out terminalId))
            {
                return Json(new { Message = "ترمینال درگاه صادرات تنظیم نشده است", status = -1 });
            }

            if (string.IsNullOrWhiteSpace(callbackUrl))
                callbackUrl = SepehrPaymentHelper.DefaultCallbackUrl;

            if (string.IsNullOrWhiteSpace(callbackUrl))
            {
                return Json(new { Message = "آدرس بازگشت درگاه صادرات (callback) تنظیم نشده است", status = -1 });
            }

            DataAccess.Payment payment = new DataAccess.Payment();
            payment.Amount = amountRial;
            payment.BankName = "بانک صادرات - سپهر";
            payment.CompanyID = model.CompanyID;
            payment.CreateDate = DateTime.Now;
            payment.CreateDate_Shamsi = pc.MiladiToShamsi(DateTime.Now);
            payment.PaymentType = 2;
            payment.PurchaseID = purchaseId;
            payment.Status = 0;
            payment.Description = purchaseId + "," + model.CompanyID + "," + model.FullName + ",Sepehr";
            db.Payments.Add(payment);
            db.SaveChanges();

            var tokenResult = SepehrPaymentHelper.GetToken(new API.Models.Sepehr.SepehrGetTokenRequest
            {
                TerminalID = terminalId,
                Amount = amountRial,
                InvoiceID = payment.ID,
                callbackURL = callbackUrl,
                payload = payment.Description
            });

            if (tokenResult.Status == 0 && !string.IsNullOrWhiteSpace(tokenResult.ResolvedToken))
            {
                payment.Token = tokenResult.ResolvedToken;
                db.SaveChanges();
                var redirectUrl = SepehrPaymentHelper.BuildRedirectUrl(tokenResult.ResolvedToken, terminalId);
                log.WriteErrorLog("Sepehr redirect: " + redirectUrl);
                return Json(new
                {
                    status = 0,
                    location = redirectUrl
                });
            }

            payment.Error = tokenResult.Message ?? ("Status=" + tokenResult.Status);
            db.SaveChanges();
            return Json(new
            {
                Message = string.IsNullOrWhiteSpace(tokenResult.Message)
                    ? "خطا در دریافت توکن درگاه صادرات (کد: " + tokenResult.Status + ")"
                    : tokenResult.Message,
                status = -1
            });
        }

        private dynamic SendToBank(API.Models.SalePaymentRequestModel model_bank)
        {
            string[] info = model_bank.AdditionalData.Split(',');
            DataAccess.Payment payment = new DataAccess.Payment();
            payment.Amount = model_bank.Amount;
            payment.BankName = "تجارت الکترنیک پارسیان";
            payment.CompanyID = Convert.ToInt32(info[1]);
            payment.CreateDate = DateTime.Now;
            payment.CreateDate_Shamsi = pc.MiladiToShamsi(DateTime.Now);
            payment.PaymentType = 1;
            payment.PurchaseID = Convert.ToInt32(info[0]);
            payment.Status = 0;
            payment.Description = model_bank.AdditionalData;
            db.Payments.Add(payment);
            db.SaveChanges();


            long token = 0;
            short paymentStatus = Int16.MinValue;
            ClientPaymentResponseDataBase responseData = null;
            using (var service = new SaleService())
            {
                //it is not recommended to bypass Server Certificate Validation, due to vaiolating security concerns.
                System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback((o, xc, xch, sslP) => true);


                //set Service Url from your application configuration
                service.Url = ConfigHelper.ParsianPGWSaleServiceUrl;

                //instantiate a new instance of the class containing request parameters
                var saleRequest = new ClientSaleRequestData();



                string loginAccount = model_bank.LoginAccount;

                saleRequest.LoginAccount = loginAccount;

                //make sure you set the CallBackUrl property. because after user has completed Payment on IPG page, it will be redirected to the callback url you provided
                //to get you back result of the user Payment on IPG.
                saleRequest.CallBackUrl = model_bank.CallBackUrl;

                //Amount is not used. you should not assign a value to this property.
                saleRequest.Amount = model_bank.Amount;

                //AdditionalData will be saved in Parsian PGW
                saleRequest.AdditionalData = model_bank.AdditionalData;

                //Mobile Number, Telephone or any property of your user to indicate he/she is ordered this payment request.
                saleRequest.Originator = model_bank.Originator;

                //Order Id MUST be UNIQUE at all times. if a duplicated Order Id is received from your request, you will get Status= -112
                // DateTime.Now.Ticks does not ensures uniqueness of OrderId. consider generate OrderId by a Sequence or Identity field in your database.
                saleRequest.OrderId = payment.ID;

                //call appropriate api based on the documentation provided to you.
                responseData = service.SalePaymentRequest(saleRequest);

                paymentStatus = responseData.Status;

                //check Status property of the response object to see if the operation was successful.
                if (responseData.Status == Constants.ParsianPaymentGateway.Successful)
                {
                    //if everything is OK (LoginAccount and your IP address is valid in Parsian PGW), save the token in a data store
                    // to use it for redirectgion of your web site's user to the Parsian IPG (Internet Payment Gateway) page to complete payment.
                    token = responseData.Token;

                }
                else
                {
                    API.Models.Log log = new API.Models.Log();
                    log.WriteErrorLog($"Parsian PGW service call status code : {responseData.Status}");

                }
            }

            //if pgwStatus == 0 and pgwToken > 0, redirect user to the Parsian IPG to continue payment flow.
            if (paymentStatus == Constants.ParsianPaymentGateway.Successful && token > 0L)
            {
                //first, save token to your database to be able to track payment process with your business.
                //after successfully retrieved a token from Parsian PGW, redirect user to Parsian IPG to complete the payment operation.
                var pay = db.Payments.Where(a => a.ID == payment.ID).FirstOrDefault();
                pay.Token = token.ToString();
                db.SaveChanges();
                var redirectUrl = string.Format(ConfigHelper.ParsianIPGPageUrl, responseData.Token);
                return Json(new
                {
                    status = 0,
                    location = redirectUrl
                });
            }
            else
            {
                return Json(new
                {
                    Message = responseData?.Message,
                    status = responseData?.Status,
                    Token = responseData?.Token
                });

            }
        }


    }
}
