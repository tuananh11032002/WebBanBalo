using System.ComponentModel.DataAnnotations;
using WebBanBalo.Model;

namespace WebBanBalo.ModelOther
{


    public enum PaymentMethod
    {
        COD,
        Momo
    }
    public class OrderUpdateModel
    {
        [Required]
        public int Id { get; set; }
        public string? Coupon { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public string? OrderNote { get; set; }

        [EnumDataType(typeof(PaymentMethod))]
        public PaymentMethod PaymentMethod { get; set; }
        public string CustomerName { get; set; }    
        public string CustomerPhone { set;get; }
    }
}
