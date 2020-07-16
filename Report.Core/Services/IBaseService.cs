using Report.Core.Dto.Responses;

namespace Report.Core.Services
{
    public interface IBaseService
    {
        Response OkResponse(string message = null, object data = null);
        Response BadRequestResponse(string message = null, object data = null);
        Response ForbiddenResponse(string message = null, object data = null);
        Response NotFoundResponse(string message = null, object data = null);
        Response ConflictResponse(string message = null, object data = null);
    }
}