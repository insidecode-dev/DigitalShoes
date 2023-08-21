using DigitalShoes.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Service.Abstractions
{
    public interface IUserProfileService
    {
        Task<ApiResponse> IncreaseBalanceAsync(decimal balance, HttpContext httpContext);
        Task<ApiResponse> AddProfileImageAsync(IFormFile image, HttpContext httpContext);
        Task<ApiResponse> UpdateProfileImageAsync(IFormFile image, HttpContext httpContext);
        Task<ApiResponse> DeleteProfileImageAsync(HttpContext httpContext);
    }
}
