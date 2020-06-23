using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Report.Domain.Models;
using Report.Domain.Repositories;
using Report.Api.Authorization;
using Report.Domain.Enums;
using Report.Infra.Services.Hash;
using System.Collections.Generic;
using AutoMapper;
using Report.Api.Dto.Responses;
using Report.Api.Dto.Requests;

namespace Report.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _repository;

        public UserController(IMapper mapper, IUserRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        [HttpGet]
        [AuthorizeUserRoles(EUserRole.MANAGER)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var users = await _repository.GetAll();
                var response = _mapper.Map<UserResponse[]>(users);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro: { ex.Message }" });
            }
        }

        [HttpGet("{userId}")]
        [AuthorizeUserRoles(EUserRole.MANAGER)]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var user = await _repository.GetById(userId);
                var response = _mapper.Map<UserResponse>(user);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro: { ex.Message }" });
            }
        }

        [HttpPost]
        [AuthorizeUserRoles(EUserRole.MANAGER)]
        public async Task<IActionResult> Post(
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

        [HttpPut("{userId}")]
        [AuthorizeUserRoles(EUserRole.MANAGER)]
        public async Task<IActionResult> Put(
            [FromServices]IHashService hashService,
            int userId,
            UpdateUserRequest request)
        {
            try
            {
                var user = await _repository.GetById(userId);

                if (user.Equals(null))
                    return NotFound(new { message = "Usuário não encontrado" });

                user.Name  = request.Name;
                
                var saltedHash = hashService.GenerateSaltedHash(request.Password);
                user.Salt = saltedHash.Salt;
                user.Hash = saltedHash.Hash;
                
                _repository.Update(user);

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

        [HttpDelete("{userId}")]
        [AuthorizeUserRoles(EUserRole.MANAGER)]
        public async Task<IActionResult> Delete(int userId)
        {
            try
            {
                var user = await _repository.GetById(userId);
                
                if (user.Equals(null))
                    return NotFound(new { message = "Usuário não encontrado" });
                
                _repository.Delete(user);

                if (await _repository.SaveChangesAsync())
                    return Ok(new { message = "Usuário deletado" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro: { ex.Message }" });
            }

            return BadRequest();
        }   
    }
}