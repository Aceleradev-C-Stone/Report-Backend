using AutoMapper;
using Report.Api.Dto.Requests;

namespace Report.Api.Extensions
{
    public static class UpdateUserRequestExtensions
    {
        public static Core.Dto.Requests.UpdateUserRequest MapToCoreRequest(
            this UpdateUserRequest request,
            IMapper mapper)
        {
            return mapper.Map<Core.Dto.Requests.UpdateUserRequest>(request);
        }
    }
}