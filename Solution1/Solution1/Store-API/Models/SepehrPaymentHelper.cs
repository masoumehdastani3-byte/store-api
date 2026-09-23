using API.Models.Sepehr;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Web.Configuration;

namespace API.Models
{
    public static class SepehrPaymentHelper
    {
        public static string GetTokenUrl
        {
            get
            {
                var v = WebConfigurationManager.AppSettings["SepehrGetTokenUrl"];
                return string.IsNullOrWhiteSpace(v)
                    ? "https://sepehr.shaparak.ir/Rest/V1/PeymentApi/GetToken"
                    : v;
            }
        }

        public static string AdviceUrl
        {
            get
            {
                var v = WebConfigurationManager.AppSettings["SepehrAdviceUrl"];
                return string.IsNullOrWhiteSpace(v)
                    ? "https://sepehr.shaparak.ir/Rest/V1/PeymentApi/Advice"
                    : v;
            }
        }

        public static string PayPageUrl
        {
            get
            {
                var v = WebConfigurationManager.AppSettings["SepehrPayPageUrl"];
                return string.IsNullOrWhiteSpace(v)
                    ? "https://sepehr.shaparak.ir/Payment/Pay?token={0}&terminalId={1}"
                    : v;
            }
        }

        public static string DefaultTerminalId
        {
            get
            {
                var v = WebConfigurationManager.AppSettings["SepehrTerminalId"];
                return string.IsNullOrWhiteSpace(v) ? "99380285" : v;
            }
        }

        public static string DefaultCallbackUrl
        {
            get
            {
                return WebConfigurationManager.AppSettings["SepehrCallbackUrl"];
            }
        }

        public static SepehrGetTokenResponse GetToken(SepehrGetTokenRequest request)
        {
            var log = new Log();
            var json = JsonConvert.SerializeObject(request);
            log.WriteErrorLog("Sepehr GetToken request: " + json);

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(60);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PostAsync(GetTokenUrl, content).Result;
                var body = response.Content.ReadAsStringAsync().Result;
                log.WriteErrorLog("Sepehr GetToken response: " + body);

                var result = new SepehrGetTokenResponse();
                try
                {
                    var jo = JObject.Parse(body);
                    result.Status = jo["Status"] != null ? Convert.ToInt32(jo["Status"]) : -1;
                    result.AccessToken = jo["AccessToken"]?.ToString() ?? jo["Accesstoken"]?.ToString() ?? jo["Token"]?.ToString();
                    result.Message = jo["Message"]?.ToString() ?? jo["message"]?.ToString();
                }
                catch (Exception ex)
                {
                    result.Status = -1;
                    result.Message = "پاسخ نامعتبر از درگاه سپهر: " + ex.Message;
                }
                return result;
            }
        }

        public static SepehrAdviceResponse Advice(SepehrAdviceRequest request)
        {
            var log = new Log();
            var json = JsonConvert.SerializeObject(request);
            log.WriteErrorLog("Sepehr Advice request: " + json);

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(60);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = client.PostAsync(AdviceUrl, content).Result;
                var body = response.Content.ReadAsStringAsync().Result;
                log.WriteErrorLog("Sepehr Advice response: " + body);

                var result = new SepehrAdviceResponse();
                try
                {
                    var jo = JObject.Parse(body);
                    result.Status = jo["Status"]?.ToString();
                    result.ReturnId = jo["ReturnId"]?.ToString() ?? jo["ReturnID"]?.ToString();
                    result.Message = jo["Message"]?.ToString();
                }
                catch (Exception ex)
                {
                    result.Status = "Error";
                    result.Message = ex.Message;
                }
                return result;
            }
        }

        public static string BuildRedirectUrl(string token, long terminalId)
        {
            return string.Format(PayPageUrl, Uri.EscapeDataString(token), terminalId);
        }

        public static bool IsSaderatBankName(string bankName)
        {
            var n = bankName.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(bankName))
                return false;
            else if(n.Contains("sepehr") || n.Contains("سپهر") || n.Contains("saderat") || n.Contains("sepehr") || n.Contains("صادرات"))
                return true;
            return false;
        }
    }
}
