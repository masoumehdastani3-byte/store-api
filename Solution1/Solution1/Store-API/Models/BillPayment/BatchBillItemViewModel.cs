using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class BatchBillItemViewModel
    {
        [Required]
        [MaxLength(30)]
        public string LoginAccount { get; set; }

        [Required]
        [MaxLength(13)]
        public string BillId { get; set; }

        [Required]
        [MaxLength(13)]
        public string PayId { get; set; }


        public long OrderId { get; set; }

        public string AdditionalData { get; set; }
    }
}