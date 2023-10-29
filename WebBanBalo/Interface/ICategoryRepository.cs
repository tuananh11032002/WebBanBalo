using Microsoft.AspNetCore.Mvc;
using WebBanBalo.Dto;
using WebBanBalo.Model;

namespace WebBanBalo.Interface
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int id);
        Category GetCategory(string name);
        Category GetCategoryTrimToUpper(CategoryDto CategoryCreate);
        bool CategoryExists(int pokeId);
        bool CreateCategory(Category Category);
        bool UpdateCategory(Category Category);
        bool DeleteCategory(Category Category);
        bool Save();
    }
}
