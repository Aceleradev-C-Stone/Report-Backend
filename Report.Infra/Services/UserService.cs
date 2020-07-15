using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Report.Core.Dto.Requests;
using Report.Core.Dto.Responses;
using Report.Core.Enums;
using Report.Core.Extensions;
using Report.Core.Services;

namespace Report.Infra.Services
{
    public class UserService : IUserService
    {
        public UserService(IHttpContextAccessor http)
        {
            User = http.HttpContext.User;
        }

        public Task<Response> Get()
        {
            throw new System.NotImplementedException();
        }

        public Task<Response> GetUserById(int userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Response> Post(CreateUserRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<Response> Put(int userId, UpdateUserRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<Response> Delete(int userId)
        {
            throw new System.NotImplementedException();
        }

        public bool IsAuthenticated(int userId)
        {
            return userId == GetLoggedUserId() || IsLoggedUserManager();
        }

        public bool IsLoggedUserManager()
        {
            return GetLoggedUserRole() == EUserRole.MANAGER.GetName();
        }

        public int GetLoggedUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return int.Parse(userId);
        }

        public string GetLoggedUserRole()
        {
            return User.FindFirst(ClaimTypes.Role).Value;
        }

        public ClaimsPrincipal User { get; private set; }
    }
}