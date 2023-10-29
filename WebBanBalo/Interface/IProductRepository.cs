using WebBanBalo.Dto;
using WebBanBalo.Model;
using WebBanBalo.ModelOther;

namespace WebBanBalo.Interface
{
    public interface IProductRepository
    {
        bool UpdateProduct(Product Product);
        bool ProductExists(int productId);
        Task<IEnumerable<Product>> GetProductsAsync(string search, string orderBy, int page, int pageSize);
        Task<Product> CreateProductWithImage(ProductInputModel productInput);
        bool DeleteProduct(Product Product);
        Task<Product> GetProductByIdAsync(int productId);
    }
}
