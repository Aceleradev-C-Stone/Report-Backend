using AutoMapper;
using Report.Api.Dto.Requests;
using Report.Core.Dto.Responses;
using Report.Core.Models;

namespace Report.Api.Mappers
{
    public class LogProfile : Profile
    {
        public LogProfile()
        {
            CreateMap<Log, LogResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name));
            CreateMap<Core.Dto.Requests.CreateLogRequest, Log>();
            CreateMap<Core.Dto.Requests.UpdateLogRequest, Log>();

            CreateMap<CreateLogRequest, Core.Dto.Requests.CreateLogRequest>();
            CreateMap<UpdateLogRequest, Core.Dto.Requests.UpdateLogRequest>();
        }
    }
}