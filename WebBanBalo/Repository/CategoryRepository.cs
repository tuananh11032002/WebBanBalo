using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebBanBalo.Data;
using WebBanBalo.Dto;
using WebBanBalo.Interface;
using WebBanBalo.Model;
using WebBanBalo.ModelOther;

namespace WebBanBalo.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CategoryRepository(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public bool CategoryExists(int Id)
        {
            return _dataContext.Category.Any(p=>p.Id == Id);
            
        }

        public async Task<bool> CreateCategory(CategoryCreateModel Category)
        {

            string imageDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            string imagePaths="";
            string imagePathsReplace = "";

            if (Category.Image.Length > 0)
            {
                // Tạo tên tệp hình ảnh duy nhất sử dụng Guid
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Category.Image.FileName);
                 imagePaths = Path.Combine("images", uniqueFileName);

                var imageFilePath = Path.Combine(_webHostEnvironment.WebRootPath, imagePaths);
                using (var stream = new FileStream(imageFilePath, FileMode.Create))
                {
                    await Category.Image.CopyToAsync(stream);
                }
                   
            }
            if (Category.ImageReplace.Length > 0)
            {
                // Tạo tên tệp hình ảnh duy nhất sử dụng Guid
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Category.ImageReplace.FileName);

                imagePathsReplace = Path.Combine("images", uniqueFileName);

                var imageFilePath = Path.Combine(_webHostEnvironment.WebRootPath, imagePathsReplace);
                using (var stream = new FileStream(imageFilePath, FileMode.Create))
                {
                    await Category.ImageReplace.CopyToAsync(stream);
                }

            }
            var category = new Category
            {
                Image = imagePaths,
                Name = Category.Name,
                ImageReplace= imagePathsReplace,
            };
            _dataContext.Category.Add(category);
            return await SaveAsync();
        }

        private async Task<bool> SaveAsync()
        {
            var saved = await _dataContext.SaveChangesAsync();
            return saved > 0 ? true : false;
            
        }

        public bool DeleteCategory(int  id)
        {
            var category = _dataContext.Category.FirstOrDefault(c => c.Id == id);
            if (!category.Image.IsNullOrEmpty())
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, category.Image);
                string imagePathPlace = Path.Combine(_webHostEnvironment.WebRootPath, category.ImageReplace);


                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);

                }
                if (File.Exists(imagePathPlace))
                {
                    File.Delete(imagePathPlace);
                }
            }
            _dataContext?.Category.Remove(category);
            return Save();

        }

        public ICollection<Category> GetCategories()
        {
            return _dataContext.Category.ToList();
        }

        public ICollection<CategoryDto> GetCategoriesForAdmin()
        {
            var catgory= _dataContext.Category.ToList();
            var result =catgory.Select(p => new CategoryDto
            {
                Name = p.Name,
                Id = p.Id,
                Image = p.Image,
                ImageReplace = p.ImageReplace,
                TotalProduct = _dataContext.Product.Where(ot => ot.CategoryId == p.Id).Count(),
                TotalEarning = _dataContext.OrderItem.Where(ot => ot.Product.CategoryId == p.Id  && ot.Order.Done).Select(pr => pr.Price * pr.Quantity).Sum()
            }).ToList() ;
            return result;
        }

        public Category GetCategory(int id)
        {
            return   _dataContext.Category.Where(p => p.Id == id).FirstOrDefault();
        }

        public Category GetCategory(string name)
        {
            return _dataContext.Category.Where(p => p.Name == name).FirstOrDefault();        }

     



        public Category GetCategoryTrimToUpper(CategoryDto CategoryCreate)
        {
            throw new NotImplementedException();
        }



        public bool Save()
        {
            
            var saved = _dataContext.SaveChanges();
            return  saved >0 ?true:false;

        }

        public bool UpdateCategory(CategoryUpdateModel category)
        {
            var imageReplace = category.ImageReplace;

            var image= category.Image;
            string imRe = "";
            string im = "";
            if (imageReplace!=null)
            {
               if (imageReplace.Length>0) {
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageReplace.FileName);

                    string imagePath = Path.Combine("images", uniqueFileName);
                    imRe = imagePath;

                    var imageFilePath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath);
                    using (var stream = new FileStream(imageFilePath, FileMode.Create))
                    {
                         imageReplace.CopyTo(stream);
                    }
                }
            }
            if (image != null)
            {
                if (image.Length > 0)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

                    string imagePath = Path.Combine("images", uniqueFileName);
                    im = imagePath;

                    var imageFilePath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath);
                    using (var stream = new FileStream(imageFilePath, FileMode.Create))
                    {
                          image.CopyTo(stream);
                    }
                }
            }

            var categoryUpdate = _dataContext.Category.Where(_c => _c.Id == category.Id).Select(p => new Category
            {
                Id = category.Id,
                Name = category.Name,
                Image = string.IsNullOrEmpty(im) ? p.Image : im,
                ImageReplace = string.IsNullOrEmpty(imRe) ? p.ImageReplace : imRe
            }).FirstOrDefault();
            _dataContext.Category.Update(categoryUpdate);
            return Save();
        }
    }
}
