using Microsoft.AspNetCore.Mvc;
using WebBanBalo.Model;
using WebBanBalo.ModelOther;

namespace WebBanBalo.Interface
{
    public interface IOrderRepository
    {
        Task<bool> AddOrder(Order order);
        ValueReturn AddProduct(OrderItem orderItem);
        Task<ValueReturn> GetOrder(int pageIndex, int pageSize, string? search);
        Task<object> FindOrder(string userId);
        Task<Order> FindOrderWithUserId(string userId);

        Order GetOrder(int id);
        OrderItem FindOrderItem(int productId, int orderId);
        Task<bool> Delete(OrderItem order);
        Task<ValueReturn> ConfirmOrder(OrderUpdateModel order,int userId);
        Task<ValueReturn> getOrderDoneWithUserId(int userId, int pageIndex, int pageSize);
        Task<ValueReturn> DeleteWithOrderId(int orderId);
        Task<ValueReturn> GetOrderDetailAndCustomerInfo(int orderId);
        Task<ValueReturn> UpdateStatus(int orderId);
    }
}
