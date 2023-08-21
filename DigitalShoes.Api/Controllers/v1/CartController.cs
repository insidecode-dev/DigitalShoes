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

        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartController(ICartService cartService, IHttpContextAccessor httpContextAccessor)
        {
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Roles = "buyer")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]        
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        
        public async Task<IActionResult> CreateCartAsync()
        {
            var cart = await _cartService.CreateCartAsync(_httpContextAccessor.HttpContext);
            return StatusCode((int)cart.StatusCode, cart);
        }


        [Authorize(Roles = "buyer")]
        [HttpPost("AddToCart")]        
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]        
        [ProducesResponseType(StatusCodes.Status404NotFound)]        
        public async Task<IActionResult> AddToCartAsync([FromBody] List<CartItemCreateDTO> cartItemCreateDTO)
        {
            var cart = await _cartService.AddToCartAsync(cartItemCreateDTO, _httpContextAccessor.HttpContext);
            return StatusCode((int)cart.StatusCode, cart);
        }

        [Authorize(Roles = "buyer")]
        [HttpGet("MyCartItems")]
        [ProducesResponseType(StatusCodes.Status200OK)]        
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]        
        public async Task<IActionResult> MyCartItemsAsync()
        {
            var cart = await _cartService.MyCartItemsAsync(_httpContextAccessor.HttpContext);
            return StatusCode((int)cart.StatusCode, cart);
        }

        [Authorize(Roles = "buyer")]
        [HttpGet("{id:int}/MyCartItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]        
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MyCartItemAsync([FromRoute] int? id)
        {
            var cart = await _cartService.MyCartItemAsync(id, _httpContextAccessor.HttpContext);
            return StatusCode((int)cart.StatusCode, cart);
        }

        [Authorize(Roles = "buyer")]
        [HttpPut("{id:int}/UpdateCartItemCount", Name ="UpdateCartItemCount")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]        
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCartItemCountAsync([FromRoute] int? id, CartItemUpdateDTO cartItemUpdateDTO)
        {
            var cart = await _cartService.UpdateCartItemCountAsync(id, cartItemUpdateDTO, _httpContextAccessor.HttpContext);
            if (!cart.IsSuccess)
            {
                return StatusCode((int)cart.StatusCode, cart);
            }
            return StatusCode((int)cart.StatusCode);
        }

        [Authorize(Roles = "buyer")]
        [HttpDelete("{id:int}/RemoveCartItem", Name = "RemoveCartItem")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]        
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveCartItemAsync([FromRoute] int? id)
        {
            var cart = await _cartService.RemoveCartItemAsync(id, _httpContextAccessor.HttpContext);
            if (!cart.IsSuccess)
            {
                return StatusCode((int)cart.StatusCode, cart);
            }
            return StatusCode((int)cart.StatusCode);
        }
    }
}
