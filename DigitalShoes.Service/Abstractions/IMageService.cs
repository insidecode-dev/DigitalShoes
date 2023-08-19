using DigitalShoes.Domain.DTOs.ShoeDTOs;
using DigitalShoes.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalShoes.Domain.DTOs.ImageDTOs;
using Azure.Core;

namespace DigitalShoes.Service.Abstractions
{
    public interface IMageService
    {        
        Task<ApiResponse> CreateAsync(ImageCreateDTO imageCreateDTO, HttpContext httpContext);
        Task<ApiResponse> DeleteAsync(ImageDeleteDTO imageDeleteDTO, HttpContext httpContext); 
        Task<ApiResponse> GetAllShoeImagesAsync(HttpContext httpContext);  
        Task<ApiResponse> GetImageByShoeIdAsync(int? id, HttpContext httpContext);  
    }
}
