using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.ProductDTOs;
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

        public ProductOwnerController(IShoeService shoeService)
        {
            _shoeService = shoeService;
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddProduct(/*[FromBody] ShoeCreateDTO shoeCreateDTO*/)
        {
            if (ModelState.IsValid)
            {

            }

            var x = HttpContext.Request;

            string username = HttpContext.User.Identities.FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))
        ?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;
            _shoeService.Create(new ShoeCreateDTO(), username, x );
            return Ok();
        }

        [HttpPut("add product image")]
        public IActionResult AddProductImage([FromBody] ShoeCreateDTO shoeCreateDTO)
        {
            return Ok();
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
