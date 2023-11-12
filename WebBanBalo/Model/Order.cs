using WebBanBalo.ModelOther;

namespace WebBanBalo.Model
{
    
   
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public float TotalAmount { get; set; }
        public float FeeShip { set; get; }
        public float Discount { set; get; }
        public float GrandTotal
        {
            get { return TotalAmount + FeeShip - Discount; }
        }
        public bool Done { get; set; }
        public DateTime? FinishedAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public Users User { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }

        public string? OrderNote { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public string OrderStatus { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public Order()
        {
         
            TotalAmount = 0;
            FeeShip = 0;
            Discount = 0;
            Done = false;
            CreatedAt = DateTime.Now;
            
            ShippingAddress = "";
            BillingAddress = "";
            OrderNote = null;
            PaymentMethod = PaymentMethod.COD;
            OrderStatus = "";
        }
    }

}
