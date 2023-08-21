using AutoMapper;
using DigitalShoes.Dal.Context;
using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.PaymentDTOs;
using DigitalShoes.Domain.Entities;
using DigitalShoes.Service.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;


namespace DigitalShoes.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _dbContext;
        //
        private readonly IMapper _mapper;
        protected ApiResponse _apiResponse;
        //
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentService(ApplicationDbContext dbContext, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _apiResponse = new();
            _userManager = userManager;
        }

        public async Task<ApiResponse> GetMyPaymentsAsync(HttpContext httpContext)
        {
            string username = httpContext
            .User
            .Identities
            .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
            .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
            .Value;

            var user = await _userManager
                .Users
                .Include(p => p.Payments)
                .ThenInclude(po => po.PaymentObjects)
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user.Payments.Count == 0)
            {
                _apiResponse.ErrorMessages.Add($"you don't have any payments");
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                return _apiResponse;
            }

            var payment = _mapper.Map<List<PaymentGetDTO>>(user.Payments);

            // response
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Result = payment;
            return _apiResponse;
        }

        public async Task<ApiResponse> GetMyPaymentByIdAsync(int? PaymentId, HttpContext httpContext)
        {
            if (PaymentId == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add($"id is null");
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return _apiResponse;
            }

            string username = httpContext
            .User
            .Identities
            .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
            .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
            .Value;

            var user = await _userManager
                .Users
                .Include(p => p.Payments)
                .ThenInclude(po => po.PaymentObjects)
                .FirstOrDefaultAsync(u => u.UserName == username);

            var payment = user.Payments.Where(x => x.Id == PaymentId).FirstOrDefault();
            if (payment is null)
            {
                _apiResponse.ErrorMessages.Add($"you don't have payment with {PaymentId} id");
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                return _apiResponse;
            }

            // response
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Result = _mapper.Map<PaymentGetDTO>(payment);
            return _apiResponse;
        }
    }
}
