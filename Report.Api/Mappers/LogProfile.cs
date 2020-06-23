using AutoMapper;
using Report.Api.Dto.Requests;
using Report.Api.Dto.Responses;
using Report.Domain.Models;

namespace Report.Api.Mappers
{
    public class LogProfile : Profile
    {
        public LogProfile()
        {
            CreateMap<Log, LogResponse>();
            CreateMap<CreateLogRequest, Log>();
            CreateMap<UpdateLogRequest, Log>();
        }
    }
}