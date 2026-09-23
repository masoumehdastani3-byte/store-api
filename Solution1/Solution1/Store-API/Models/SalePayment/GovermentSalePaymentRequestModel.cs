using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class GovermentSalePaymentRequestModel : SalePaymentRequestModel
    {
        [MaxLength(30)]
        [MinLength(30)]
        [Required]
        [Display(Name = "Goverment ID")]
        [RegularExpression(@"^\d{30}$", ErrorMessage = "Invalid Goverment ID")]
        public override string AdditionalData { get; set; }
    }
}