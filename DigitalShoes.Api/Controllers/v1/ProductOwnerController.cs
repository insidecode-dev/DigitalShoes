using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.CategoryDTOs;
using DigitalShoes.Domain.DTOs.ImageDTOs;
using DigitalShoes.Domain.DTOs.ShoeDTOs;
using DigitalShoes.Domain.Entities;
using DigitalShoes.Service.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DigitalShoes.Api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ProductOwnerController : ControllerBase
    {
        private readonly IShoeService _shoeService;
        private readonly IMageService _imageService;
        private readonly ICategoryService _categoryService;
        public ProductOwnerController(IShoeService shoeService, IMageService imageService, ICategoryService categoryService)
        {
            _shoeService = shoeService;
            _imageService = imageService;
            _categoryService = categoryService;
        }


        [Authorize]
        [HttpGet("GetMyProducts")]
        public async Task<IActionResult> GetMyProductsAsync()
        {
            string username = HttpContext
                .User
                .Identities
                .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
                .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
                .Value;
            var shoes = await _shoeService.GetAllAsync(username);
            if (!shoes.IsSuccess)
            {
                return BadRequest(shoes);
            }
            return Ok(shoes);
        }

        [Authorize]
        [HttpGet("{id:int}/GetMyProductById")]
        public async Task<IActionResult> GetMyProductByIdAsync([FromRoute] int? id)
        {
            string username = HttpContext
                .User
                .Identities
                .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
                .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
                .Value;
            var shoes = await _shoeService.GetByIdAsync(id, username);
            if (!shoes.IsSuccess)
            {
                return BadRequest(shoes);
            }
            return Ok(shoes);
        }

        [Authorize]
        [HttpPost("AddShoe")]
        public async Task<IActionResult> AddShoeAsync([FromBody] ShoeCreateDTO shoeCreateDTO)
        {
            string username = HttpContext
                .User
                .Identities
                .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
                .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
                .Value;
            var shoe = await _shoeService.CreateAsync(shoeCreateDTO, username);
            if (!shoe.IsSuccess)
            {
                return BadRequest(shoe);
            }
            return Ok(shoe);
        }

        [Authorize]
        [HttpPut("{id:int}/UpdateShoe")]
        public async Task<IActionResult> UpdateShoeAsync([FromRoute] int? id, [FromBody] ShoeUpdateDTO shoeUpdateDTO)
        {
            string username = HttpContext
                .User
                .Identities
                .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
                .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
                .Value;
            var shoe = await _shoeService.UpdateAsync(id, shoeUpdateDTO, username);
            if (!shoe.IsSuccess)
            {
                return BadRequest(shoe);
            }
            return Ok(shoe);
        }

        [Authorize]
        [HttpDelete("{id:int}/DeleteProductById")]
        public async Task<IActionResult> DeleteProductByIdAsync([FromRoute] int? id)
        {
            string username = HttpContext
                .User
                .Identities
                .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
                .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
                .Value;
            var shoes = await _shoeService.DeleteProductByIdAsync(id, username);
            if (!shoes.IsSuccess)
            {
                return BadRequest(shoes);
            }
            return Ok(shoes);
        }

        [Authorize]
        [HttpPost("AddShoeImage")]
        public async Task<IActionResult> AddShoeImageAsync([FromForm] ImageCreateDTO imageCreateDTO)
        {
            string username = HttpContext
                .User
                .Identities
                .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
                .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
                .Value;
            var request = HttpContext.Request;
            var image = await _imageService.CreateAsync(imageCreateDTO, username, request);
            if (!image.IsSuccess)
            {
                return BadRequest(image);
            }

            return Ok(image);
        }

        [Authorize]
        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategoryAsync([FromForm] CategoryCreateDTO categoryCreateDTO)
        {
            var category = await _categoryService.CreateAsync(categoryCreateDTO);
            if (!category.IsSuccess)
            {
                return BadRequest(category);
            }

            return Ok(category);
        }

        [HttpPut]
        public IActionResult UpdateProduct()
        {
            return Ok();
        }

        [HttpPut("remove product image")]
        public IActionResult RemoveProductImage()
        {
            return Ok();
        }

        [HttpDelete]
        public IActionResult RemoveProduct()
        {
            return Ok();
        }

        [HttpGet]
        public IActionResult GetMyProducts()
        {
            return Ok();
        }



















        // test 

        //[Authorize(Roles = "buyer")]
        //[HttpPost("buyer")]
        //public IActionResult GetBuyer()
        //{
        //    return Ok("Buyer is here");
        //}

        //[Authorize(Roles = "admin")]
        //[HttpPost("admin")]
        //public IActionResult GetAdmin()
        //{
        //    return Ok("Admin is here");
        //}

        //[Authorize(Roles = "admin,buyer")]
        //[HttpPost]
        //public IActionResult GetEveryOne()
        //{
        //    return Ok("everyone is here");
        //}



    }
}
