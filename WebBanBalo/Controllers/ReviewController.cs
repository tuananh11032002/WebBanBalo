using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebBanBalo.Interface;
using WebBanBalo.ModelOther;

namespace WebBanBalo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        public ReviewController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody]ReviewCreateModel reviewInput)
        {
            try
            {
                ValueReturn result = await _reviewRepository.CreateReview(reviewInput);
                if (result.Status == true)
                {
                    return Ok(result.Message);
                }
                else return BadRequest(result.Message);
            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(_reviewRepository.getAll());
        }
    }
}
