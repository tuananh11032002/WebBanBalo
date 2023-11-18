using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using WebBanBalo.Data;
using WebBanBalo.Dto;
using WebBanBalo.Interface;
using WebBanBalo.Model;
using WebBanBalo.ModelOther;
using static System.Net.Mime.MediaTypeNames;

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


        public async Task<IEnumerable<object>> GetFilteredProducts(int categoryId, string search, string orderBy, int pageSize, int pageIndex)
        {
            var categories = await  _dataContext.Category
                .Where(c => categoryId == -1 || c.Id == categoryId)
                .Include(c => c.Products).ThenInclude(im=>im.Images).ToListAsync();

            var result = new List<dynamic>();

            foreach (var category in categories)
            {
                var categoryProducts = category.Products.Where(p=>p.Stock==true);

                if (!string.IsNullOrEmpty(search))
                {
                    categoryProducts = categoryProducts
                        .Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if (!string.IsNullOrEmpty(orderBy))
                {
                    if (orderBy == "insc")
                    {
                        categoryProducts = categoryProducts.Where(p => p.Stock).OrderBy(o=>o.Price).ToList();
                    }
                    else if (orderBy == "desc")
                    {
                        categoryProducts = categoryProducts.Where(p => p.Stock).OrderByDescending(o=>o.Price).ToList();
                    }
                }

                result.Add(new
                {
                    categoryName = category.Name,
                    products = categoryProducts.Select(p => new ProductDto
                    {
                        Id = p.Id,
                        Name=p.Name,
                        Description = p.Description,
                        CategoryId = category.Id,
                        CreatedAt = p.CreatedAt,
                        Image = p.Images.Select(i => i.FilePath).ToList(),
                        Stock = p.Stock,
                        Price = p.Price,
                        Soluong = p.Soluong,


                    }).ToList(),
                }); ;
            }

            var totalProductCount = result.SelectMany(x => ((List<ProductDto>)x.products)).Count();

            var totalPages = (int)Math.Ceiling((double)totalProductCount / pageSize);
            var currentPage = Math.Max(1, Math.Min(pageIndex, totalPages));

            var pagedResult = result
                .Select(x => new
                {
                    totalProduct = x.products.Count,

                    totalPage = (int)Math.Ceiling(((List<ProductDto>)x.products).Count() / (double)pageSize),
                    categoryName = x.categoryName,
                    products = ((List<ProductDto>)x.products)
                        .Skip((currentPage - 1) * pageSize)
                        .Take(pageSize)
                });

            return pagedResult;
        }

        #region temp
        //public async Task<IEnumerable<Product>> GetProductsAsync(string search, string orderBy, int page, int pageSize )
        //{
        //    IQueryable<Product> query = _dataContext.Product.Include(p=>p.Images);

        //    if (!string.IsNullOrWhiteSpace(search))
        //    {
        //        query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
        //    }

        //    if (!string.IsNullOrWhiteSpace(orderBy))
        //    {
        //        switch (orderBy.ToLower())
        //        {
        //            case "desc":
        //                query = query.OrderByDescending(p => p.Price);
        //                break;
        //            case "asc":
        //                query = query.OrderBy(p => p.Price);
        //                break;
        //            default:
        //                // Không hợp lệ, mặc định sắp xếp theo giá tăng dần
        //                query = query.OrderBy(p => p.Price);
        //                break;
        //        }
        //    }

        //    var products = await query
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    return products;
        //}
        #endregion



        public async Task<object> GetProductsAsync(string search, string orderBy, string stock,StatusProduct? status, int page  , int pageSize , int categoryId)
        {
            IQueryable<Product> query = _dataContext.Product.Include(p => p.Images).Include(i => i.Categories);
            if (!string.IsNullOrWhiteSpace(stock))
            {
                if (stock.ToLower() == "in")
                {
                    query = query.Where(p => p.Stock==true);
                }
                if (stock.ToLower() == "out")
                {
                    query = query.Where(p => p.Stock == false);
                }
            }
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
            if (status!=null)
            {           
                query = query.Where(p=>p.Status==status);                    
            }
            if (categoryId != -1)
            {
                if(!_dataContext.Category.Any(p => p.Id == categoryId))
                {
                    return new { };
                }
                else
                {
                    query = query.Where(p => p.CategoryId == categoryId);
                }
            }

            var products = await query
                .Skip(search.IsNullOrEmpty()? (page - 1) * pageSize : 0)
                .Take(search.IsNullOrEmpty() ? pageSize: int.MaxValue)
                .ToListAsync();

            // Tạo danh sách sản phẩm với danh sách đường dẫn hình ảnh
            var productsWithImagePaths = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Soluong = p.Soluong,
                Stock = p.Stock,
                CreatedAt = p.CreatedAt,
                CategoryId = p.CategoryId,
                CategoryName = p.Categories.Name,
                Image = p.Images.Where(im => im.ProductId == p.Id).Select(i => i.FilePath).ToList(),
                Status= p.Status
            }).ToList();
            var result = new
            {
                product = productsWithImagePaths,
                totalEntry = productsWithImagePaths.Count,
                totalPage = (int)Math.Ceiling(productsWithImagePaths.Count / (double)pageSize),
                totalProduct = query.ToList().Count,
            };

            return result;
        }



        public async Task<Product> GetProductByIdAsync(int productId)
        {
            try
            {
                return await _dataContext.Product.Where(px => px.Id == productId).Include(p => p.Images).Include(p=>p.Reviews).FirstOrDefaultAsync();

            }
            catch (Exception ex)
            {
                return null;
            }
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
                Images = new List<ProductImage>(),
                TotalProduct = productInput.SoLuong,
                Soluong = productInput.SoLuong,
                CategoryId = productInput.CategoryId,
                Discount = productInput.Discount,
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
            var imageProduct = _dataContext.Product.Include(p => p.Images).SingleOrDefault(px => px.Id == Product.Id);

            if (imageProduct != null)
            {
                foreach (var im in imageProduct.Images)
                {
                    if (File.Exists(Path.Combine(_webHostEnvironment.WebRootPath, im.FilePath))) 
                    {
                        File.Delete(Path.Combine(_webHostEnvironment.WebRootPath, im.FilePath));
                    }
                }

            }

            _dataContext.Product.Remove(imageProduct);
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

        public async Task UpdateProductAsync(Product product)
        {
           
            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdateProductImages(Product product, ProductUpdateModel productInput )
        {
            //IformFile
            var imageList = productInput.ImageFiles;
            _dataContext.Product.Update(product);

            //var oldImagePaths = _dataContext.ProductImage
            //   .Where(i => i.ProductId == product.Id)
            //   .Select(i => Path.Combine(_webHostEnvironment.WebRootPath, i.FilePath));

            //foreach (var imagePath in oldImagePaths)
            //{
            //    if (System.IO.File.Exists(imagePath))
            //    {
            //        System.IO.File.Delete(imagePath);
            //    }
            //}
           
            List<string> ImageLink = _dataContext.ProductImage.Where(i => i.ProductId == product.Id).Select(im=>im.FilePath).ToList();

            var deletedImages = ImageLink.Except(productInput.linkImage).ToArray();
            string webRootPath = _webHostEnvironment.WebRootPath;


            if (ImageLink != null)
            {
                foreach (var imageLink in deletedImages)
                {
                    string fullPath = Path.Combine(webRootPath, imageLink);
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);

                    }

                    if (imageLink != null)
                    {
                        _dataContext.ProductImage.Remove(_dataContext.ProductImage.Where(i => i.FilePath == imageLink).First());
                    }
                }
                await _dataContext.SaveChangesAsync();
            }
            
           

            string imageDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            var imagePaths = new List<string>();
            if (imageList != null) {

                foreach (var imageFile in imageList)
                {
                    if (imageFile.Length > 0)
                    {
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




                foreach (var imagePath in imagePaths)
                {
                    product.Images.Add(new ProductImage { FilePath = imagePath });
                }
            }

          
             await _dataContext.SaveChangesAsync();
        }

        public bool UpdateProduct( Product Product)
        {
            _dataContext.Product.Update(Product);
            return Save();
               
        }

        public async Task<bool> ChangeStock(bool stock, Product product)
        {
            product.Stock= stock;
            _dataContext.Update(product);
            await _dataContext.SaveChangesAsync();

            return true ;
        }

        public async Task<ValueReturn> GetProductById(int id)
        {
            try
            {
                var product = await _dataContext.Product.FindAsync(id);
                if (product == null)
                {
                    return new ValueReturn
                    {
                        Status = false,
                        Message = "Không tìm thấy sản phẩm"
                    };
                }
                else
                {

                    return new ValueReturn
                    {
                        Status = true,
                        Data = await _dataContext.Product.Where(p => p.Id == id).Select(p => new
                        {
                            Id=p.Id,
                            Name= p.Name,
                            Price= p.Price,
                            Image = p.Images.Select(pc=>pc.FilePath).ToList(),
                            Description = p.Description,
                            Reviews = p.Reviews.Select(re=>new
                            {
                                Id=re.Id,
                                Rating = re.Rating,
                                DatePosted = re.DatePosted,
                                Comment = re.Comment,
                                UserName= re.User.UserName,
                            }).ToList()

                        }).FirstOrDefaultAsync()
                    };
                }
                
            
            
            }
            catch (Exception ex) 
            {
                return new ValueReturn { Status = false , Message=ex.Message };

            }
        }
    }
}
