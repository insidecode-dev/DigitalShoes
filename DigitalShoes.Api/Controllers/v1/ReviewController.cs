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
        //  get review for id item (done)
        //  get my reviews for ShoeId item (not authorized)
        //  get reviews items
        //  create review (done)        
        //  delete review (done)

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IReviewService _reviewService;

        public ReviewController(IHttpContextAccessor httpContextAccessor, IReviewService reviewService)
        {
            _httpContextAccessor = httpContextAccessor;
            _reviewService = reviewService;
        }

        [Authorize(Roles = "buyer")]
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

        [Authorize(Roles = "buyer")]
        [HttpGet("{ReviewId:int}", Name = "GetReviewById")]
        public async Task<IActionResult> GetReviewByIdAsync([FromRoute] int? ReviewId)
        {
            var review = await _reviewService.GetByIdAsync(ReviewId, _httpContextAccessor.HttpContext);
            return StatusCode((int)review.StatusCode, review);
        }


        

        [Authorize(Roles = "buyer")]
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

        [Authorize(Roles = "buyer")]
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
