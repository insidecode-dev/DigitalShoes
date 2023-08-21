using AutoMapper;
using DigitalShoes.Dal.Context;
using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.CategoryDTOs;
using DigitalShoes.Domain.Entities;
using DigitalShoes.Domain.FluentValidators;
using DigitalShoes.Service.Abstractions;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Net;


namespace DigitalShoes.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _dbContext;
        //
        private readonly IMapper _mapper;
        protected ApiResponse _apiResponse;
        public CategoryService(IMapper mapper, ApplicationDbContext dbContext)
        {
            _mapper = mapper;
            _apiResponse = new();
            _dbContext = dbContext;
        }
        public async Task<ApiResponse> CreateAsync(CategoryCreateDTO categoryCreateDTO)
        {
            ValidationResult categoryValidationResult = new CategoryCreateDTOValidator().Validate(categoryCreateDTO);
            if (!categoryValidationResult.IsValid)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                foreach (var error in categoryValidationResult.Errors)
                {
                    _apiResponse.ErrorMessages.Add(error.ErrorMessage);
                }
                _apiResponse.Result = categoryCreateDTO;
                return _apiResponse;
            }

            var existingCategory = await _dbContext.Categories.Where(ct => ct.Name == categoryCreateDTO.Name).FirstOrDefaultAsync();
            if (existingCategory != null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add($"{categoryCreateDTO.Name} category exists");
                _apiResponse.Result = categoryCreateDTO;
                return _apiResponse;
            }

            var category = _mapper.Map<Category>(categoryCreateDTO);
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            var categoryInDb = await _dbContext.Categories.Where(x => x.Name == categoryCreateDTO.Name).AsNoTracking().FirstOrDefaultAsync();
            if (categoryInDb != null)
            {
                var categoryDTO = _mapper.Map<CategoryDTO>(categoryInDb);
                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                _apiResponse.Result = categoryDTO;
                return _apiResponse;
            }

            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages.Add($"{categoryCreateDTO.Name} category was not created");
            _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
            return _apiResponse;
        }

        public async Task<ApiResponse> DeleteCategoryByIdAsync(int? id)
        {
            if (id is null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("id is null");
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return _apiResponse;
            }

            var existingCategory = await _dbContext.Categories.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (existingCategory != null)
            {
                _dbContext.Categories.Remove(existingCategory);
                await _dbContext.SaveChangesAsync();
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                _apiResponse.IsSuccess = true;
                return _apiResponse;
            }

            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages.Add($"category with {id} id does not exist");
            _apiResponse.StatusCode = HttpStatusCode.NotFound;
            return _apiResponse;
        }

        public async Task<ApiResponse> GetByIdAsync(int? id)
        {
            if (id is null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("id is null");
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return _apiResponse;
            }

            var existingCategory = await _dbContext.Categories.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (existingCategory != null)
            {
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.Result = _mapper.Map<CategoryDTO>(existingCategory);
                _apiResponse.IsSuccess = true;
                return _apiResponse;
            }

            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages.Add($"category with {id} id does not exist");
            _apiResponse.StatusCode = HttpStatusCode.NotFound;
            return _apiResponse;
        }
    }
}
