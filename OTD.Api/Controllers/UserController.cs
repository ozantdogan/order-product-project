using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OTD.Core.Models.Requests;
using OTD.ServiceLayer.Abstract;

namespace OTD.Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService userService)
        {
            _service = userService;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var response = await _service.Register(request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            var response = await _service.ConfirmEmail(request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _service.Login(request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("ResendOtp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest request)
        {
            var response = await _service.ResendConfirmationCode(request);
            return Ok(response);
        }
    }
}
