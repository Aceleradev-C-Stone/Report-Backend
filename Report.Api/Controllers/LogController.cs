using System;
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
using Report.Core.Extensions;

namespace Report.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
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
        public async Task<IActionResult> GetLogsByUserId(int userId)
        {
            try
            {
                if (!IsAuthenticated(userId))
                    return StatusCode(403,
                        new { message = "Não é possível obter logs de outro usuário" });

                var logs = await _repository.GetAllByUserId(userId);
                var response = _mapper.Map<LogResponse[]>(logs);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: { ex.Message }");
            }
        }

        [HttpGet("unarchived/user/{userId}")]
        public async Task<IActionResult> GetUnarchivedLogsByUserId(int userId)
        {
            try
            {
                if (!IsAuthenticated(userId))
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
        public async Task<IActionResult> GetArchivedLogsByUserId(int userId)
        {
            try
            {
                if (!IsAuthenticated(userId))
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
        public async Task<IActionResult> GetLogById(int logId)
        {
            try
            {
                var log = await _repository.GetById(logId);

                if (!IsAuthenticated(log.UserId))
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
        public async Task<IActionResult> Post(CreateLogRequest request)
        {
            try
            {
                // If user role equals MANAGER, use id from request body,
                // otherwise, use logged user id
                int userId = GetLoggedUserId();
                if (IsLoggedUserManager() && request.UserId.HasValue)
                    userId =  request.UserId.Value;

                var log = MapCreateLogRequestToLog(userId, request);
                _repository.Add(log);

                if (await _repository.SaveChangesAsync())
                {
                    log = await _repository.GetById(log.Id);
                    var response = _mapper.Map<LogResponse>(log);
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro: { ex.Message }" });
            }

            return BadRequest();
        }

        [HttpPut("{logId}")]
        public async Task<IActionResult> Put(int logId, UpdateLogRequest request)
        {
            try
            {
                var log = await _repository.GetById(logId);

                if (!IsAuthenticated(log.UserId))
                    return StatusCode(403,
                        new { message = "Não é possível atualizar um log de outro usuário" });

                log = MapUpdateLogRequestToLog(log, request);
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
        public async Task<IActionResult> Delete(int logId)
        {
            try
            {
                var log = await _repository.GetById(logId);

                if (log.Equals(null))
                    return NotFound(new { message = "Log não encontrado" });

                if (!IsAuthenticated(log.UserId))
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
        public async Task<IActionResult> Archive(int logId)
        {
            try
            {
                var log = await _repository.GetById(logId);

                if (!IsAuthenticated(log.UserId))
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

        private Log MapCreateLogRequestToLog(int userId, CreateLogRequest request)
        {
            var log = _mapper.Map<Log>(request);
            log.UserId = userId;
            log.Archived = false;
            log.CreatedAt = DateTime.Now;
            return log;
        }

        private Log MapUpdateLogRequestToLog(Log log, UpdateLogRequest request)
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

        // TODO: Put inside UserService

        private bool IsAuthenticated(int userId)
        {
            return userId == GetLoggedUserId() || IsLoggedUserManager();
        }

        private bool IsLoggedUserManager()
        {
            return GetLoggedUserRole() == EUserRole.MANAGER.GetName();
        }

        private int GetLoggedUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return int.Parse(userId);
        }

        private string GetLoggedUserRole()
        {
            return User.FindFirst(ClaimTypes.Role).Value;
        }
    }
}