using DigitalShoes.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using DigitalShoes.Domain.DTOs.CartItemDTOs;


namespace DigitalShoes.Service.Abstractions
{
    public interface ICartService
    {
        Task<ApiResponse> CreateCartAsync(HttpContext httpContext); 
        Task<ApiResponse> AddToCartAsync(List<CartItemCreateDTO> cartItemCreateDTO, HttpContext httpContext); 
        Task<ApiResponse> MyCartItemsAsync(HttpContext httpContext); 
        Task<ApiResponse> UpdateCartItemCountAsync(int? id, CartItemUpdateDTO cartItemUpdateDTO, HttpContext httpContext); 
        Task<ApiResponse> MyCartItemAsync(int? id, HttpContext httpContext); 
        Task<ApiResponse> RemoveCartItemAsync(int? id, HttpContext httpContext); //
    }
}
