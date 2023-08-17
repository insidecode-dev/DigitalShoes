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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddShoeImageAsync([FromForm] ImageCreateDTO imageCreateDTO)
        {
            var image = await _imageService.CreateAsync(imageCreateDTO, _httpContextAccessor.HttpContext);
            return StatusCode((int)image.StatusCode, image);
        }

        [Authorize]
        [HttpDelete(Name = "DeleteShoeImage")]
        public async Task<ActionResult<ApiResponse>> DeleteShoeImageAsync([FromBody] ImageDeleteDTO imageDeleteDTO)
        {
            try
            {
                var image = await _imageService.DeleteAsync(imageDeleteDTO, _httpContextAccessor.HttpContext);
                return StatusCode((int)image.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex);
            }
        }


        // Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6Imluc2lkZWNvZGVfYnV5IiwiZW1haWwiOiJpbnNpZGVjb2RlQGdtYWlsLmNvbSIsInJvbGUiOiJhZG1pbiIsIm5iZiI6MTY5MjExOTUwNywiZXhwIjoxNjkyNzI0MzA3LCJpYXQiOjE2OTIxMTk1MDd9.-hJ7aRiDf3DsDENnqd6OSgLgHqaUBfg-4OE3KoktRf0

    }
}