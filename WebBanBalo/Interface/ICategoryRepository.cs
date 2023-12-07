using Microsoft.AspNetCore.Mvc;
using WebBanBalo.Dto;
using WebBanBalo.Model;
using WebBanBalo.ModelOther;

namespace WebBanBalo.Interface
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        ICollection<CategoryDto> GetCategoriesForAdmin(string? search);
        Category GetCategory(int id);
        Category GetCategory(string name);
        Category GetCategoryTrimToUpper(CategoryDto CategoryCreate);
        bool CategoryExists(int pokeId);
        Task<bool> CreateCategory(CategoryCreateModel Category);
        bool UpdateCategory(CategoryUpdateModel category);
        bool DeleteCategory(int  id);
        bool Save();
    }
}
