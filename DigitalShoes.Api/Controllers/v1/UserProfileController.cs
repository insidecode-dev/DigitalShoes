using DigitalShoes.Dal.Context;
using DigitalShoes.Domain.DTOs.UserProfileDTOs;
using DigitalShoes.Domain.Entities;
using DigitalShoes.Service;
using DigitalShoes.Service.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly ApplicationDbContext _applicationDbContext;

        public UserProfileController(IHttpContextAccessor httpContextAccessor, IUserProfileService userProfileService, ApplicationDbContext applicationDbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _userProfileService = userProfileService;
            _applicationDbContext = applicationDbContext;
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
        [HttpPut("UpdateOrderAdress")]
        public async Task<IActionResult> UpdateOrderAdressAsync([FromQuery] string? orderAdress)
        {
            var response = await _userProfileService.UpdateOrderAdressAsync(orderAdress, _httpContextAccessor.HttpContext);
            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return StatusCode((int)response.StatusCode);
        }

        [Authorize]
        [HttpPut("UpdateUserName")]
        public async Task<IActionResult> UpdateUserNameAsync([FromQuery] string? username)
        {
            var response = await _userProfileService.UpdateUserNameAsync(username, _httpContextAccessor.HttpContext);            
            return StatusCode((int)response.StatusCode, response);                        
        }

        [Authorize]
        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePasswordAsync([FromBody] UpdatePasswordDTO updatePasswordDTO)
        {
            var response = await _userProfileService.UpdatePasswordAsync(updatePasswordDTO, _httpContextAccessor.HttpContext);
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

        [HttpDelete("DeleteMyAccount")]
        public async Task<IActionResult> DeleteMyAccountAsync()
        {
            var response = await _userProfileService.DeleteMyAccountAsync(_httpContextAccessor.HttpContext);
            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return StatusCode((int)response.StatusCode);            
        }
    }
}


// Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InVzZXJfYnV5ZXIiLCJlbWFpbCI6InVzZXJidXllckBnbWFpbC5jb20iLCJyb2xlIjoiYnV5ZXIiLCJuYmYiOjE2OTI5NzkzODEsImV4cCI6MTY5Mjk3OTk4MSwiaWF0IjoxNjkyOTc5MzgxLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MjQ5LyIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjcyNDkvIn0.p31VEItWWC_VloW2K9Vs9o0l3dRrBVXYenlpJBIWWk0