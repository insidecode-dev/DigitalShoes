
using AutoMapper;
using DigitalShoes.Dal.Repository.Interfaces;
using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.CategoryDTOs;
using DigitalShoes.Domain.DTOs.ImageDTOs;
using DigitalShoes.Domain.DTOs.ShoeDTOs;
using DigitalShoes.Domain.Entities;
using DigitalShoes.Domain.FluentValidators;
using DigitalShoes.Service.Abstractions;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        //
        private readonly IMapper _mapper;
        protected ApiResponse _apiResponse;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _apiResponse = new();
        }
        public async Task<ApiResponse> CreateAsync(CategoryCreateDTO categoryCreateDTO)
        {
            try
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

                var existingCategory = await _categoryRepository.GetAsync(ct=>ct.Name==categoryCreateDTO.Name);
                if (existingCategory != null)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"{categoryCreateDTO.Name} category exists");                    
                    _apiResponse.Result = categoryCreateDTO;
                    return _apiResponse;
                }

                var category = _mapper.Map<Category>(categoryCreateDTO);
                await _categoryRepository.CreateAsync(category);

                var categoryDTO = _mapper.Map<CategoryDTO>(category);
                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                _apiResponse.Result = categoryDTO;
                return _apiResponse;
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add(ex.Message);
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.Result = ex;
                return _apiResponse;                
            }
        }
    }
}
