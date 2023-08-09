using DigitalShoes.Domain.DTOs.ShoeDTOs;
using DigitalShoes.Domain.DTOs;


namespace DigitalShoes.Service.Abstractions
{
    public interface IShoeService
    {
        Task<ApiResponse> CreateAsync(ShoeCreateDTO shoeCreateDTO, string username);
        Task<ApiResponse> UpdateAsync(int? id, ShoeUpdateDTO shoeUpdateDTO, string username);
        Task<ApiResponse> GetAllAsync(string username);
        Task<ApiResponse> GetByIdAsync(int? id, string username);
        Task<ApiResponse> DeleteProductByIdAsync(int? id, string username);
    }
}
