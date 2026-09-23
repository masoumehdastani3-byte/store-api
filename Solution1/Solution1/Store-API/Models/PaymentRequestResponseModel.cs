namespace API.Models
{
    public class PaymentRequestResponseModel
    {
        public short? Status { get; set; }
        public string Message { get; set; }
        public long? Token { get; set; }
    }

}