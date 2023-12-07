using System.ComponentModel.DataAnnotations;
using WebBanBalo.ModelOther;

namespace WebBanBalo.Model
{

    public enum OrderStatus {

        Cancelled,
        ReadytoPickup,
        Dispatched,
        OutforDelivery,
        Delivered,

    }
    public enum PaymentStatus
    {
        Pending, Paid, Failed, Cancelled
    }


    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PaymentId { get; set; }
        public float TotalAmount { get; set; }
        public float FeeShip { set; get; }
        public float Discount { set; get; }
        public float GrandTotal
        {
            get { return TotalAmount + FeeShip - Discount; }
        }
        [MaxLength(11)]
        public string CustomerPhone { set; get; }
        public string CustomerName { set; get; }
        public bool Done { get; set; }
        public DateTime? FinishedAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public Users User { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }

        public string? OrderNote { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

        public List<OrderStatusUpdate> OrderStatusUpdates { get; set; }

        public ICollection<Payment> Payment { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
        public Order()
        {
         
            TotalAmount = 0;
            FeeShip = 0;
            Discount = 0;
            Done = false;
            CreatedAt = DateTime.Now;
            CustomerName = string.Empty;
            ShippingAddress = "";
            BillingAddress = "";
            OrderNote = null;
            PaymentMethod = PaymentMethod.COD;
            OrderStatusUpdates = new List<OrderStatusUpdate>();
            CustomerPhone = string.Empty;
            PaymentStatus = PaymentStatus.Pending;

        }
    }

}
