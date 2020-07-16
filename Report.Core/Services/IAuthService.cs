using System.Threading.Tasks;
using Report.Core.Dto.Requests;
using Report.Core.Dto.Responses;

namespace Report.Core.Services
{
    public interface IAuthService
    {
        Task<Response> Authenticate(LoginUserRequest request);
        Task<Response> Register(RegisterUserRequest request);
    }
}