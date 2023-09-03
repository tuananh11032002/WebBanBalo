using Microsoft.AspNetCore.Mvc;
using WebBanBalo.Model;

namespace WebBanBalo.Interface
{
    public interface IOrderRepository
    {
        bool AddOrder(Order order);
        bool AddProduct(OrderItem orderItem);
        ICollection<Order> GetOrder();
        Order FindOrder(string userId);
        Order GetOrder(int id);
        List<OrderItemDto> getProductOrder(int orderid);
        OrderItem FindOrderItem(int productId, int orderId);
        bool Delete(OrderItem order);
    }
}
