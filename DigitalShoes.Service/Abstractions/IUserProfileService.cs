using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.UserProfileDTOs;
using Microsoft.AspNetCore.Http;

namespace DigitalShoes.Service.Abstractions
{
    public interface IUserProfileService
    {
        Task<ApiResponse> IncreaseBalanceAsync(decimal balance, HttpContext httpContext);
        Task<ApiResponse> AddProfileImageAsync(IFormFile image, HttpContext httpContext);
        Task<ApiResponse> UpdateProfileImageAsync(IFormFile image, HttpContext httpContext);
        Task<ApiResponse> DeleteProfileImageAsync(HttpContext httpContext);
        Task<ApiResponse> UpdateOrderAdressAsync(string? orderAdress, HttpContext httpContext);
        Task<ApiResponse> UpdateUserNameAsync(string? username, HttpContext httpContext);
        Task<ApiResponse> UpdatePasswordAsync(UpdatePasswordDTO updatePasswordDTO, HttpContext httpContext);
        Task<ApiResponse> DeleteMyAccountAsync(HttpContext httpContext);
    }
}
