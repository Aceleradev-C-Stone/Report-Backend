using System;
using System.Threading.Tasks;
using AutoMapper;
using Report.Core.Dto.Requests;
using Report.Core.Dto.Responses;
using Report.Core.Enums;
using Report.Core.Models;
using Report.Core.Repositories;
using Report.Core.Services;

namespace Report.Infra.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _repository;
        private readonly ITokenService _tokenService;
        private readonly IHashService _hashService;

        public AuthService(
            IMapper mapper,
            IUserRepository repository,
            ITokenService tokenService,
            IHashService hashService)
        {
            _mapper = mapper;
            _repository = repository;
            _tokenService = tokenService;
            _hashService = hashService;
        }

        public async Task<Response> Authenticate(LoginUserRequest request)
        {
            try
            {
                var user = await _repository.GetByEmail(request.Email);

                if (!_hashService.AreEqual(request.Password, user.Hash, user.Salt))
                    return ForbiddenResponse("Email ou senha incorretos");

                var response = new LoginUserResponse();
                response.User = _mapper.Map<UserResponse>(user);
                response.Token = _tokenService.GenerateToken(user);
                response.ExpiresIn = _tokenService.GetExpirationInSeconds();
                
                return OkResponse(null, response);
            }
            catch (NullReferenceException)
            {
                return NotFoundResponse("Usuário não encontrado");
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ex.Message);
            }
        }

        public async Task<Response> Register(RegisterUserRequest request)
        {
            try
            {
                var user = _mapper.Map<User>(request);
                var found = await _repository.GetByEmail(user.Email);

                if (found != null)
                    return ConflictResponse("Esse email já está sendo utilizado");

                var saltedHash = _hashService.GenerateSaltedHash(request.Password);
                user.Salt = saltedHash.Salt;
                user.Hash = saltedHash.Hash;

                user.Role = EUserRole.DEVELOPER;
                user.CreatedAt = DateTime.Now;

                _repository.Add(user);
                
                if (await _repository.SaveChangesAsync())
                {
                    var response = _mapper.Map<UserResponse>(user);
                    return OkResponse(null, response);
                }

                return BadRequestResponse("Erro desconhecido");
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ex.Message);
            }
        }
    }
}