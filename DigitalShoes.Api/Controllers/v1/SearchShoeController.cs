using DigitalShoes.Domain.DTOs;
using DigitalShoes.Service.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalShoes.Api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SearchShoeController : ControllerBase
    {
        private readonly IShoeService _shoeService;

        public SearchShoeController(IShoeService shoeService)
        {
            _shoeService = shoeService;
        }

        [Authorize]
        [HttpGet("{hashtag}", Name = "SearchShoeByHashtag")]
        public async Task<ActionResult<ApiResponse>> SearchShoeByHashtagAsync([FromRoute] string? hashtag)
        {
            var shoes = await _shoeService.SearchByHashtagAsync(hashtag);
            return StatusCode((int)shoes.StatusCode, shoes);
        }
    }
}
