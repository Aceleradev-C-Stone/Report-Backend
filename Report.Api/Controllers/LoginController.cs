using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Report.Api.Dto.Requests;
using Report.Api.Dto.Responses;
using Report.Domain.Repositories;
using Report.Infra.Services;
using Report.Infra.Services.Hash;

namespace Report.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _repository;

        public LoginController(IMapper mapper, IUserRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(
            [FromServices]IHashService hashService,
            [FromServices]ITokenService tokenService,
            LoginUserRequest request)
        {
            try
            {
                var user = await _repository.GetByEmail(request.Email);

                if (!hashService.AreEqual(request.Password, user.Hash, user.Salt))
                    return StatusCode(403, new { message = "Email ou senha incorretos" });

                var response = new LoginUserResponse();
                response.User = _mapper.Map<UserResponse>(user);
                response.Token = tokenService.GenerateToken(user);
                response.ExpiresIn = tokenService.GetExpirationInSeconds();
                
                return Ok(response);
            }
            catch (NullReferenceException)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro: { ex.Message }" });
            }
        }
    }
}