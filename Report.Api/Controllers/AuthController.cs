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
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public AuthController(IMapper mapper, IAuthService authService)
        {
            _mapper = mapper;
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Authenticate(LoginUserRequest request)
        {
            var mapped = request.MapToCoreRequest(_mapper);
            var response = await _authService.Authenticate(mapped);
            return StatusCode(response.Code, response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserRequest request)
        {
            var mapped = request.MapToCoreRequest(_mapper);
            var response = await _authService.Register(mapped);
            return StatusCode(response.Code, response);
        }
    }
}