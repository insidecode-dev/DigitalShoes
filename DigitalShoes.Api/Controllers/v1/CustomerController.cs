using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.PaymentDTOs;
using DigitalShoes.Service.Abstractions;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "buyer")]
        [HttpPost("BuyProductById")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> BuyProductByIdAsync([FromBody] TransactionDTO transactionDTO, string? orderAdress = null)
        {
            var category = await _customerService.BuyProductByIdAsync(transactionDTO, _httpContextAccessor.HttpContext, orderAdress);
            if (!category.IsSuccess)
            {
                return StatusCode((int)category.StatusCode, category);
            }
            return StatusCode((int)category.StatusCode);
        }

        [Authorize(Roles = "buyer")]
        [HttpPost("ApproveCart")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> ApproveCartAsync([FromQuery] string? orderAdress = null)
        {
            var category = await _customerService.ApproveCartAsync(_httpContextAccessor.HttpContext, orderAdress);
            if (!category.IsSuccess)
            {
                return StatusCode((int)category.StatusCode, category);
            }
            return StatusCode((int)category.StatusCode);
        }
    }
}