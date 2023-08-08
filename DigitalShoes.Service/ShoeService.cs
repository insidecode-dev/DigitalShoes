using DigitalShoes.Dal.Repository.Interfaces;
using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.ShoeDTOs;
using DigitalShoes.Service.Abstractions;
using DigitalShoes.Domain.FluentValidators;
using DigitalShoes.Domain.Entities;
using FluentValidation.Results;
using AutoMapper;
using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using static DigitalShoes.Domain.StaticDetails;
using Azure.Core;

namespace DigitalShoes.Service
{
    public class ShoeService : IShoeService
    {
        private readonly IShoeRepository _shoeRepository;        
        private readonly IHashtagRepository _hashtagRepository;
        private readonly IShoeHashtagRepository _shoeHashtagRepository;
        private readonly ICategoryRepository _categoryRepository;
        //
        private readonly IMapper _mapper;
        protected ApiResponse _apiResponse;
        private readonly IWebHostEnvironment _webHostEnvironment;
        //
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoeService(IMapper mapper, IShoeRepository shoeRepository, IWebHostEnvironment webHostEnvironment, IHashtagRepository hashtagRepository, IShoeHashtagRepository shoeHashtagRepository, UserManager<ApplicationUser> userManager, ICategoryRepository categoryRepository)
        {
            _mapper = mapper;
            _shoeRepository = shoeRepository;
            _apiResponse = new();            
            _webHostEnvironment = webHostEnvironment;
            _hashtagRepository = hashtagRepository;
            _shoeHashtagRepository = shoeHashtagRepository;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
        }

        public async Task<ApiResponse> CreateAsync(ShoeCreateDTO shoeCreateDTO, string username)
        {
            try
            {
                ValidationResult shoeCreateDTOValidationResult = new ShoeCreateDTOValidator().Validate(shoeCreateDTO);
                if (!shoeCreateDTOValidationResult.IsValid)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    foreach (var error in shoeCreateDTOValidationResult.Errors)
                    {
                        _apiResponse.ErrorMessages.Add(error.ErrorMessage);
                    }
                    _apiResponse.Result = shoeCreateDTOValidationResult;
                    return _apiResponse;
                }

                // checking if user have this product in database
                var user = await _userManager.FindByNameAsync(username);
                var existingShoe = await _shoeRepository.GetAsync(sh => sh.Brand == shoeCreateDTO.Brand && sh.Model == shoeCreateDTO.Model && sh.ApplicationUserId == user.Id, tracked:false);
                if (existingShoe != null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"You have {shoeCreateDTO.Model} mdoel of {shoeCreateDTO.Brand} brand in your products, you can increase size intead");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                // gender 
                if (!Enum.TryParse<Gender>(shoeCreateDTO.Gender, ignoreCase: true, out var gender))
                {                    
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"gender is not valid");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.Result = shoeCreateDTOValidationResult;
                    return _apiResponse;
                }

                // color 
                if (!Enum.TryParse<Color>(shoeCreateDTO.Color, ignoreCase: true, out var color))
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"color is not valid");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.Result = shoeCreateDTOValidationResult;
                    return _apiResponse;
                }
                
                // category                
                var category = await _categoryRepository.GetAsync(ct => ct.Name == shoeCreateDTO.CTName, tracked:false);
                if (category == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"category does not exist");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                // shoe                
                var shoe = _mapper.Map<Shoe>(shoeCreateDTO);                
                shoe.Gender = gender;
                shoe.Color = color;                
                shoe.ApplicationUserId = user.Id;
                shoe.CategoryId = category.Id;

                await _shoeRepository.CreateAsync(shoe);

                // hashtag
                if (shoeCreateDTO.Hashtags.Count > 0)
                {
                    foreach (var item in shoeCreateDTO.Hashtags)
                    {
                        var hashTag = await _hashtagRepository.GetAsync(x => x.Text == item.Text);
                        if (hashTag is null)
                        {
                            var hashTagCreated = _mapper.Map<Hashtag>(item);
                            ValidationResult hashtagResult = new HashtagValidator().Validate(hashTagCreated);
                            if (!hashtagResult.IsValid)
                            {
                                _apiResponse.IsSuccess = false;
                                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                                foreach (var error in hashtagResult.Errors)
                                {
                                    _apiResponse.ErrorMessages.Add(error.ErrorMessage);
                                }
                                return _apiResponse;
                            }
                            await _hashtagRepository.CreateAsync(hashTagCreated);

                            // shoeHashtag
                            await _shoeHashtagRepository.CreateAsync(new ShoeHashtag
                            {
                                HashtagId = hashTagCreated.Id,
                                ShoeId = shoe.Id
                            });
                        }
                        else
                        {
                            // shoehashtag
                            await _shoeHashtagRepository.CreateAsync(new ShoeHashtag
                            {
                                HashtagId = hashTag.Id,
                                ShoeId = shoe.Id
                            });
                        }
                    }
                }


                var shoeDTO = _mapper.Map<ShoeDTO>(shoe);
                foreach (var item in shoeCreateDTO.Hashtags)
                {
                    shoeDTO.Hashtags.Add(item.Text);
                }

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                _apiResponse.Result = shoeDTO;
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
