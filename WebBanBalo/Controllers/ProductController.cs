using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;
using WebBanBalo.Dto;
using WebBanBalo.Interface;
using WebBanBalo.Migrations;
using WebBanBalo.Model;
using WebBanBalo.Repository;

namespace WebBanBalo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ProductController(IProductRepository productRepository, IMapper mapper,ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _categoryRepository=categoryRepository;

        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Collection<ProductDto>>))]

        public IActionResult Get()
        {
            return Ok(_mapper.Map<List<ProductDto>>(_productRepository.GetProducts()));
        }
        [HttpGet("Search")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Collection<ProductDto>>))]

        public IActionResult GetProductSearch([FromQuery] string data)
        {
            return Ok(_mapper.Map<List<ProductDto>>(_productRepository.GetProductSearch(data)));
        }


        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Boolean))]
        public IActionResult CreateProduct([FromBody] ProductDto productDto, [FromQuery] int categoryId)
        {

            var product = _mapper.Map<Product>(productDto);
            if (categoryId == null) return BadRequest(ModelState);
            if (!_categoryRepository.CategoryExists(categoryId)) return NotFound();
            var category = _categoryRepository.GetCategory(categoryId);
            ProductCategory productCategory = new ProductCategory() {
                Product = product,
                Category = category,
                
            };

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            
            return Ok(_productRepository.CreateProduct(product, productCategory));
           
        }
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Product))]


        [ProducesResponseType(400)]
        public IActionResult Get(int id)
        {
            if (!_productRepository.ProductExists(id)) return NotFound();
            var product = _mapper.Map<Product>(_productRepository.GetProduct(id));
            return Ok(product);
        }
        [HttpPut]
        [ProducesResponseType(200, Type = typeof(Boolean))]
        [ProducesResponseType(400)]
        public IActionResult Put( [FromBody] ProductDto productDto)
        {
            if (productDto == null) return BadRequest(ModelState);
            if (productDto.Id == null) return BadRequest(ModelState);
            if (!_productRepository.ProductExists(productDto.Id)) return NotFound();
            var product = _mapper.Map<Product>(productDto);
            if (!_productRepository.UpdateProduct(product)) return StatusCode(500, "Something Wrong on Server");
            return NoContent();

        }
        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCategory(int id)
        {
            if (!_productRepository.ProductExists(id))
            {
                return NotFound();
            }

            var productToDelete = _productRepository.GetProduct(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_productRepository.DeleteProduct(productToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting category");
            }

            return NoContent();
        }
    }
}
