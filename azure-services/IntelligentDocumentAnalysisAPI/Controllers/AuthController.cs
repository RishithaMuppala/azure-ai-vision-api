using IntelligentDocumentAnalysisAPI.DTOs;
using IntelligentDocumentAnalysisAPI.IServices;
using Microsoft.AspNetCore.Mvc;

namespace IntelligentDocumentAnalysisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _accountService;

        public AuthController(IAuthService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var result = await _accountService.LoginAsync(model);
            if (!string.IsNullOrEmpty(result?.Token))
            {
                return Ok(result);
            }
            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO model)
        {
            RegisterResponseDTO result = await _accountService.RegisterAsync(model);
            if (result.Status == "Error")
            {
                return Unauthorized(result);
            }
            else
            {
                return Ok(result);
            }
        }
    }
}
