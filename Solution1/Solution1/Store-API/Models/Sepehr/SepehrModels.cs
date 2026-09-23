namespace API.Models.Sepehr
{
    public class SepehrGetTokenRequest
    {
        public long TerminalID { get; set; }
        public long Amount { get; set; }
        public long InvoiceID { get; set; }
        public string callbackURL { get; set; }
        public string payload { get; set; }
    }

    public class SepehrGetTokenResponse
    {
        public int Status { get; set; }
        public string AccessToken { get; set; }
        public string Accesstoken { get; set; }
        public string Message { get; set; }

        public string ResolvedToken
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(AccessToken))
                    return AccessToken;
                return Accesstoken;
            }
        }
    }

    public class SepehrAdviceRequest
    {
        public string digitalreceipt { get; set; }
        public long Tid { get; set; }
        public string InvoiceID { get; set; }
    }

    public class SepehrAdviceResponse
    {
        public string Status { get; set; }
        public string ReturnId { get; set; }
        public string Message { get; set; }
    }

    public class SepehrCallbackModel
    {
        public string respcode { get; set; }
        public string respmsg { get; set; }
        public string amount { get; set; }
        public string invoiceid { get; set; }
        public string terminalid { get; set; }
        public string tracenumber { get; set; }
        public string rrn { get; set; }
        public string datepaid { get; set; }
        public string digitalreceipt { get; set; }
        public string cardnumber { get; set; }
        public string issuerbank { get; set; }
    }
}
