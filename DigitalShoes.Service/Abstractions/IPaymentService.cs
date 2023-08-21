using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Service.Abstractions
{
    public interface IPaymentService
    {
        Task<ApiResponse> GetMyPaymentsAsync(HttpContext httpContext);
        Task<ApiResponse> GetMyPaymentByIdAsync(int? PaymentId, HttpContext httpContext);
    }
}
