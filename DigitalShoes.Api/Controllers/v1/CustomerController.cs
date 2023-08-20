using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.PaymentDTOs;
using DigitalShoes.Service.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalShoes.Api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerController(ICustomerService customerService, IHttpContextAccessor httpContextAccessor)
        {
            _customerService = customerService;
            _httpContextAccessor = httpContextAccessor;
        }

        // buy shoe  (also completes payment and payment objects) (done)


        [Authorize(Roles = "buyer")]
        [HttpPost("BuyProductById")]
        public async Task<ActionResult<ApiResponse>> BuyProductByIdAsync([FromBody] TransactionDTO transactionDTO)
        {
            var category = await _customerService.BuyProductByIdAsync(transactionDTO, _httpContextAccessor.HttpContext);
            if (!category.IsSuccess)
            {
                return StatusCode((int)category.StatusCode, category);
            }
            return StatusCode((int)category.StatusCode);
        }

        [Authorize(Roles = "buyer")]
        [HttpPost("ApproveCart")]
        public async Task<ActionResult<ApiResponse>> ApproveCartAsync()
        {
            var category = await _customerService.ApproveCartAsync(_httpContextAccessor.HttpContext);
            if (!category.IsSuccess)
            {
                return StatusCode((int)category.StatusCode, category);
            }
            return StatusCode((int)category.StatusCode);
        }        
    }
}
