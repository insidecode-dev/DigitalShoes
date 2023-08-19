using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.SearchDTOs;
using DigitalShoes.Service.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace DigitalShoes.Api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SearchShoeController : ControllerBase
    {        
        private readonly ISearchService _searchService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SearchShoeController(IHttpContextAccessor httpContextAccessor, ISearchService searchService)
        {            
            _httpContextAccessor = httpContextAccessor;
            _searchService = searchService;
        }



        [HttpGet("GetAllWithPagination")]
        public async Task<ActionResult<ApiResponse>> GetAllWithPaginationAsync([FromQuery] GetAllWithPaginationRequestDTO getAllWithPaginationRequestDTO)
        {
            var shoes = await _searchService.GetAllWithPaginationAsync(getAllWithPaginationRequestDTO, _httpContextAccessor.HttpContext);
            return StatusCode((int)shoes.StatusCode, shoes);
        }

        [HttpGet("SearchShoeByFilter")]
        public async Task<ActionResult<ApiResponse>> SearchShoeByFilterAsync([FromQuery] GetShoeByFilterDTO getShoeByFilterDTO)
        {
            var shoes = await _searchService.SearchShoeByFilterAsync(getShoeByFilterDTO, _httpContextAccessor.HttpContext);
            return StatusCode((int)shoes.StatusCode, shoes);
        }

        [HttpGet("SearchShoeByHashtag")]
        public async Task<ActionResult<ApiResponse>> SearchShoeByHashtagAsync([FromQuery]SearchByHashtagRequestDTO searchByHashtagRequestDTO)
        {
            var shoes = await _searchService.SearchByHashtagAsync(searchByHashtagRequestDTO, _httpContextAccessor.HttpContext);
            return StatusCode((int)shoes.StatusCode, shoes);
        }



        // all products with pagination
        // by rating with pagination
        // by price with pagination

    }
}
