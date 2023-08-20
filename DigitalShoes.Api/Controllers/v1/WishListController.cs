using DigitalShoes.Domain.DTOs.CartItemDTOs;
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
    public class WishListController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWishListService _wishListService;

        public WishListController(IHttpContextAccessor httpContextAccessor, IWishListService wishListService)
        {
            _httpContextAccessor = httpContextAccessor;
            _wishListService = wishListService;
        }

        [Authorize(Roles = "buyer")]
        [HttpPost("{ShoeId:int}/AddToWishlist")]
        public async Task<IActionResult> AddToWishlistAsync([FromRoute] int? ShoeId)
        {
            var wishlist = await _wishListService.AddToWishlistAsync(ShoeId, _httpContextAccessor.HttpContext);
            return StatusCode((int)wishlist.StatusCode, wishlist);
        }

        [Authorize(Roles = "buyer")]
        [HttpDelete("{ShoeId:int}/RemoveFromWishlist")]
        public async Task<IActionResult> RemoveFromWishlistAsync([FromRoute] int? ShoeId)
        {
            var wishlist = await _wishListService.RemoveFromWishlistAsync(ShoeId, _httpContextAccessor.HttpContext);
            
            if (!wishlist.IsSuccess)
            {
                return StatusCode((int)wishlist.StatusCode, wishlist);
            }
            return StatusCode((int)wishlist.StatusCode);           
            
        }
        


    }
}
