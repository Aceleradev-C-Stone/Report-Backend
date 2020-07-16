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

            CreateMap<Core.Dto.Requests.CreateUserRequest, User>();
            CreateMap<Core.Dto.Requests.UpdateUserRequest, User>();

            CreateMap<CreateUserRequest, Core.Dto.Requests.CreateUserRequest>();
            CreateMap<UpdateUserRequest, Core.Dto.Requests.UpdateUserRequest>();
        }
    }
}