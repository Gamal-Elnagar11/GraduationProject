 
namespace E_Commerce_API.Mapping
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<RegisterDTO, User>();
            CreateMap<User, UsersDTO>();

            CreateMap<Feedback, FeedDTO>();




        }
    }
}
