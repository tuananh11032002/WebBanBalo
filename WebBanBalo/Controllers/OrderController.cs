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
        [HttpPost("AddOrder")]
        [Authorize]
        public IActionResult PostOrder([FromBody] OrderDto orderDto )
        {
            var userIdClaim = User.FindFirst("Id").Value;
            orderDto.UserId = Int32.Parse(userIdClaim);
            return Ok(_orderRepository.AddOrder(_mapper.Map<Order>(orderDto)));
        }
        [HttpGet("")]
        [Authorize(Roles ="admin")]
        public IActionResult GetOrder()
        {
            return Ok(_orderRepository.GetOrder());
        }
        [HttpGet("orderNow")]
        [Authorize]
        public IActionResult GetOrderNow()
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"];

            var userIdClaim = User.FindFirst("Id").Value;
          
            return Ok(_orderRepository.FindOrder(userIdClaim));
        }
        [HttpPost("Product/{productid}")]
        [Authorize]
        public async Task<IActionResult> AddProductIntoOrder( int productid, [FromBody] OrderItemInputDto orderItemDto)
        {
            var userIdClaim = User.FindFirst("Id").Value;
            var orderLast = _orderRepository.FindOrder(userIdClaim);
            if (orderLast!=null && orderLast.Done==false)
            {
                var product = await _productRepository.GetProductByIdAsync(productid);
                if (product == null) return NotFound("Product dont exist");
                else
                {
                    OrderItem orderItem = new OrderItem
                    {
                        Product = product,
                        Order = orderLast,
                        Price = orderItemDto.Price,
                        Quantity = orderItemDto.Quantity,
                    };
                    return Ok(_orderRepository.AddProduct(orderItem));
                }
            }
            else
            {
                var user = _userRepository.getUser(Int32.Parse(userIdClaim));
                if (user == null) return NotFound("User does not exist");
                Order order = new Order
                {
                    UserId = Int32.Parse(userIdClaim)
                };
                _orderRepository.AddOrder(order);

                return Ok(AddProductIntoOrder(productid, orderItemDto));
            }

        }
        [HttpGet("Product")]
        [Authorize]
        public IActionResult GetProductOrder()
        {
          
            var userIdClaim = User.FindFirst("Id");
            if (userIdClaim != null)
            {
                var userId  = userIdClaim.Value;
                var order = _orderRepository.FindOrder(userId);
                if (order == null) return NotFound();
                return Ok(_orderRepository.getProductOrder(order.Id));
            }
            else
            {
                return Unauthorized();
            }
            
        }
        [HttpDelete("")]
        [Authorize]
        public IActionResult DeleteOrderProduct([FromQuery]int productId)
        {
            var userIdClaim = User.FindFirst("Id").Value;
            var order = _orderRepository.FindOrder(userIdClaim);
            
            if (order != null)
            {
                var orderItem = _orderRepository.FindOrderItem(productId, order.Id);

                return Ok(_orderRepository.Delete(orderItem));
            }
            return BadRequest();
        }
    }

}
