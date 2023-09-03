using WebBanBalo.Dto;
using WebBanBalo.Model;

namespace WebBanBalo.Interface
{
    public interface IProductRepository
    {
        ICollection<Product> GetProducts();
        Product GetProduct(int id);
        Product GetProduct(string name);
        Product GetProductTrimToUpper(ProductDto ProductCreate);
        decimal GetProductRating(int pokeId);
        bool ProductExists(int pokeId);
        bool CreateProduct( Product Product, ProductCategory productCategory);
        bool UpdateProduct( Product Product);
        bool DeleteProduct(Product Product);
        bool Save();
        List<Product> GetProductSearch(string data);
        ICollection<object> GetProductAndCategory(IEnumerable<string> nameCategory);
        ICollection<object> GetProductAndCategory(string nameCategory, int option);

    }
}
