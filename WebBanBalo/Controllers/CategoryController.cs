using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebBanBalo.Dto;
using WebBanBalo.Interface;
using WebBanBalo.Model;
using WebBanBalo.ModelOther;

namespace WebBanBalo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;

        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]

        public IActionResult Get()
        {
            return Ok(_mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories()));
        }
        [HttpGet("admin")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]

        public IActionResult GetForAdmin()
        {
            return Ok(_categoryRepository.GetCategoriesForAdmin());
        }


        /// <summary>
        /// Api create Category
        /// </summary>
        /// <param name="categoryDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Boolean))]

        
        public async Task<IActionResult> CreateCategory([FromForm] CategoryCreateModel category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                return Ok(await _categoryRepository.CreateCategory(category));
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex);
            }
        }
        
        
        /// <summary>
        /// Api Get Category by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Product))]


        [ProducesResponseType(400)]
        public IActionResult Get(int id)
        {
            if (!_categoryRepository.CategoryExists(id)) return NotFound();
            var product = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(id));
            return Ok(product);
        }
 

        [HttpPut]
        [ProducesResponseType(200, Type = typeof(Boolean))]
        [ProducesResponseType(400)]
        public IActionResult Put( [FromForm] CategoryUpdateModel category)
        {
            if (category.Id == null) return BadRequest(ModelState);

            if (!_categoryRepository.CategoryExists(category.Id)) return NotFound("Category không tồn tại");


            if (!_categoryRepository.UpdateCategory(category)) return StatusCode(500, "Something Wrong on Server");
            return Ok(new {status= "success", message= "Đã cập nhật"});

        }
        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCategory(int id)
        {
            if (!_categoryRepository.CategoryExists(id))
            {
                return NotFound("Không timf thấy sản phẩm");
            }


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_categoryRepository.DeleteCategory(id))
            {
                ModelState.AddModelError("", "Something went wrong deleting category");
            }

            return NoContent();
        }
    }
}
