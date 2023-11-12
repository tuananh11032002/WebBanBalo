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
        private DataContext _dataContext;

        public ProductController(DataContext dataContext, IProductRepository productRepository, IMapper mapper, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;

        }


        
        
        [HttpGet("get-with-category")]
        public async Task<IActionResult> GetFilteredProducts(
        [FromQuery] int categoryId = -1,
        [FromQuery] string? search = "",
        [FromQuery] string? orderBy = "",
        [FromQuery] int pageSize = 10,
        [FromQuery] int pageIndex = 1)
        {
            var result = await _productRepository.GetFilteredProducts(categoryId, search, orderBy, pageSize, pageIndex);

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id )
        {
            var result = await _productRepository.GetProductByIdAsync(id);
           
            return Ok(_mapper.Map<ProductDto>(result));
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetProducts(
             [FromQuery] string? search,
             [FromQuery] string? orderBy,
             [FromQuery] int? categoryId,
             [FromQuery] string? stock,
              [FromQuery] string? status,

             [FromQuery] int? page = 1,
             [FromQuery] int? pageSize = 10

            )
        {
            var products = await _productRepository.GetProductsAsync(search, orderBy , stock??"", status ?? "", page ?? 1, pageSize?? 10,categoryId??-1);
            return products;
        }
        [HttpPost("create-product-with-images")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductInputModel productInput)
        {
            try
            {
                if (productInput.ImageFiles != null && productInput.ImageFiles.Count > 0)
                {
                    return Ok(await _productRepository.CreateProductWithImage(productInput));
                }

                return BadRequest("Hình ảnh không hợp lệ.");
            }
            catch
            {
                return StatusCode(500, "Something went wrong");
            }
        }

        [HttpPost("changeStock/{productId}")]
        public async Task<IActionResult> ChangeStock([FromQuery] bool stock,int productId)
        {
            try
            {
                Product product = await _productRepository.GetProductByIdAsync(productId);
                if (product == null) return BadRequest("Sản phẩm không tồn tại ");
                return Ok(await _productRepository.ChangeStock(stock, product));
            }
            catch (Exception ex)
            {
                 return BadRequest("Some things went wrong");

            }
        }

        [HttpPut("update/{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromForm] ProductUpdateModel productInput)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(productId);

            if (existingProduct == null)
            {
                return NotFound("Sản phẩm không tồn tại.");
            }

            existingProduct.Name = productInput.Name;
            existingProduct.Description = productInput.Description;
            existingProduct.Price = productInput.Price;
            existingProduct.CategoryId = productInput.CategoryId;
           if (productInput.SoLuong < 0)
            {
                existingProduct.TotalProduct += existingProduct.Soluong + productInput.SoLuong < 0 ? -existingProduct.Soluong : productInput.SoLuong;

                existingProduct.Soluong = existingProduct.Soluong+productInput.SoLuong <0?0: existingProduct.Soluong + productInput.SoLuong;
            }
            else
            {
                existingProduct.Soluong += productInput.SoLuong;
                existingProduct.TotalProduct += productInput.SoLuong;
            }

            existingProduct.Discount = productInput.Discount;
            if(existingProduct.Images == null)
            {
                existingProduct.Images= new List<ProductImage>();   
            }

            // Cập nhật danh sách đường dẫn hình ảnh
            await _productRepository.UpdateProductImages(existingProduct, productInput);

            return Ok("Sản phẩm đã được cập nhật.");
        }





        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteProduct(int id)
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
