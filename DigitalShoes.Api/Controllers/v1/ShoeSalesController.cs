using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DigitalShoes.Api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ShoeSalesController : ControllerBase
    {

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "buyer")]
        [HttpPost("buyer")]
        public IActionResult GetBuyer()
        {
            return Ok("Buyer is here");
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin")]
        [HttpPost("admin")]
        public IActionResult GetAdmin()
        {
            return Ok("Admin is here");
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin,buyer")]
        [HttpPost]
        public IActionResult GetEveryOne()
        {
            return Ok("everyone is here");
        }
    }
}
