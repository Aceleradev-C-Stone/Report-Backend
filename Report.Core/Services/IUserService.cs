using System.Threading.Tasks;
using Report.Core.Dto.Requests;
using Report.Core.Dto.Responses;

namespace Report.Core.Services
{
    public interface IUserService
    {
        Task<Response> Get();
        Task<Response> GetUserById(int userId);
        Task<Response> Post(CreateUserRequest request);
        Task<Response> Put(int userId, UpdateUserRequest request);
        Task<Response> Delete(int userId);
        bool IsAuthenticated(int userId);
        bool IsLoggedUserManager();
        int GetLoggedUserId();
        string GetLoggedUserRole();
    }
}