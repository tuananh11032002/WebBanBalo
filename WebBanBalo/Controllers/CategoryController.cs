using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebBanBalo.Dto;
using WebBanBalo.Interface;
using WebBanBalo.Model;

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

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Boolean))]
        public IActionResult CreateProduct([FromBody] CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Ok(_categoryRepository.CreateCategory(category));
        }
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Product))]


        [ProducesResponseType(400)]
        public IActionResult Get(int id)
        {
            if (!_categoryRepository.CategoryExists(id)) return NotFound();
            var product = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(id));
            return Ok(product);
        }
        [HttpGet("{id}/product")]
        public IActionResult GetProductbyCate(int id)
        {
            return Ok(_categoryRepository.GetProductbyCate(id));
        }

        [HttpPut]
        [ProducesResponseType(200, Type = typeof(Boolean))]
        [ProducesResponseType(400)]
        public IActionResult Put( [FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null) return BadRequest(ModelState);
            if (categoryDto.Id == null) return BadRequest(ModelState);

            if (!_categoryRepository.CategoryExists(categoryDto.Id)) return NotFound();

            var category = _mapper.Map<Category>(categoryDto);

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
                return NotFound();
            }

            var productToDelete = _categoryRepository.GetCategory(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_categoryRepository.DeleteCategory(productToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting category");
            }

            return NoContent();
        }
    }
}
