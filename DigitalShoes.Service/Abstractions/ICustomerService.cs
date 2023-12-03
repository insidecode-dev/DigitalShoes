using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.PaymentDTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Service.Abstractions
{
    public interface ICustomerService
    {
        Task<ApiResponse> BuyProductByIdAsync(TransactionDTO transactionDTO, HttpContext httpContext, string? orderAdress = null);
        Task<ApiResponse> ApproveCartAsync(HttpContext httpContext, string? orderAdress = null); //
    }
}
