using AutoMapper;
using Report.Api.Dto.Requests;
using Report.Api.Dto.Responses;
using Report.Core.Models;

namespace Report.Api.Mappers
{
    public class LogProfile : Profile
    {
        public LogProfile()
        {
            CreateMap<Log, LogResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name));
            CreateMap<CreateLogRequest, Log>();
            CreateMap<UpdateLogRequest, Log>();
        }
    }
}