using DigitalShoes.Domain.DTOs.ShoeDTOs;
using DigitalShoes.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalShoes.Domain.DTOs.ImageDTOs;

namespace DigitalShoes.Service.Abstractions
{
    public interface IMageService
    {        
        Task<ApiResponse> CreateAsync(ImageCreateDTO imageCreateDTO, string username, HttpRequest httpRequest);
    }
}
