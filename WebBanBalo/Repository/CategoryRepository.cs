using Microsoft.AspNetCore.Mvc;
using WebBanBalo.Data;
using WebBanBalo.Dto;
using WebBanBalo.Interface;
using WebBanBalo.Model;

namespace WebBanBalo.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _dataContext;
        public CategoryRepository(DataContext dataContext)
        {
            _dataContext = dataContext; 
        }
        public bool CategoryExists(int Id)
        {
            return _dataContext.Category.Any(p=>p.Id == Id);
            
        }

        public bool CreateCategory(Category Category)
        {
            _dataContext.Category.Add(Category);
            return Save();
        }

        public bool DeleteCategory(Category Category)
        {
            _dataContext?.Category.Remove(Category);
            return Save();

        }

        public ICollection<Category> GetCategories()
        {
            return _dataContext.Category.ToList();
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

        public ICollection<Product> GetProductbyCate(int id)

        {
            return _dataContext.ProductCategory.Where(p => p.CategoryId == id).Select(p => p.Product).ToList();
            
        }

        public bool Save()
        {
            
            var saved = _dataContext.SaveChanges();
            return  saved >0 ?true:false;

        }

        public bool UpdateCategory(Category Category)
        {
            _dataContext.Category.Update(Category);
            return Save();
        }
    }
}
