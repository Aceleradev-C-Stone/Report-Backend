using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Report.Api.Authorization;
using Report.Domain.Commands;
using Report.Domain.Enums;
using Report.Domain.Models;
using Report.Domain.Repositories;

namespace Report.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LogController : ControllerBase
    {
        private readonly ILogRepository _repository;

        public LogController(ILogRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Authorize]
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

        [HttpGet("{logId}")]
        [Authorize]
        public async Task<IActionResult> GetLogById(int logId)
        {
            try
            {
                var result = await _repository.GetById(logId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: { ex.Message }");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(CreateLogCommand command)
        {
            try
            {
                var log = new Log()
                {
                    Description = command.Description,
                    Title       = command.Title,
                    Details     = command.Details,
                    Source      = command.Source,
                    EventCount  = command.EventCount,
                    Level       = command.Level,
                    Channel     = command.Channel,
                    CreatedAt   = DateTime.Now,
                    UserId      = 1
                };

                _repository.Add(log);

                if (await _repository.SaveChangesAsync())
                    return Ok(log);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: { ex.Message }");
            }

            return BadRequest();
        }

        [HttpPut("{logId}")]
        [Authorize]
        public async Task<IActionResult> Put(int logId, CreateLogCommand command)
        {
            try
            {
                var log = await _repository.GetById(logId);

                if (log == null)
                    return NotFound("Log não encontrado");

                log.Description = command.Description;
                log.Title       = command.Title;
                log.Details     = command.Details;
                log.Source      = command.Source;
                log.EventCount  = command.EventCount;
                log.Level       = command.Level;
                log.Channel     = command.Channel;

                _repository.Update(log);

                if (await _repository.SaveChangesAsync())
                    return Ok(log);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: { ex.Message }");
            }

            return BadRequest();
        }

        [HttpDelete("{logId}")]
        [AuthorizeUserRoles(EUserRole.MANAGER)]
        public async Task<IActionResult> Delete(int logId)
        {
            try
            {
                var log = await _repository.GetById(logId);
                
                if (log == null)
                    return NotFound("Log não encontrado");
                
                _repository.Delete(log);

                if (await _repository.SaveChangesAsync())
                    return Ok("Log deletado");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: { ex.Message }");
            }

            return BadRequest();
        }
    }
}