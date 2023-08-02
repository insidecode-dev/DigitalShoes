using Microsoft.AspNetCore.Mvc;

namespace DigitalShoes.Api.Controllers.v2
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoeSalesController : ControllerBase
    {
        [Route("api/v{version:apiVersion}/[controller]")]
        [ApiVersion("2.0")]
        [HttpPost]
        public IActionResult CreateShoe(int id)
        {
            return BadRequest();
        }

        [HttpPost("second")]
        public IActionResult CreateCategory(int id)
        {
            return BadRequest();
        }
    }
}
