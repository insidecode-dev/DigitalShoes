using DigitalShoes.Api.AuthOperations.Repositories;
using DigitalShoes.Api.AuthOperations.Services;
using DigitalShoes.Domain.DTOs.AuthDTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
namespace DigitalShoes.Api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class UsersAuthController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserService _userService;

        public UsersAuthController(IUserService userService, IHttpContextAccessor contextAccessor)
        {
            _userService = userService;
            _contextAccessor = contextAccessor;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LogInRequestDto logInRequestDto)
        {            
            var apiResponse = await _userService.LogIn(logInRequestDto);
            return StatusCode((int)apiResponse.StatusCode, apiResponse);            
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO registrationRequestDTO)
        {
            var apiResponse = await _userService.Register(registrationRequestDTO);
            return StatusCode((int)apiResponse.StatusCode, apiResponse);
        }

        [Authorize]
        [HttpPost("mynewrole")]
        public async Task<IActionResult> AddMyNewRole([FromBody] MyNewRoleRequestDTO myNewRoleRequestDTO)
        {
            var apiResponse = await _userService.AddMyNewRole(myNewRoleRequestDTO);
            return StatusCode((int)apiResponse.StatusCode, apiResponse);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("newrole")]
        public async Task<IActionResult> CreateNewRole([FromBody] NewRoleRequestDTO newRoleRequestDTO)
        {
            var apiResponse = await _userService.CreateNewRole(newRoleRequestDTO);
            return StatusCode((int)apiResponse.StatusCode, apiResponse);
        }

        [HttpPost("logout")]
        [Authorize] 
        public async Task<IActionResult> Logout()
        {            
            await _contextAccessor.HttpContext.SignOutAsync();

            return Ok(new { message = "Logout successful" });
        }
    }
}