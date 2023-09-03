using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection.Metadata.Ecma335;
using WebBanBalo.Data;
using WebBanBalo.Dto;
using WebBanBalo.Interface;
using WebBanBalo.Model;

namespace WebBanBalo.Repository
{
    public class OrderRepository:IOrderRepository
    {
        private readonly DataContext _dataContext;

        public OrderRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public bool AddOrder(Order order)
        {
            _dataContext.Order.Add(order);
           
            return Save();
        }

        private bool Save()
        {
            var saved = _dataContext.SaveChanges();
            return saved > 0 ? true : false;
        }
        public Order FindOrder(string userId)
        {
            var order = _dataContext.Order.Where(p=>p.UserId==Int32.Parse(userId)).FirstOrDefault();
            return order;
        }
        public ICollection<Order> GetOrder()
        {
            return _dataContext.Order.ToList();
        }

        public bool AddProduct(OrderItem orderItem)
        {
            var check = _dataContext.OrderItem.Where(p=>p.ProductId == orderItem.Product.Id && p.OrderId==orderItem.Order.Id).FirstOrDefault();
            var order = _dataContext.Order.Where(p => p.Id == orderItem.Order.Id).FirstOrDefault();
            var orderProductList = _dataContext.OrderItem.Where(p=>p.OrderId == orderItem.Order.Id).ToList();
            
            if (check != null)
            {

                check.Quantity = orderItem.Quantity;
                float totalPrice = 0;
                foreach(var t in orderProductList)
                {
                    totalPrice += t.Price*t.Quantity;
                }
                order.TotalAmount = totalPrice;

            }
            else
            {
                _dataContext.Add(orderItem);
                _dataContext.SaveChanges();
                var orderItemTemp = _dataContext.OrderItem.Where(p => p.ProductId == orderItem.Product.Id && p.OrderId == orderItem.Order.Id).FirstOrDefault();
                var orderProductListTemp = _dataContext.OrderItem.Where(p => p.OrderId == orderItem.Order.Id).ToList();
                orderItemTemp.Quantity = orderItem.Quantity;
                float totalPrice = 0;
                foreach (var t in orderProductListTemp)
                {
                    totalPrice += t.Price * t.Quantity;
                }
                order.TotalAmount = totalPrice;

            }

            return Save();
        }

        public Order GetOrder(int id)
        {
            return _dataContext.Order.Where(p => p.Id == id).FirstOrDefault();
        }

        public List<OrderItemDto> getProductOrder(int orderid)
        {
            return _dataContext.OrderItem.Where(p => p.OrderId == orderid).Select(p => new OrderItemDto { Product= p.Product, Price=p.Price, Quantity=p.Quantity}).ToList();

        }

        public OrderItem FindOrderItem(int productId, int orderId)
        {
            var orderItem = _dataContext.OrderItem.Where(p => p.ProductId== productId && p.OrderId==orderId).FirstOrDefault();
            return orderItem;
        }

        public bool Delete(OrderItem orderItem)
        {
            

            _dataContext.Remove(orderItem);
            _dataContext.SaveChanges();
            var order = _dataContext.Order.Where(p => p.Id == orderItem.OrderId).FirstOrDefault();

            var listProduct = _dataContext.OrderItem.Where(p => p.OrderId == orderItem.OrderId).ToList();

            float result = 0;

            foreach (var item in listProduct)
            {
                result += item.Price*item.Quantity;
            }
            order.TotalAmount = result;

            
            return Save();
        }
    }
}
