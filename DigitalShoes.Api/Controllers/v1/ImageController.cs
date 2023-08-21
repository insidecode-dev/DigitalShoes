using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.ImageDTOs;
using DigitalShoes.Service.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;


namespace DigitalShoes.Api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ImageController : ControllerBase
    {

        private readonly IMageService _imageService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ImageController(IMageService imageService, IHttpContextAccessor httpContextAccessor)
        {
            _imageService = imageService;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Roles = "seller")]
        [HttpGet("{id:int}/GetImagesByShoeId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetImagesByShoeIdAsync([FromRoute] int? id)
        {
            var image = await _imageService.GetImageByShoeIdAsync(id, _httpContextAccessor.HttpContext);
            return StatusCode((int)image.StatusCode, image);
        }

        [Authorize(Roles = "seller")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        
        public async Task<IActionResult> GetAllShoeImagesAsync()
        {
            var image = await _imageService.GetAllShoeImagesAsync(_httpContextAccessor.HttpContext);
            return StatusCode((int)image.StatusCode, image);
        }

        [Authorize(Roles = "seller")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddShoeImageAsync([FromForm] ImageCreateDTO imageCreateDTO)
        {
            var image = await _imageService.CreateAsync(imageCreateDTO, _httpContextAccessor.HttpContext);
            return StatusCode((int)image.StatusCode, image);
        }

        [Authorize(Roles = "seller")]
        [HttpDelete(Name = "DeleteShoeImage")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> DeleteShoeImageAsync([FromBody] ImageDeleteDTO imageDeleteDTO)
        {

            var image = await _imageService.DeleteAsync(imageDeleteDTO, _httpContextAccessor.HttpContext);
            if (!image.IsSuccess)
            {
                return StatusCode((int)image.StatusCode, image);
            }
            return StatusCode((int)image.StatusCode);
        }        
    }
}