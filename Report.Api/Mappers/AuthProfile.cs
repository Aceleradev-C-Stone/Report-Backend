using AutoMapper;
using Report.Api.Dto.Requests;
using Report.Core.Models;

namespace Report.Api.Mappers
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterUserRequest, User>();
            CreateMap<Core.Dto.Requests.RegisterUserRequest, User>();
            
            CreateMap<LoginUserRequest, Core.Dto.Requests.LoginUserRequest>();
            CreateMap<RegisterUserRequest, Core.Dto.Requests.RegisterUserRequest>();
        }
    }
}