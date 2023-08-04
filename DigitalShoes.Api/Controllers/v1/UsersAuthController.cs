using DigitalShoes.Api.AuthOperations.Repositories;
using DigitalShoes.Api.AuthOperations.Services;
using DigitalShoes.Domain.DTOs;
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
        
        private readonly IUserService _userService;
        
        public UsersAuthController(IUserService userService)
        {   
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LogInRequestDto logInRequestDto)
        {            
            var apiResponse = await _userService.LogIn(logInRequestDto);

            if (!apiResponse.IsSuccess || apiResponse.ErrorMessages.Count>0)
            {                
                return BadRequest(apiResponse);
            }

            return Ok(apiResponse);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO registrationRequestDTO)
        {

            var apiResponse = await _userService.Register(registrationRequestDTO);
            if (!apiResponse.IsSuccess || apiResponse.ErrorMessages.Count>0)
            {                
                return BadRequest(apiResponse);
            }

            return Ok(apiResponse);
        }

        [Authorize]
        [HttpPost("mynewrole")]
        public async Task<IActionResult> AddMyNewRole([FromBody] MyNewRoleRequestDTO myNewRoleRequestDTO)
        {
            var apiResponse = await _userService.AddMyNewRole(myNewRoleRequestDTO);
            if (!apiResponse.IsSuccess || apiResponse.ErrorMessages.Count > 0)
            {
                return BadRequest(apiResponse);
            }

            return Ok(apiResponse);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("newrole")]
        public async Task<IActionResult> CreateNewRole([FromBody] NewRoleRequestDTO newRoleRequestDTO)
        {
            var apiResponse = await _userService.CreateNewRole(newRoleRequestDTO);
            if (!apiResponse.IsSuccess || apiResponse.ErrorMessages.Count > 0)
            {
                return BadRequest(apiResponse);
            }

            return Ok(apiResponse);
        }
    }
}
