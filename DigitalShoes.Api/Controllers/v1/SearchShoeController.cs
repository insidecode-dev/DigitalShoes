using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.SearchDTOs;
using DigitalShoes.Service;
using DigitalShoes.Service.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DigitalShoes.Api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SearchShoeController : ControllerBase
    {        
        private readonly ISearchService _searchService;        
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SearchShoeController> _logger;
        public SearchShoeController(IHttpContextAccessor httpContextAccessor, ISearchService searchService, ILogger<SearchShoeController> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _searchService = searchService;
            _logger = logger;
        }


        [HttpGet("GetAllWithPagination")]
        public async Task<ActionResult<ApiResponse>> GetAllWithPaginationAsync([FromQuery] GetAllWithPaginationRequestDTO getAllWithPaginationRequestDTO)
        {
            var shoes = await _searchService.GetAllWithPaginationAsync(getAllWithPaginationRequestDTO, _httpContextAccessor.HttpContext);
            _logger.LogInformation("searcher products.......");
            return StatusCode((int)shoes.StatusCode, shoes);
        }

        [HttpGet("SearchShoeByFilter")]
        public async Task<ActionResult<ApiResponse>> SearchShoeByFilterAsync([FromQuery] GetShoeByFilterDTO getShoeByFilterDTO)
        {
            var shoes = await _searchService.SearchShoeByFilterAsync(getShoeByFilterDTO);
            return StatusCode((int)shoes.StatusCode, shoes);
        }

        [HttpGet("SearchShoeByHashtag")]
        public async Task<ActionResult<ApiResponse>> SearchShoeByHashtagAsync([FromQuery]SearchByHashtagRequestDTO searchByHashtagRequestDTO)
        {
            var shoes = await _searchService.SearchByHashtagAsync(searchByHashtagRequestDTO, _httpContextAccessor.HttpContext);
            return StatusCode((int)shoes.StatusCode, shoes);
        }

        
        [HttpGet("{ShoeId:int}/GetReviewsByShoeId")]
        public async Task<IActionResult> GetReviewsByShoeIdAsync([FromRoute] int? ShoeId)
        {
            var review = await _searchService.GetReviewsByShoeIdAsync(ShoeId);
            return StatusCode((int)review.StatusCode, review);
        }
    }
}
