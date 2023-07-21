using Microsoft.AspNetCore.Mvc;
using WebBanBalo.Model;

namespace WebBanBalo.Migrations
{
    public interface IOrderRepository
    {
        bool AddOrder(Order order);
        bool AddProduct(OrderItem orderItem);
        ICollection<Order> GetOrder();
        Order FindOrder();
        Order GetOrder(int id);
        List<OrderItemDto> getProductOrder(int orderid);
        OrderItem FindOrderItem(int productId, int orderId);
        bool Delete(OrderItem order);
    }
}
