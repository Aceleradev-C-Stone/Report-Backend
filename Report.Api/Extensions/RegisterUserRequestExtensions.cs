using AutoMapper;
using Report.Api.Dto.Requests;

namespace Report.Api.Extensions
{
    public static class RegisterUserRequestExtensions
    {
        public static Core.Dto.Requests.RegisterUserRequest MapToCoreRequest(
            this RegisterUserRequest request,
            IMapper mapper)
        {
            return mapper.Map<Core.Dto.Requests.RegisterUserRequest>(request);
        }
    }
}