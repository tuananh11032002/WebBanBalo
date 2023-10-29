using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WebBanBalo.Data;
using WebBanBalo.Dto;
using WebBanBalo.Interface;
using WebBanBalo.Migrations;
using WebBanBalo.Model;
using WebBanBalo.ModelOther;
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
        private  DataContext _dataContext;

        public ProductController(DataContext dataContext,IProductRepository productRepository, IMapper mapper,ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _categoryRepository=categoryRepository;

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
             [FromQuery] string? search,
             [FromQuery] string? orderBy,
             [FromQuery] int? page = 1,
             [FromQuery] int? pageSize = 10)
        {
            var products = await _productRepository.GetProductsAsync(search, orderBy, page ?? 1, pageSize?? 10);
            return products.ToList();
        }
        [HttpPost("create-product-with-images")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductInputModel productInput)
        {
            if (productInput.ImageFiles != null && productInput.ImageFiles.Count > 0)
            {
                return Ok( await _productRepository.CreateProductWithImage(productInput));
            }

            return BadRequest("Hình ảnh không hợp lệ.");
        }

        [HttpPut]
        [ProducesResponseType(200, Type = typeof(Boolean))]
        [ProducesResponseType(400)]
        public IActionResult Put( [FromBody] ProductDto productDto)
        {
            if (productDto == null) return BadRequest(ModelState);
            if (!_productRepository.ProductExists(productDto.Id)) return NotFound();
            var product = _mapper.Map<Product>(productDto);
            if (!_productRepository.UpdateProduct(product)) return StatusCode(500, "Something Wrong on Server");
            return NoContent();

        }
        
        
        
        
        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (!_productRepository.ProductExists(id))
            {
                return NotFound();
            }

            var productToDelete = await _productRepository.GetProductByIdAsync(id);

            if (productToDelete == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_productRepository.DeleteProduct(productToDelete))
            {
                return Ok("Xóa thành công");
            }
            else
            {
                ModelState.AddModelError("", "Something went wrong deleting category");
                return BadRequest(ModelState);
            }
        }

    }
}
