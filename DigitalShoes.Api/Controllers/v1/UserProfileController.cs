using DigitalShoes.Domain.Entities;
using DigitalShoes.Service;
using DigitalShoes.Service.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DigitalShoes.Api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IHttpContextAccessor httpContextAccessor, IUserProfileService userProfileService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userProfileService = userProfileService;
        }

        [Authorize]
        [HttpPut("IncreaseBalance")]
        public async Task<IActionResult> IncreaseBalanceAsync([FromQuery] decimal balance)
        {
            var response = await _userProfileService.IncreaseBalanceAsync(balance, _httpContextAccessor.HttpContext);            
            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return StatusCode((int)response.StatusCode);
        }

        [Authorize]
        [HttpPost("AddProfileImage")]
        public async Task<IActionResult> AddProfileImageAsync(IFormFile image)
        {
            var response = await _userProfileService.AddProfileImageAsync(image, _httpContextAccessor.HttpContext);            
            return StatusCode((int)response.StatusCode, response);
        }

        [Authorize]
        [HttpPut("UpdateProfileImage")]
        public async Task<IActionResult> UpdateProfileImageAsync(IFormFile image)
        {
            var response = await _userProfileService.UpdateProfileImageAsync(image, _httpContextAccessor.HttpContext);
            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return StatusCode((int)response.StatusCode);
        }

        [Authorize]
        [HttpDelete("DeleteProfileImage")]
        public async Task<IActionResult> DeleteProfileImageAsync()
        {
            var response = await _userProfileService.DeleteProfileImageAsync(_httpContextAccessor.HttpContext);
            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return StatusCode((int)response.StatusCode);
        }
    }
}
