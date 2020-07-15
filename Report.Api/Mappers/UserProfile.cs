using AutoMapper;
using Report.Api.Dto.Requests;
using Report.Core.Dto.Responses;
using Report.Core.Models;

namespace Report.Api.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserResponse>();
            CreateMap<CreateUserRequest, User>();
            CreateMap<UpdateUserRequest, User>();
        }
    }
}