using Microsoft.EntityFrameworkCore;
using WebBanBalo.Data;
using WebBanBalo.Interface;
using WebBanBalo.Model;
using WebBanBalo.ModelOther;

namespace WebBanBalo.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext _dataContext;
        public ReviewRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<List<Review>> getAll()
        {
            return _dataContext.Review.ToList();
        }
        public async Task<ValueReturn> CreateReview(ReviewCreateModel reviewInput)
        {
            
            try
            {
                
                Review review = new Review()
                {
                    DatePosted = DateTime.Now,
                    Comment = reviewInput.Comment,
                    UserId = reviewInput.UserId,
                    Rating = reviewInput.Rating,
                    ProductId= reviewInput.ProductId,
                    UserName= reviewInput.UserId.ToString(),
                };
                await _dataContext.Review.AddAsync(review);
                if(await _dataContext.SaveChangesAsync() > 0)
                {
                    var orderItem = await _dataContext.OrderItem.Where(p => p.ProductId == reviewInput.ProductId && p.OrderId == reviewInput.OrderId).FirstOrDefaultAsync();
                    orderItem.IsReview = true;
                    _dataContext.OrderItem.Update(orderItem);
                    await _dataContext.SaveChangesAsync();
                }

                return new ValueReturn
                {
                    Status = true,
                    Message = "Tạo Review thành công "

                };

            }catch (Exception ex)
            {
                return new ValueReturn
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }
    }
}
