using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Report.Api.Dto.Requests;
using Report.Api.Extensions;
using Report.Core.Services;

namespace Report.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class LogController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogService _logService;

        public LogController(IMapper mapper, ILogService service)
        {
            _mapper = mapper;
            _logService = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _logService.GetAll();
            return StatusCode(response.Code, response);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetLogsByUserId(int userId)
        {
            var response = await _logService.GetLogsByUserId(userId);
            return StatusCode(response.Code, response);
        }

        [HttpGet("unarchived/user/{userId}")]
        public async Task<IActionResult> GetUnarchivedLogsByUserId(int userId)
        {
            var response = await _logService.GetUnarchivedLogsByUserId(userId);
            return StatusCode(response.Code, response);
        }

        [HttpGet("archived/user/{userId}")]
        public async Task<IActionResult> GetArchivedLogsByUserId(int userId)
        {
            var response = await _logService.GetArchivedLogsByUserId(userId);
            return StatusCode(response.Code, response);
        }

        [HttpGet("{logId}")]
        public async Task<IActionResult> GetLogById(int logId)
        {
            var response = await _logService.GetLogById(logId);
            return StatusCode(response.Code, response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateLogRequest request)
        {
            var mapped = request.MapToCoreRequest(_mapper);
            var response = await _logService.Create(mapped);
            return StatusCode(response.Code, response);
        }

        [HttpPut("{logId}")]
        public async Task<IActionResult> Put(int logId, UpdateLogRequest request)
        {
            var mapped = request.MapToCoreRequest(_mapper);
            var response = await _logService.Update(logId, mapped);
            return StatusCode(response.Code, response);
        }

        [HttpDelete("{logId}")]
        public async Task<IActionResult> Delete(int logId)
        {
            var response = await _logService.Delete(logId);
            return StatusCode(response.Code, response);
        }

        [HttpPatch("archive/{logId}")]
        public async Task<IActionResult> Archive(int logId)
        {
            var response = await _logService.Archive(logId);
            return StatusCode(response.Code, response);
        }
    }
}