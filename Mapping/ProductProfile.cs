 
namespace E_Commerce_API.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<AddProductDTO, Product>()
           .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());  

            CreateMap<GetAllProduct, Product>().ReverseMap();
            CreateMap<ResponseProduct, Product>().ReverseMap();
            CreateMap<UpdateProductDTO, Product>().ReverseMap();
            CreateMap<Product , ResponseProduct>().ReverseMap();
            CreateMap<Product , ProductBaseDTO>().ReverseMap();

                 


        }
    }
}
