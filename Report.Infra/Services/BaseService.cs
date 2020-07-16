using System.Net;
using Report.Core.Dto.Responses;
using Report.Core.Services;

namespace Report.Infra.Services
{
    public class BaseService : IBaseService
    {
        public Response OkResponse(string message = null, object data = null)
        {
            return new Response
            {
                Code = StatusCode(HttpStatusCode.OK),
                Message = message,
                Data = data
            };
        }

        public Response BadRequestResponse(string message = null, object data = null)
        {
            return new Response
            {
                Code = StatusCode(HttpStatusCode.BadRequest),
                Message = message,
                Data = data
            };
        }

        public Response ForbiddenResponse(string message = null, object data = null)
        {
            return new Response
            {
                Code = StatusCode(HttpStatusCode.Forbidden),
                Message = message,
                Data = data
            };
        }

        public Response NotFoundResponse(string message = null, object data = null)
        {
            return new Response
            {
                Code = StatusCode(HttpStatusCode.NotFound),
                Message = message,
                Data = data
            };
        }

        public Response ConflictResponse(string message = null, object data = null)
        {
            return new Response
            {
                Code = StatusCode(HttpStatusCode.Conflict),
                Message = message,
                Data = data
            };
        }

        private int StatusCode(HttpStatusCode statusCode) => (int)statusCode;
    }
}