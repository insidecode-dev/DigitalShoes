using DigitalShoes.Domain.Entities;
using DigitalShoes.Service.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DigitalShoes.Api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPaymentService _paymentService;

        public PaymentController(IHttpContextAccessor httpContextAccessor, IPaymentService paymentService)
        {
            _httpContextAccessor = httpContextAccessor;
            _paymentService = paymentService;
        }

        [Authorize(Roles = "buyer")]
        [HttpGet("GetMyPayments")]
        public async Task<IActionResult> GetMyPaymentsAsync()
        {
            var payment = await _paymentService.GetMyPaymentsAsync(_httpContextAccessor.HttpContext);
            return StatusCode((int)payment.StatusCode, payment);
        }

        [Authorize(Roles = "buyer")]
        [HttpGet("{PaymentId:int}/GetMyPaymentById")]
        public async Task<IActionResult> GetMyPaymentByIdAsync([FromRoute] int? PaymentId)
        {
            var payment = await _paymentService.GetMyPaymentByIdAsync(PaymentId, _httpContextAccessor.HttpContext);
            return StatusCode((int)payment.StatusCode, payment);
        }        
    }
}
