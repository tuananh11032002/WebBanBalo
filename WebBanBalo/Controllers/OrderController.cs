using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.DataClassification;
using WebBanBalo.Dto;
using WebBanBalo.Interface;
using WebBanBalo.Migrations;
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

        public IActionResult PostOrder([FromBody] OrderDto orderDto )
        {

            return Ok(_orderRepository.AddOrder(_mapper.Map<Order>(orderDto)));
        }
        [HttpGet("")]
        public IActionResult GetOrder()
        {
            return Ok(_orderRepository.GetOrder());
        }
        [HttpPost("Product/{productid}")]
        public IActionResult AddProductIntoOrder( int productid, [FromQuery] int userid, [FromBody] OrderItemInputDto orderItemDto)
        {
            var orderLast = _orderRepository.FindOrder();
            if (orderLast!=null||orderLast.Done==false)
            {
                var product = _productRepository.GetProduct(productid);
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
                var user = _userRepository.getUser(userid);
                if (user == null) return NotFound("User does not exist");
                Order order = new Order
                {
                    UserId = userid
                };
                _orderRepository.AddOrder(order);

                return Ok(AddProductIntoOrder(productid,userid,orderItemDto));
            }

        }
        [HttpGet("Product")]
        public IActionResult GetProductOrder()
        {

            var order = _orderRepository.FindOrder();
            if (order == null) return NotFound();
            return Ok(_orderRepository.getProductOrder(order.Id));
        }
        [HttpDelete("")]
        public IActionResult DeleteOrderProduct([FromQuery]int productId)
        {
            var order = _orderRepository.FindOrder();
            if (order != null)
            {
                var orderItem = _orderRepository.FindOrderItem(productId, order.Id);

                return Ok(_orderRepository.Delete(orderItem));
            }
            return BadRequest();
        }
    }

}
