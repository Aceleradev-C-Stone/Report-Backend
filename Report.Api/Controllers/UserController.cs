using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Report.Api.Dto.Requests;
using Microsoft.AspNetCore.Authorization;
using Report.Core.Services;

namespace Report.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public UserController(IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _userService.Get();
            return StatusCode(response.Code, response);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var response = await _userService.GetUserById(userId);
            return StatusCode(response.Code, response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateUserRequest request)
        {
            var mapped = _mapper.Map<Core.Dto.Requests.CreateUserRequest>(request);
            var response = await _userService.Create(mapped);
            return StatusCode(response.Code, response);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> Put(int userId, UpdateUserRequest request)
        {
            var mapped = _mapper.Map<Core.Dto.Requests.UpdateUserRequest>(request);
            var response = await _userService.Update(userId, mapped);
            return StatusCode(response.Code, response);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(int userId)
        {
            var response = await _userService.Delete(userId);
            return StatusCode(response.Code, response);
        }
    }
}