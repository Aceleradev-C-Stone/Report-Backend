using AutoMapper;
using Report.Api.Dto.Requests;

namespace Report.Api.Extensions
{
    public static class CreateLogRequestExtensions
    {
        public static Core.Dto.Requests.CreateLogRequest MapToCoreRequest(
            this CreateLogRequest request,
            IMapper mapper)
        {
            return mapper.Map<Core.Dto.Requests.CreateLogRequest>(request);
        }
    }
}