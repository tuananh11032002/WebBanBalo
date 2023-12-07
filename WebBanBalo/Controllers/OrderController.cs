using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.DataClassification;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebBanBalo.Dto;
using WebBanBalo.Interface;
using WebBanBalo.Model;
using WebBanBalo.ModelOther;

namespace WebBanBalo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public OrderController(IOrderRepository orderRepository,IMapper mapper,IProductRepository productRepository, IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }
        [HttpGet("get-sale-revenue")]
        public async Task<IActionResult> getSalesRevenue()
        {
            try
            {
                ValueReturn result = await _orderRepository.getSalesRevenue();
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("AddOrder")]
        [Authorize]
        public IActionResult PostOrder([FromBody] OrderDto orderDto )
        {
            var userIdClaim = User.FindFirst("Id").Value;
            orderDto.UserId = Int32.Parse(userIdClaim);
            return Ok(_orderRepository.AddOrder(_mapper.Map<Order>(orderDto)));
        }

        /// <summary>
        /// Get All order for admin
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet()]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetOrder([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] string? search)
        {


            try
            {
                ValueReturn valueReturn = await _orderRepository.GetOrder(pageIndex, pageSize, search);
                if (valueReturn.Status == true)
                {
                    return Ok(valueReturn.Data);
                }

                return BadRequest(valueReturn.Message);

            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }
        
        
        
        [HttpGet("orderNow")]
        [Authorize]
        public async Task<IActionResult> GetOrderNow()
        {
            try
            {
                var userIdClaim = User.FindFirst("Id").Value;
                if (userIdClaim == null)
                {
                    return Unauthorized();
                }
                else
                {
                    int userId = Int32.Parse(userIdClaim);
                    var user = await _userRepository.getUser(userId);

                    if (user == null)
                    {
                        return NotFound("User không tồn tại");
                    }

                    var order = await _orderRepository.FindOrder(userIdClaim);
                    if (order == null)
                    {
                        var newOrder = new Order
                        {
                            UserId = int.Parse(userIdClaim)

                        };
                        await _orderRepository.AddOrder(newOrder);
                        order = await _orderRepository.FindOrder(userIdClaim);

                    }
                    return Ok(order);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("Product")]
        [Authorize]
        public async Task<IActionResult> AddProductIntoOrder(  [FromBody] OrderItemInputDto orderItemDto)
        {
            try
            {
                var userIdClaim = User.FindFirst("Id").Value;
                if(userIdClaim == null)
                {
                    return Unauthorized();
                }

                ValueReturn result = await _orderRepository.AddProduct(userIdClaim, orderItemDto);
                if (result.Status == true)
                {
                    return Ok();

                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
          

        }


        /// <summary>
        /// Get Order with userId, Paging (ListOrder)
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet("Product/Done")]
        [Authorize]
        public async Task<IActionResult> GetProductOrderDone([FromQuery] string? status,[FromQuery] int pageSize=3, [FromQuery]  int pageIndex=1)
        {
            try
            {
                int  userId = int.Parse(User.FindFirstValue("id"));
                if (userId == null) return StatusCode(500, "Lỗi do server hoặc do token ");
                ValueReturn result = await _orderRepository.getOrderDoneWithUserId(userId, status,pageIndex, pageSize);
                if(result.Status== true)
                {
                    return Ok(result.Data);
                }
                else
                {
                    return BadRequest(result.Message);
                }

            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }



        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderDetailAndCustomerInfo(int orderId)
        {
            try
            {
                ValueReturn result = await _orderRepository.GetOrderDetailAndCustomerInfo(orderId);
                if(result.Status== true)
                {
                    return Ok(result.Data);
                }
                return BadRequest(result.Message);

            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }
       
        


        /// <summary>
        /// Delete Prodduct in Order base on Userid
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteOrderProduct([FromQuery]int productId)
        {
            try
            {
                var userIdClaim = User.FindFirst("Id").Value;
                var order = await _orderRepository.FindOrderWithUserId(userIdClaim);

                if (order != null)
                {
                    var orderItem = _orderRepository.FindOrderItem(productId, order.Id);

                    return Ok(await _orderRepository.Delete(orderItem));
                }
                return BadRequest("Có lỗi gì đó mà tôi không biết. Hãy chờ đợi :)");
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }
        
        
        /// <summary>
        /// Xóa với orderId
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpDelete("{orderId}")]
        [Authorize]
        public async Task<IActionResult> DeleteWithOrderId(int orderId)
        {
            try
            {
                ValueReturn result = await _orderRepository.DeleteWithOrderId(orderId);
                if (result.Status == true)
                {
                    return Ok(result.Message);
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// API comfirm order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        
        [HttpPut("payments")]
        [Authorize]
        public async Task<IActionResult> UpdateOrder([FromBody] OrderUpdateModel order)
        {

            try
            {
                var userId = int.Parse(User.FindFirst("Id").Value);
                ValueReturn result = await _orderRepository.ConfirmOrder(order, userId);
                if (result.Status == true)
                {
                    return Ok();
                }
                else {
                    return BadRequest(result.Message);
                }

            }catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPut("cancel-order/{orderId}")]
        //[Authorize]
        public async Task<IActionResult> HuyDon(int orderId)
        {

            try
            {
                var result = await _orderRepository.CancelOrder(orderId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateStatusOrder(int orderId)
        {
            try
            {
                ValueReturn result = await _orderRepository.UpdateStatus(orderId);
                if (result.Status == true)
                {
                    return Ok("Cập nhật thành công ");
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("coupon")]
        public async Task<IActionResult> getCoupon([FromQuery] string coupon)
        {
            try
            {
                ValueReturn result = _orderRepository.GetCoupon(coupon);
                if(result.Status == true)
                {
                    return Ok(result.Data);

                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }

}
