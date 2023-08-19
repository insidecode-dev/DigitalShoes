using DigitalShoes.Domain.DTOs.SearchDTOs;
using DigitalShoes.Domain.DTOs;
using Microsoft.AspNetCore.Http;


namespace DigitalShoes.Service.Abstractions
{
    public interface ISearchService
    {
        Task<ApiResponse> SearchByHashtagAsync(SearchByHashtagRequestDTO searchByHashtagRequestDTO, HttpContext httpContext);
        Task<ApiResponse> GetAllWithPaginationAsync(GetAllWithPaginationRequestDTO getAllWithPaginationRequestDTO, HttpContext httpContext);
        Task<ApiResponse> SearchShoeByFilterAsync(GetShoeByFilterDTO getShoeByFilterDTO, HttpContext httpContext);
    }
}
