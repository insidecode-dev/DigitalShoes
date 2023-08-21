using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.ReviewDTOs;
using DigitalShoes.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Service.Abstractions
{
    public interface IReviewService
    {
        Task<ApiResponse> AddReviewAsync(ReviewCreateDTO reviewCreateDTO, HttpContext httpContext);
        Task<ApiResponse> GetByIdAsync(int? ReviewId, HttpContext httpContext);
        Task<ApiResponse> GetMyReviewsByShoeIdAsync(int? ShoeId, HttpContext httpContext);
        Task<ApiResponse> UpdateReviewAsync(ReviewUpdateDTO reviewUpdateDTO, HttpContext httpContext); 
        Task<ApiResponse> RemoveReviewAsync(int? ReviewId, HttpContext httpContext); 
    }
}
