using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Report.Api.Dto.Requests;
using Report.Api.Dto.Responses;
using Report.Core.Enums;
using Report.Core.Models;
using Report.Core.Repositories;
using Report.Infra.Extensions;

namespace Report.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LogController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogRepository _repository;

        public LogController(IMapper mapper, ILogRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            try
            {
                var logs = await _repository.GetAll();
                var response = _mapper.Map<LogResponse[]>(logs);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: { ex.Message }");
            }
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetLogsByUserId(int userId)
        {
            try
            {
                var managerRole = EUserRole.MANAGER;
                var loggedUserId = getLoggedUserId();
                var loggedUserRole = getLoggedUserRole();

                if (userId.Equals(loggedUserId) || loggedUserRole.Equals(managerRole.GetName()))
                {
                    var logs = await _repository.GetAllByUserId(userId);
                    var response = _mapper.Map<LogResponse[]>(logs);
                    return Ok(response);
                }

                return StatusCode(403,
                    new { message = "Não é possível obter logs de outro usuário" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: { ex.Message }");
            }
        }

        [HttpGet("unarchived/user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUnarchivedLogsByUserId(int userId)
        {
            try
            {
                var loggedUserId = getLoggedUserId();
                if (!userId.Equals(loggedUserId))
                    return StatusCode(403,
                        new { message = "Não é possível obter logs de outro usuário" });

                var logs = await _repository.GetAllUnarchivedByUserId(userId);
                var response = _mapper.Map<LogResponse[]>(logs);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: { ex.Message }");
            }
        }

        [HttpGet("archived/user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetArchivedLogsByUserId(int userId)
        {
            try
            {
                var loggedUserId = getLoggedUserId();
                if (!userId.Equals(loggedUserId))
                    return StatusCode(403,
                        new { message = "Não é possível obter logs de outro usuário" });

                var logs = await _repository.GetAllArchivedByUserId(userId);
                var response = _mapper.Map<LogResponse[]>(logs);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: { ex.Message }");
            }
        }

        [HttpGet("{logId}")]
        [Authorize]
        public async Task<IActionResult> GetLogById(int logId)
        {
            try
            {
                var log = await _repository.GetById(logId);

                var userId = getLoggedUserId();
                if (!log.UserId.Equals(userId))
                    return StatusCode(403,
                        new { message = "Não é possível obter logs de outro usuário" });

                var response = _mapper.Map<LogResponse>(log);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: { ex.Message }");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(CreateLogRequest request)
        {
            try
            {
                int userId = getLoggedUserId();

                var log = mapCreateLogRequestToLog(userId, request);
                _repository.Add(log);

                if (await _repository.SaveChangesAsync())
                {
                    var response = _mapper.Map<LogResponse>(log);
                    return Ok(log);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro: { ex.Message }" });
            }

            return BadRequest();
        }

        [HttpPut("{logId}")]
        [Authorize]
        public async Task<IActionResult> Put(int logId, UpdateLogRequest request)
        {
            try
            {
                var log = await _repository.GetById(logId);

                var userId = getLoggedUserId();
                if (!log.UserId.Equals(userId))
                    return StatusCode(403,
                        new { message = "Não é possível atualizar um log de outro usuário" });

                log = mapUpdateLogRequestToLog(log, request);
                _repository.Update(log);

                if (await _repository.SaveChangesAsync())
                {
                    var response = _mapper.Map<LogResponse>(log);
                    return Ok(response);
                }
            }
            catch (NullReferenceException)
            {
                return NotFound(new { message = "Log não encontrado" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro: { ex.Message }" });
            }

            return BadRequest();
        }

        [HttpDelete("{logId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int logId)
        {
            try
            {
                var log = await _repository.GetById(logId);

                if (log.Equals(null))
                    return NotFound(new { message = "Log não encontrado" });

                var userId = getLoggedUserId();
                if (!log.UserId.Equals(userId))
                    return StatusCode(403,
                        new { message = "Não é possível deletar um log de outro usuário" });
                
                _repository.Delete(log);

                if (await _repository.SaveChangesAsync())
                    return Ok(new { message = "Log deletado" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro: { ex.Message }" });
            }

            return BadRequest();
        }

        [HttpPatch("archive/{logId}")]
        [Authorize]
        public async Task<IActionResult> Archive(int logId)
        {
            try
            {
                var log = await _repository.GetById(logId);

                var userId = getLoggedUserId();
                if (!log.UserId.Equals(userId))
                    return StatusCode(403, new {
                        message = "Não é possível arquivar ou desarquivar um log de outro usuário"
                    });

                log.Archived = !log.Archived;
                _repository.Update(log);

                if (await _repository.SaveChangesAsync())
                {
                    var response = _mapper.Map<LogResponse>(log);
                    return Ok(response);
                }
            }
            catch (NullReferenceException)
            {
                return NotFound(new { message = "Log não encontrado" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro: { ex.Message }" });
            }

            return BadRequest();
        }

        private Log mapCreateLogRequestToLog(int userId, CreateLogRequest request)
        {
            var log = _mapper.Map<Log>(request);
            log.UserId = userId;
            log.Archived = false;
            log.CreatedAt = DateTime.Now;
            return log;
        }

        private Log mapUpdateLogRequestToLog(Log log, UpdateLogRequest request)
        {
            log.Description = request.Description;
            log.Title       = request.Title;
            log.Details     = request.Details;
            log.Source      = request.Source;
            log.EventCount  = request.EventCount;
            log.Level       = request.Level;
            log.Channel     = request.Channel;
            return log;
        }

        private int getLoggedUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return int.Parse(userId);
        }

        private string getLoggedUserRole()
        {
            return User.FindFirst(ClaimTypes.Role).Value;
        }
    }
}