using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class PaymentCallBackViewModel : PaymentCallbackModel
    {
        [Display(Name = "کد پاسخ سرویس تایید تراکنش")]
        [DisplayFormat(NullDisplayText = "NULL")]
        public short? ConfirmResponseStatus { get; set; }
    }
}