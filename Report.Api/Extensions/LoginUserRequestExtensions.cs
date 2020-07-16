using AutoMapper;
using Report.Api.Dto.Requests;

namespace Report.Api.Extensions
{
    public static class LoginUserRequestExtensions
    {
        public static Core.Dto.Requests.LoginUserRequest MapToCoreRequest(
            this LoginUserRequest request,
            IMapper mapper)
        {
            return mapper.Map<Core.Dto.Requests.LoginUserRequest>(request);
        }
    }
}