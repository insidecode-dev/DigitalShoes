using DigitalShoes.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IdentityModel.Tokens.Jwt;

namespace DigitalShoes.Api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ProductSailingController : ControllerBase
    {

        [HttpPost]
        public IActionResult AddProduct()
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult MyProducts()
        {
            return Ok();
        }




        [Authorize(Roles = "buyer")]
        [HttpPost("buyer")]
        public IActionResult GetBuyer()
        {
            return Ok("Buyer is here");
        }

        [Authorize(Roles = "admin")]
        [HttpPost("admin")]
        public IActionResult GetAdmin()
        {
            return Ok("Admin is here");
        }

        [Authorize(Roles = "admin,buyer")]
        [HttpPost]
        public IActionResult GetEveryOne()
        {
            return Ok("everyone is here");
        }



    }
}
