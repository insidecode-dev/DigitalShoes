using Azure;
using DigitalShoes.Domain.DTOs.CartItemDTOs;
using DigitalShoes.Domain.DTOs.CategoryDTOs;
using DigitalShoes.Domain.Entities;
using DigitalShoes.Service.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalShoes.Api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CartController : ControllerBase
    {
        //         get cart items
        //         create my cart (done)
        //         add to cart
        //         delete from cart

        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartController(ICartService cartService, IHttpContextAccessor httpContextAccessor)
        {
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Roles = "buyer")]
        [HttpPost]
        public async Task<IActionResult> CreateCartAsync()
        {
            var cart = await _cartService.CreateCartAsync(_httpContextAccessor.HttpContext);
            return StatusCode((int)cart.StatusCode, cart);
        }


        [Authorize(Roles = "buyer")]
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCartAsync([FromBody] List<CartItemCreateDTO> cartItemCreateDTO)
        {
            var cart = await _cartService.AddToCartAsync(cartItemCreateDTO, _httpContextAccessor.HttpContext);
            return StatusCode((int)cart.StatusCode, cart);
        }

        [Authorize(Roles = "buyer")]
        [HttpGet("MyCartItems")]
        public async Task<IActionResult> MyCartItemsAsync()
        {
            var cart = await _cartService.MyCartItemsAsync(_httpContextAccessor.HttpContext);
            return StatusCode((int)cart.StatusCode, cart);
        }

        [Authorize(Roles = "buyer")]
        [HttpPut("{id:int}/UpdateCartItemCount", Name ="UpdateCartItemCount")]
        public async Task<IActionResult> UpdateCartItemCountAsync(int? id, CartItemUpdateDTO cartItemUpdateDTO)
        {
            var cart = await _cartService.UpdateCartItemCountAsync(id, cartItemUpdateDTO, _httpContextAccessor.HttpContext);
            return StatusCode((int)cart.StatusCode);
        }
        
        // Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InVzZXJfYnV5ZXIiLCJlbWFpbCI6InVzZXJidXllckBnbWFpbC5jb20iLCJyb2xlIjoiYnV5ZXIiLCJuYmYiOjE2OTIyNzg5NzEsImV4cCI6MTY5Mjg4Mzc3MSwiaWF0IjoxNjkyMjc4OTcxfQ.5KN5NTLIomOqPA5882N_ewyNKnyM2h9AqB-SBHmSI5g
    }
}
