using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebBanBalo.Data;
using WebBanBalo.Helper;
using WebBanBalo.Interface;
using WebBanBalo.Model;
using WebBanBalo.ModelOther;
using static Azure.Core.HttpHeader;

namespace WebBanBalo.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly VnPayConfig _vnPayConfig;
        private readonly DataContext _dataContext;
        private readonly IOrderRepository _orderRepository;

        public PaymentRepository(IOptions<VnPayConfig> vnPayConfig, DataContext dataContext, IOrderRepository orderRepository)
        {
            _vnPayConfig = vnPayConfig.Value;
            _dataContext = dataContext;
            _orderRepository = orderRepository;
        }
        public async Task<ValueReturn> Payment(OrderUpdateModel orderInput)
        {



            var orderUpdate = _dataContext.Order.Include(p => p.OrderItems).ThenInclude(p => p.Product).FirstOrDefault(p => p.Id == orderInput.Id && p.Done == false);
            if(orderUpdate == null)
            {
                return new ValueReturn { Status = false, Message= " ĐƠN HÀNG GIẢ" };

            }

            //check quantity is valid
            var orderItem = await _dataContext.Order.Where(p => p.Id == orderInput.Id).Select(px => new
            {
                OrderId = px.Id,
                ProductList = px.OrderItems.Select(py => new { Name = py.Product.Name, SoLuong = py.Product.Soluong, SoLuongCan = py.Quantity }).ToList(),


            }).FirstOrDefaultAsync();
            if (orderItem != null)
            {
                string message = "";
                foreach (var item in orderItem.ProductList)
                {
                    if (item.SoLuongCan > item.SoLuong)
                    {
                        message = message + $"\n {item.Name} không đủ hàng so với nhu cầu của bạn ";

                    }
                }
                if (!message.IsNullOrEmpty())
                {
                    return new ValueReturn()
                    {
                        Status = false,
                        Message = message
                    };
                }
            }
            var discount= _orderRepository.GetCoupon(orderInput.Coupon);

            float tong = 0;
            foreach (var item in orderUpdate.OrderItems)
            {
                item.Price = item.Product.Price;
                item.Discount = item.Product.Discount;
                tong += (item.Product.Price - item.Product.Discount) *item.Quantity;


            }
            var discountLast = discount.Status ? Convert.ToSingle(discount.Data) : 0;
            tong = tong + 30000 - discountLast;



            var vnp_Url = _vnPayConfig.VnpUrl;
            var vnp_TmnCode = _vnPayConfig.VnpTmnCode;
            var vnp_HashSecret = _vnPayConfig.VnpHashSecret;
            var vnp_Returnurl = _vnPayConfig.VnpReturnUrl;
            OrderInfo order = new OrderInfo();
            order.OrderId = orderInput.Id;
            
            order.Amount = (long)tong;
            order.Status = "0";
            order.CreatedDate = DateTime.Now;
            VnPayLibrary vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString());
            vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString());
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return new ValueReturn { Status= true, Data= paymentUrl };

        }

        public async Task<ValueReturn> SavePayment(VnPayResponseModel vnPayResponse)
        {
            try
            {
                var payment = new Payment {
                    CreateAt = DateTime.Now,
                    OrderId = (int)vnPayResponse.vnp_TxnRef,
                    vnp_Amount = vnPayResponse.vnp_Amount,
                    vnp_BankCode = vnPayResponse.vnp_BankCode,
                    vnp_BankTranNo = vnPayResponse.vnp_BankTranNo,
                    vnp_CardType = vnPayResponse?.vnp_CardType,
                    vnp_OrderInfo =vnPayResponse?.vnp_OrderInfo,
                    vnp_PayDate = vnPayResponse.vnp_PayDate,
                    vnp_ResponseCode = vnPayResponse.vnp_ResponseCode,
                    vnp_TmnCode = vnPayResponse.vnp_TmnCode,
                    vnp_TransactionNo = vnPayResponse.vnp_TransactionNo,
                    vnp_TransactionStatus = vnPayResponse.vnp_TransactionStatus,
                    vnp_TxnRef = vnPayResponse.vnp_TxnRef,
                    vnp_SecureHash = vnPayResponse.vnp_SecureHash,


                };
                await _dataContext.Payment.AddAsync(payment);
                await _dataContext.SaveChangesAsync();

                return new ValueReturn
                {
                   
                    Status = true,
                };
            }catch (Exception ex)
            {
                return new ValueReturn
                {
                    Status = false,
                    Message = ex.Message,
                };
            }
        }
    }
}
