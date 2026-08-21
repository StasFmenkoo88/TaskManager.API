using Microsoft.AspNetCore.Mvc;
using TaskManager.API.DTOs;
using TaskManager.API.Interfaces;

namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = await _authService.RegisterAsync(dto);

            return StatusCode(201, user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);

            return Ok(token);
        }
    }
}