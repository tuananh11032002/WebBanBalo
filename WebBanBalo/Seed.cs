using System.Diagnostics.Metrics;
using WebBanBalo.Data;
using WebBanBalo.Model;

namespace WebBanBalo
{
    public class Seed
    {
        private readonly DataContext dataContext;
        public Seed(DataContext context)
        {
            this.dataContext = context;
        }
        public void SeedDataContext()
        {
            if (!dataContext.ProductCategory.Any())
            {
                var productCategory = new List<ProductCategory>()
                {
                    new ProductCategory()

                    {
                        Product = new Product()
                        {
                            Name = "FLIP BACKPACK",
                            Description="",
                            Price=20000,
                            Image="https://product.hstatic.net/1000365849/product/flip_backpack__1__b0c4b95520b54afa8d3ab4b29673b75d_grande.jpg",
                            CreatedAt=new DateTime (2022,3,11),
                            OrderItems= new List<OrderItem>()
                            {
                                new OrderItem()
                                {
                                    Quantity=5,
                                    Price=20000
                                }

                            }
                            
                        },
                        Category = new Category()
                        {
                            Name="BACKPACK",
                            
                        }
                    },
                    new ProductCategory()

                    {
                        Product = new Product()
                        {
                            Name = "FLIP Wallet",
                            Description="",
                            Price=600000,
                            Image="https://product.hstatic.net/1000365849/product/clutch_xl-navy1_15f12c01a0dd4e20ab961274b3334ade_grande.jpg",
                            CreatedAt=new DateTime (2022,3,11),
                            OrderItems= new List<OrderItem>()
                            {
                                new OrderItem()
                                {
                                    Quantity=2,
                                    Price=600000
                                }

                            }

                        },
                        Category = new Category()
                        {
                            Name="Wallet",

                        }
                    },

                };
                dataContext.ProductCategory.AddRange(productCategory);
                dataContext.SaveChanges();
            }
        }
    }
}
