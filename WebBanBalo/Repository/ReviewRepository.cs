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
                
               var orderItem= await _dataContext.OrderItem.FirstOrDefaultAsync(p=>p.ProductId==reviewInput.ProductId && p.OrderId==reviewInput.OrderId);
                if(orderItem == null)
                {
                    return new ValueReturn
                    {
                        Status = false,
                        Message = "Sản phẩm không tồn tại trong đơn hàng naỳ ",
                    };
                }
                else
                {
                    if (orderItem.IsReview == true)
                    {
                        return new ValueReturn
                        {
                            Status = false,
                            Message = "Đơn hàng này đã đánh giá rồi"
                        };
                    }
                    else
                    {
                        Review review = new Review()
                        {
                            DatePosted = DateTime.Now,
                            Comment = reviewInput.Comment,
                            UserId = reviewInput.UserId,
                            Rating = reviewInput.Rating,
                            ProductId = reviewInput.ProductId,
                        };
                        await _dataContext.Review.AddAsync(review);
                        if (await _dataContext.SaveChangesAsync() > 0)
                        {
                            orderItem.IsReview = true;
                            _dataContext.OrderItem.Update(orderItem);
                            await _dataContext.SaveChangesAsync();
                        }

                        return new ValueReturn
                        {
                            Status = true,
                            Message = "Tạo Review thành công "

                        };
                    }
                }
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
