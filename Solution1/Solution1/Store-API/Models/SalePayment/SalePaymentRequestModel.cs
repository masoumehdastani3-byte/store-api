using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class SalePaymentRequestModel
    {
        [Display(Name = "Login Account (merchant PIN)")]
        [MinLength(10)]
        public string LoginAccount { get; set; }

        [Required]
        public long Amount { get; set; }

        [MaxLength(250)]
        public virtual string AdditionalData { get; set; }


        [Display(Name = "Confirm after payment?")]
        public bool ConfirmAfterPayment { get; set; }

        [Display(Name = "Originator|Mobile Number")]
        public string Originator { get; set; }

        public string CallBackUrl { get; set; }
    }
}