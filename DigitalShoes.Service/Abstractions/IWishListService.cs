using DigitalShoes.Domain.DTOs.CartItemDTOs;
using DigitalShoes.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Service.Abstractions
{
    public interface IWishListService
    {        
        Task<ApiResponse> AddToWishlistAsync(int? ShoeId, HttpContext httpContext); 
        Task<ApiResponse> RemoveFromWishlistAsync(int? ShoeId, HttpContext httpContext); 
    }
}
