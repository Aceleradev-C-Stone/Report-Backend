using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Report.Api.Dto.Requests;
using Report.Api.Dto.Responses;
using Report.Domain.Enums;
using Report.Domain.Models;
using Report.Domain.Repositories;
using Report.Infra.Services.Hash;

namespace Report.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _repository;

        public RegisterController(IMapper mapper, IUserRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(
            [FromServices]IHashService hashService,
            CreateUserRequest request)
        {
            try
            {
                var user = _mapper.Map<User>(request);
                var found = await _repository.GetByEmail(user.Email);

                if (found != null)
                    return Conflict(new { message = "Esse email já está sendo utilizado." });

                var saltedHash = hashService.GenerateSaltedHash(request.Password);
                user.Salt = saltedHash.Salt;
                user.Hash = saltedHash.Hash;

                user.Role = EUserRole.DEVELOPER;
                user.CreatedAt = DateTime.Now;

                _repository.Add(user);
                
                if (await _repository.SaveChangesAsync())
                {
                    var response = _mapper.Map<UserResponse>(user);
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro: { ex.Message }" });
            }

            return BadRequest();
        }
    }
}