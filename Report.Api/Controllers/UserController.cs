using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Report.Domain.Commands;
using Report.Domain.Models;
using Report.Domain.Entities;
using Report.Domain.Repositories;
using Report.Api.Authorization;
using Report.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Report.Infra.Services;

namespace Report.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;

        public UserController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [AuthorizeUserRoles(EUserRole.MANAGER)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _repository.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: { ex.Message }");
            }
        }

        [HttpGet("{userId}")]
        [AuthorizeUserRoles(EUserRole.MANAGER)]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var result = await _repository.GetById(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: { ex.Message }");
            }
        }

        [HttpPost]
        [AuthorizeUserRoles(EUserRole.MANAGER)]
        public async Task<IActionResult> Post(CreateUserCommand command)
        {
            try
            {
                var user = new User();
                user.Name = command.Name;
                user.Email = command.Email;
                user.CreatedAt = DateTime.Now;

                var saltedHash = SaltedHash.Generate(command.Password);
                user.Salt = saltedHash.Salt;
                user.Hash = saltedHash.Hash;

                _repository.Add(user);
                
                if (await _repository.SaveChangesAsync())
                    return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: { ex.Message }");
            }

            return BadRequest();
        }

        [HttpPut("{userId}")]
        [AuthorizeUserRoles(EUserRole.MANAGER)]
        public async Task<IActionResult> Put(int userId, CreateUserCommand command)
        {
            try
            {
                var user = await _repository.GetById(userId);

                if (user == null)
                    return NotFound("Usuário não encontrado");

                user.Name  = command.Name;
                user.Email = command.Email;
                
                var saltedHash = SaltedHash.Generate(command.Password);
                user.Salt = saltedHash.Salt;
                user.Hash = saltedHash.Hash;
                
                _repository.Update(user);

                if (await _repository.SaveChangesAsync())
                    return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: { ex.Message }");
            }

            return BadRequest();
        }

        [HttpDelete("{userId}")]
        [AuthorizeUserRoles(EUserRole.MANAGER)]
        public async Task<IActionResult> Delete(int userId)
        {
            try
            {
                var user = await _repository.GetById(userId);
                
                if (user == null)
                    return NotFound("Usuário não encontrado");
                
                _repository.Delete(user);

                if (await _repository.SaveChangesAsync())
                    return Ok("Usuário deletado");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: { ex.Message }");
            }

            return BadRequest();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(
            [FromServices]ITokenService tokenService,
            LoginUserCommand command)
        {
            try
            {
                var user = await _repository.GetByEmail(command.Email);
                
                if (user == null)
                    return NotFound(new { message = "Usuário não encontrado" });

                if (!SaltedHash.AreEqual(command.Password, user.Hash, user.Salt))
                    return StatusCode(403, new { message = "Email ou senha incorretos" });

                var token = tokenService.GenerateToken(user);

                user.Hash = "";
                user.Salt = "";

                return Ok(new { user = user, token = token });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: { ex.Message }");
            }
        }
    }
}