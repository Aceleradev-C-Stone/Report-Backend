using AutoMapper;
using Report.Api.Dto.Requests;

namespace Report.Api.Extensions
{
    public static class UpdateLogRequestExtensions
    {
        public static Core.Dto.Requests.UpdateLogRequest MapToCoreRequest(
            this UpdateLogRequest request,
            IMapper mapper)
        {
            return mapper.Map<Core.Dto.Requests.UpdateLogRequest>(request);
        }
    }
}