using AutoMapper;
using Report.Api.Dto.Requests;

namespace Report.Api.Extensions
{
    public static class CreateUserRequestExtensions
    {
        public static Core.Dto.Requests.CreateUserRequest MapToCoreRequest(
            this CreateUserRequest request,
            IMapper mapper)
        {
            return mapper.Map<Core.Dto.Requests.CreateUserRequest>(request);
        }
    }
}