using AutoMapper;
using WebBanBalo.Dto;
using WebBanBalo.Model;

namespace WebBanBalo.Helper
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            CreateMap<Order, OrderDto>();
            CreateMap<OrderDto, Order>();
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
            CreateMap<Users, UsersDto>();
            CreateMap<UsersDto, Users>();
            CreateMap<Color, ColorDto>();
            CreateMap<ColorDto, Color>();
        }
    }
}
