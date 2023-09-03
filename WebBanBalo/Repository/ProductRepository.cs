using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebBanBalo.Data;
using WebBanBalo.Dto;
using WebBanBalo.Interface;
using WebBanBalo.Model;

namespace WebBanBalo.Repository
{
    public class ProductRepository : IProductRepository

    {
        private readonly DataContext _dataContext;
        public ProductRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public bool CreateProduct(Product Product, ProductCategory productCategory)
        {
            _dataContext.Add(productCategory);

            _dataContext.Add(Product);
            return Save();
            
        }

        public bool DeleteProduct(Product Product)
        {
            _dataContext.Product.Remove(Product);
            return Save();
        }

        public Product GetProduct(int id)
        {
            return _dataContext.Product.Where(p => p.Id == id).FirstOrDefault();
        }

        
        public Product GetProduct(string name)
        {
            return _dataContext.Product.Where(p => p.Name == name).FirstOrDefault();
        }

        public ICollection<object> GetProductAndCategory(IEnumerable<string> nameCategory)
        {
            

            ICollection<object> result = new List<object>();
            foreach (string category in nameCategory)
            {
                List<Product> products = GetProductbyCategory(category);
                result.Add(new { name = category, product = products });
            }
            return result;


        }

        public ICollection<object> GetProductAndCategory(string nameCategory, int option)
        {
            ICollection<object> result = new List<object>();

            var products = new List<Product>();
            if(option == 2)
            {
                products =  _dataContext.ProductCategory.Where(p => p.Category.Name == nameCategory).Select(p => p.Product).OrderBy(p => p.Price).ToList();

            }

             else if(option == 3) {
                products = _dataContext.ProductCategory.Where(p => p.Category.Name == nameCategory).Select(p => p.Product).OrderByDescending(p => p.Price).ToList();

            }

            result.Add(new { name = nameCategory, product = products });
            
           
           
            return result;
        }

        public List<Product> GetProductbyCategory(string name )
        {
            var productIds = _dataContext.ProductCategory
                .Where(p => p.Category.Name == name)
                .Select(p => p.ProductId)
                .ToList();

            return _dataContext.Product
                .Where(p => productIds.Contains(p.Id))
                .ToList();
        }


        public decimal GetProductRating(int pokeId)
        {
            throw new NotImplementedException();
        }

        public ICollection<Product> GetProducts()
        {

            return _dataContext.Product.OrderByDescending(p => p.Id).ToList();
        }

        public List<Product> GetProductSearch(string data)
        {
            List<Product> products = _dataContext.Product.Where(p=> p.Name.ToLower().Contains(data.ToLower())).ToList();
            return products;
        }

        public Product GetProductTrimToUpper(ProductDto ProductCreate)
        {
            throw new NotImplementedException();
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
