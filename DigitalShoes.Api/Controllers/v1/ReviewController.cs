using DigitalShoes.Domain.DTOs.ReviewDTOs;
using DigitalShoes.Service.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DigitalShoes.Api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ReviewController : ControllerBase
    {        

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IReviewService _reviewService;

        public ReviewController(IHttpContextAccessor httpContextAccessor, IReviewService reviewService)
        {
            _httpContextAccessor = httpContextAccessor;
            _reviewService = reviewService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReviewAsync([FromQuery] ReviewCreateDTO reviewCreateDTO)
        {
            var review = await _reviewService.AddReviewAsync(reviewCreateDTO, _httpContextAccessor.HttpContext);            
            if (review.IsSuccess)
            {
                return CreatedAtRoute("GetReviewById", routeValues: new { ReviewId = (review.Result as ReviewGetDTO).Id }, value: review);
            }
            return StatusCode((int)review.StatusCode, review);
        }

        [Authorize]
        [HttpGet("{ReviewId:int}", Name = "GetReviewById")]
        public async Task<IActionResult> GetReviewByIdAsync([FromRoute] int? ReviewId)
        {
            var review = await _reviewService.GetByIdAsync(ReviewId, _httpContextAccessor.HttpContext);
            return StatusCode((int)review.StatusCode, review);
        }

        [Authorize]
        [HttpGet("{ShoeId:int}/GetMyReviewsByShoeIdAsync")]
        public async Task<IActionResult> GetMyReviewsByShoeIdAsync([FromRoute] int? ShoeId)
        {
            var review = await _reviewService.GetMyReviewsByShoeIdAsync(ShoeId, _httpContextAccessor.HttpContext);
            return StatusCode((int)review.StatusCode, review);
        }

        [Authorize]
        [HttpPut("UpdateReview")]
        public async Task<IActionResult> UpdateReviewAsync([FromQuery] ReviewUpdateDTO reviewUpdateDTO)
        {
            var review = await _reviewService.UpdateReviewAsync(reviewUpdateDTO, _httpContextAccessor.HttpContext);
            if (!review.IsSuccess)
            {
                return StatusCode((int)review.StatusCode, review);
            }
            return StatusCode((int)review.StatusCode);
        }

        [Authorize]
        [HttpDelete("{ReviewId:int}/RemoveReview")]
        public async Task<IActionResult> RemoveReviewAsync([FromRoute] int? ReviewId)
        {
            var review = await _reviewService.RemoveReviewAsync(ReviewId, _httpContextAccessor.HttpContext);
            if (!review.IsSuccess)
            {
                return StatusCode((int)review.StatusCode, review);
            }
            return StatusCode((int)review.StatusCode);
        }
    }
}
