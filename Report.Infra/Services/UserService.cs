using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Report.Core.Dto.Requests;
using Report.Core.Dto.Responses;
using Report.Core.Enums;
using Report.Core.Extensions;
using Report.Core.Models;
using Report.Core.Repositories;
using Report.Core.Services;

namespace Report.Infra.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IHashService _hashService;
        private readonly IUserRepository _repository;
        
        public UserService(
            IMapper mapper,
            IHttpContextAccessor http,
            IHashService hashService,
            IUserRepository repository)
        {
            SetupUser(http);
            _mapper = mapper;
            _hashService = hashService;
            _repository = repository;
        }

        public async Task<Response> Get()
        {
            try
            {
                if (!IsLoggedUserManager())
                    return new Response { Code = 403 };

                var users = await _repository.GetAll();
                var response = _mapper.Map<UserResponse[]>(users);
                return new Response { Code = 200, Data = response };
            }
            catch (Exception ex)
            {
                return new Response { Code = 400, Message = ex.Message };
            }
        }

        public async Task<Response> GetUserById(int userId)
        {
            try
            {
                if (!IsAuthenticated(userId))
                {
                    return new Response
                    {
                        Code = 403,
                        Message = "Não é possível obter informações de outro usuário"
                    };
                }

                var user = await _repository.GetById(userId);
                if (user == null)
                {
                    return new Response
                    {
                        Code = 404,
                        Message = "Usuário não encontrado"
                    };
                }
                
                var response = _mapper.Map<UserResponse>(user);
                return new Response { Code = 200, Data = response };
            }
            catch (Exception ex)
            {
                return new Response { Code = 400, Message = ex.Message };
            }
        }

        public async Task<Response> Create(CreateUserRequest request)
        {
            try
            {
                if (!IsLoggedUserManager())
                    return new Response { Code = 403 };

                var user = _mapper.Map<User>(request);
                var found = await _repository.GetByEmail(user.Email);

                if (found != null)
                {
                    return new Response
                    {
                        Code = 409,
                        Message = "Esse email já está sendo utilizado."
                    };
                }

                var saltedHash = _hashService.GenerateSaltedHash(request.Password);
                user.Salt = saltedHash.Salt;
                user.Hash = saltedHash.Hash;

                user.Role = EUserRole.DEVELOPER;
                user.CreatedAt = DateTime.Now;

                _repository.Add(user);
                
                if (await _repository.SaveChangesAsync())
                {
                    var response = _mapper.Map<UserResponse>(user);
                    return new Response { Code = 200, Data = response };
                }

                return new Response { Code = 400, Message = "Erro desconhecido"};
            }
            catch (Exception ex)
            {
                return new Response { Code = 400, Message = ex.Message };
            }
        }

        public async Task<Response> Update(int userId, UpdateUserRequest request)
        {
            try
            {
                if (!IsAuthenticated(userId))
                {
                    return new Response
                    {
                        Code = 403,
                        Message = "Não é possível atualizar informações de outro usuário"
                    };
                }

                var user = await _repository.GetById(userId);
                user.Name  = request.Name;
                
                if (request.Password != null)
                {
                    var saltedHash = _hashService.GenerateSaltedHash(request.Password);
                    user.Salt = saltedHash.Salt;
                    user.Hash = saltedHash.Hash;
                }
                
                _repository.Update(user);

                if (await _repository.SaveChangesAsync())
                {
                    var response = _mapper.Map<UserResponse>(user);
                    return new Response { Code = 200, Data = response };
                }

                return new Response { Code = 400, Message = "Erro desconhecido" };
            }
            catch (NullReferenceException)
            {
                return new Response { Code = 404, Message = "Usuário não encontrado" };
            }
            catch (Exception ex)
            {
                return new Response { Code = 400, Message = ex.Message };
            }
        }

        public async Task<Response> Delete(int userId)
        {
            try
            {
                if (!IsAuthenticated(userId))
                {
                    return new Response
                    {
                        Code = 403,
                        Message = "Não é possível deletar informações de outro usuário"
                    };
                }

                var user = await _repository.GetById(userId);
                if (user == null)
                {
                    return new Response
                    {
                        Code = 404,
                        Message = "Usuário não encontrado"
                    };
                }
                
                _repository.Delete(user);

                if (await _repository.SaveChangesAsync())
                    return new  Response { Code = 200, Message = "Usuário deletado" };

                return new Response { Code = 400, Message = "Erro desconhecido" };
            }
            catch (Exception ex)
            {
                return new Response { Code = 400, Message = ex.Message };
            }
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

        private void SetupUser(IHttpContextAccessor http)
        {
            User = http.HttpContext.User;
        }

        public ClaimsPrincipal User { get; private set; }
    }
}