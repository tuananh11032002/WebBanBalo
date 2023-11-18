using AutoMapper;
using WebBanBalo.Dto;
using WebBanBalo.Model;
using WebBanBalo.ModelOther;

namespace WebBanBalo.Helper
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            CreateMap<Order, OrderDto>();
            CreateMap<OrderDto, Order>();
            CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Images.Select(ip => ip.FilePath))); ;
            CreateMap<ProductDto, Product>();
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
            CreateMap<Users, UsersDto>();
            CreateMap<UsersDto, Users>();
            CreateMap<Users, UserCreateInput>();
            CreateMap<UserCreateInput, Users>();
        
        }
    }
}
