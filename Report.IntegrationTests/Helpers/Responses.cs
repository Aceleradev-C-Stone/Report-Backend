using System.Net;
using Report.Core.Dto.Responses;

namespace Report.IntegrationTests.Helpers
{
    public static class Responses
    {
        public static Response OkResponse(string message = null, object data = null)
        {
            return new Response
            {
                Code = StatusCode(HttpStatusCode.OK),
                Message = message,
                Data = data
            };
        }

        public static Response BadRequestResponse(string message = null, object data = null)
        {
            return new Response
            {
                Code = StatusCode(HttpStatusCode.BadRequest),
                Message = message,
                Data = data
            };
        }

        public static Response ForbiddenResponse(string message = null, object data = null)
        {
            return new Response
            {
                Code = StatusCode(HttpStatusCode.Forbidden),
                Message = message,
                Data = data
            };
        }

        public static Response NotFoundResponse(string message = null, object data = null)
        {
            return new Response
            {
                Code = StatusCode(HttpStatusCode.NotFound),
                Message = message,
                Data = data
            };
        }

        public static Response ConflictResponse(string message = null, object data = null)
        {
            return new Response
            {
                Code = StatusCode(HttpStatusCode.Conflict),
                Message = message,
                Data = data
            };
        }

        private static int StatusCode(HttpStatusCode statusCode) => (int)statusCode;
    }
}