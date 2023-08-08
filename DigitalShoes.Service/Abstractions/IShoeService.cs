using DigitalShoes.Domain.DTOs.ShoeDTOs;
using DigitalShoes.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using DigitalShoes.Domain.DTOs.ShoeDTOs;

namespace DigitalShoes.Service.Abstractions
{
    public interface IShoeService
    {
        Task<ApiResponse> CreateAsync(ShoeCreateDTO shoeCreateDTO, string username);
    }
}
