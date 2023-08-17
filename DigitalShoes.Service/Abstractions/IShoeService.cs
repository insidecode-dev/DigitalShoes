using DigitalShoes.Domain.DTOs.ShoeDTOs;
using DigitalShoes.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using DigitalShoes.Domain.Entities;

namespace DigitalShoes.Service.Abstractions
{
    public interface IShoeService
    {
        Task<ApiResponse> CreateAsync(ShoeCreateDTO shoeCreateDTO, HttpContext httpContext);
        Task<ApiResponse> UpdateAsync(int? id, ShoeUpdateDTO shoeUpdateDTO, HttpContext httpContext);
        Task<ApiResponse> GetAllAsync(HttpContext httpContext);
        Task<ApiResponse> GetByIdAsync(int? id, HttpContext httpContext);
        Task<ApiResponse> DeleteProductByIdAsync(int? id, HttpContext httpContext); 
        Task<ApiResponse> SearchByHashtagAsync(string? hashtag);
    }
}
