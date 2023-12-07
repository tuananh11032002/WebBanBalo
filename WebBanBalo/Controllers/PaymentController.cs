using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebBanBalo.Helper;
using WebBanBalo.Interface;
using WebBanBalo.Model;
using WebBanBalo.ModelOther;
using WebBanBalo.Repository;

namespace WebBanBalo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;
        public PaymentController(IOptions<VnPayConfig> vnPayConfig, IOrderRepository orderRepository, IPaymentRepository paymentRepository)
        {
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
        }
        [HttpPost]
        public async Task<IActionResult> Payment([FromBody] OrderUpdateModel order)
        {

            try
            {
                ValueReturn valueReturn = await _paymentRepository.Payment(order);
                if (valueReturn.Status)
                {
                    return Ok(valueReturn.Data);

                }
                else
                {
                    return BadRequest(valueReturn.Message);
                }
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("vnpay_return")]
        public async Task<IActionResult> VnPayReturn([FromQuery] VnPayResponseModel vnPayResponse)
        {
            ValueReturn result = await _paymentRepository.SavePayment(vnPayResponse);
            if (result.Status)
            {
                return Ok("Success");

            }
            else
            {
                return BadRequest(result.Message);
            }
        }

    }
}
