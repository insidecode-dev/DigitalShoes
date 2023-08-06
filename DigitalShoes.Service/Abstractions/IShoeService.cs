using DigitalShoes.Domain.DTOs.ProductDTOs;
using DigitalShoes.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DigitalShoes.Service.Abstractions
{
    public interface IShoeService
    {
        Task<ApiResponse> Create(ShoeCreateDTO shoeCreateDTO, string username, HttpRequest httpRequest);
    }
}
