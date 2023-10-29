using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebBanBalo.Data;
using WebBanBalo.Dto;
using WebBanBalo.Interface;
using WebBanBalo.Model;
using WebBanBalo.ModelOther;

namespace WebBanBalo.Repository
{
    public class ProductRepository : IProductRepository

    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly DataContext _dataContext;
        public ProductRepository(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }

      

        public async Task<IEnumerable<Product>> GetProductsAsync(string search, string orderBy, int page, int pageSize)
        {
            IQueryable<Product> query = _dataContext.Product;

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy.ToLower())
                {
                    case "desc":
                        query = query.OrderByDescending(p => p.Price);
                        break;
                    case "asc":
                        query = query.OrderBy(p => p.Price);
                        break;
                    default:
                        // Không hợp lệ, mặc định sắp xếp theo giá tăng dần
                        query = query.OrderBy(p => p.Price);
                        break;
                }
            }

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return products;
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _dataContext.Product.FindAsync(productId);
        }


        public async Task<Product> CreateProductWithImage(ProductInputModel productInput)
        {
            string imageDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            var imagePaths = new List<string>();

            foreach (var imageFile in productInput.ImageFiles)
            {
                if (imageFile.Length > 0)
                {
                    // Tạo tên tệp hình ảnh duy nhất sử dụng Guid
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string imagePath = Path.Combine("images", uniqueFileName);

                    var imageFilePath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath);
                    using (var stream = new FileStream(imageFilePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    imagePaths.Add(imagePath);
                }
            }

            var product = new Product
            {
                Name = productInput.Name,
                Description = productInput.Description,
                Price = productInput.Price,
                Images = new List<ProductImage>()
            };

            foreach (var imagePath in imagePaths)
            {
                product.Images.Add(new ProductImage { FilePath = imagePath });
            }

            _dataContext.Product.Add(product);
            await _dataContext.SaveChangesAsync();
            return product;
        }
      

        public bool DeleteProduct(Product Product)
        {
            _dataContext.Product.Remove(Product);
            return Save();
        }

    



        public bool ProductExists(int productId)
        {
            return _dataContext.Product.Any(p => p.Id == productId);
        }

        public bool Save()
        {
            var saved = _dataContext.SaveChanges();
            return saved>0 ?true:false;
        }

        public bool UpdateProduct( Product Product)
        {
            _dataContext.Product.Update(Product);
            return Save();
               
        }

      
    }
}
