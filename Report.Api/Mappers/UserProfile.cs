using System.Collections.Generic;
using AutoMapper;
using Report.Api.Dto.Requests;
using Report.Api.Dto.Responses;
using Report.Domain.Models;

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