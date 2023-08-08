using DigitalShoes.Domain.DTOs.ShoeDTOs;
using DigitalShoes.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalShoes.Domain.DTOs.CategoryDTOs;

namespace DigitalShoes.Service.Abstractions
{
    public interface ICategoryService
    {
        Task<ApiResponse> CreateAsync(CategoryCreateDTO categoryCreateDTO);
    }
}
