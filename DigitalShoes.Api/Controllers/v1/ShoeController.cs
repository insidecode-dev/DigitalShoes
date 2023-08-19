using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.ShoeDTOs;
using DigitalShoes.Domain.Entities;
using DigitalShoes.Service.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace DigitalShoes.Api.Controllers.v1
{    
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    
    public class ShoeController : ControllerBase
    {
        private readonly IShoeService _shoeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ShoeController(IShoeService shoeService, IHttpContextAccessor httpContextAccessor)
        {
            _shoeService = shoeService;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMyProductsAsync()
        {   
            var shoes = await _shoeService.GetAllAsync(_httpContextAccessor.HttpContext);            
            return StatusCode((int)shoes.StatusCode, shoes);
        }

        [Authorize]
        [HttpGet("{id:int}", Name = "GetShoeById")]
        public async Task<ActionResult<ApiResponse>> GetMyProductByIdAsync([FromRoute]int? id)
        {            
            var shoes = await _shoeService.GetByIdAsync(id, _httpContextAccessor.HttpContext);            
            return StatusCode((int)shoes.StatusCode, shoes);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> AddShoeAsync([FromBody] ShoeCreateDTO shoeCreateDTO)
        {
            
            var shoe = await _shoeService.CreateAsync(shoeCreateDTO, _httpContextAccessor.HttpContext);
            if (shoe.IsSuccess)
            {
                return CreatedAtRoute("GetShoeById", routeValues: new { id = (shoe.Result as ShoeGetDTO).Id }, value: shoe);
            }
            return StatusCode((int)shoe.StatusCode, shoe);
        }

        [Authorize]
        [HttpPut("{id:int}", Name ="UpdateShoe")]
        public async Task<IActionResult> UpdateShoeAsync([FromRoute] int? id, [FromBody] ShoeUpdateDTO shoeUpdateDTO)
        {   
            var shoe = await _shoeService.UpdateAsync(id, shoeUpdateDTO, _httpContextAccessor.HttpContext);
            if (shoe.IsSuccess)
            {
                return StatusCode((int)shoe.StatusCode);
            }
            return StatusCode((int)shoe.StatusCode, shoe);
        }

        [Authorize]
        [HttpDelete("{id:int}", Name = "DeleteShoeById")]
        public async Task<IActionResult> DeleteShoeByIdAsync([FromRoute] int? id)
        {
            
            var shoe = await _shoeService.DeleteProductByIdAsync(id, _httpContextAccessor.HttpContext);            
            return StatusCode((int)shoe.StatusCode);
        }

        
    }
}


//{
//    "brand": "string_test2",
//      "model": "string_test3",
//  "count": 2,
//  "size": 43,
//  "description": "string",
//  "gender": "man",
//  "color": "yellow",
//  "ctName": "string"

//}