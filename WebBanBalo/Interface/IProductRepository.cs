using WebBanBalo.Dto;
using WebBanBalo.Model;
using WebBanBalo.ModelOther;

namespace WebBanBalo.Interface
{
    public interface IProductRepository
    {
        bool UpdateProduct(Product Product);
        bool ProductExists(int productId);
        Task<object> GetProductsAsync(string search, string orderBy ,string stock, StatusProduct? status,int page, int pageSize, int categoryId);
        Task<ValueReturn> GetFilteredProducts(int categoryId, string search , string orderBy, int pageSize, int pageIndex);
        Task<Product> CreateProductWithImage(ProductInputModel productInput);
        bool DeleteProduct(Product Product);
        Task<Product> GetProductByIdAsync(int productId);
        Task UpdateProductAsync(Product product);
        Task UpdateProductImages(Product product, ProductUpdateModel productInput);
        Task<bool> ChangeStock(bool stock, Product product);
        Task<ValueReturn> GetProductById(int id);
    }
}
