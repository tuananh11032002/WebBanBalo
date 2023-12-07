using WebBanBalo.ModelOther;

namespace WebBanBalo.Interface
{
    public interface IPaymentRepository
    {
        Task<ValueReturn> Payment(OrderUpdateModel orderInput);
        Task<ValueReturn> SavePayment(VnPayResponseModel vnPayResponse);
    }
}
