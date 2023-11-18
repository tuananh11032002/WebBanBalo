using WebBanBalo.Model;
using WebBanBalo.ModelOther;

namespace WebBanBalo.Interface
{
    public interface IReviewRepository
    {
        Task<ValueReturn> CreateReview(ReviewCreateModel reviewInput);
        Task<List<Review>> getAll();
    }
}
