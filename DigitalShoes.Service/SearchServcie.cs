﻿using AutoMapper;
using DigitalShoes.Dal.Context;
using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.HashtagDtos;
using DigitalShoes.Domain.DTOs.ReviewDTOs;
using DigitalShoes.Domain.DTOs.SearchDTOs;
using DigitalShoes.Domain.DTOs.ShoeDTOs;
using DigitalShoes.Domain.Entities;
using DigitalShoes.Domain.FluentValidators;
using DigitalShoes.Service.Abstractions;
using FluentValidation.Results;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Security.Claims;
using static DigitalShoes.Domain.StaticDetails;

namespace DigitalShoes.Service
{
    public class SearchService : ISearchService
    {
        private readonly ApplicationDbContext _dbContext;
        //
        private readonly IMapper _mapper;
        protected ApiResponse _apiResponse;
        //
        private readonly UserManager<ApplicationUser> _userManager;

        public SearchService(ApplicationDbContext dbContext, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _apiResponse = new();
            _userManager = userManager;
        }

        public async Task<ApiResponse> SearchByHashtagAsync(SearchByHashtagRequestDTO searchByHashtagRequestDTO, HttpContext httpContext)
        {
            if (searchByHashtagRequestDTO.Text is null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add("hashtag is null");
                _apiResponse.IsSuccess = false;
                return _apiResponse;
            }

            var pageSize = searchByHashtagRequestDTO.PageSize;
            var pageNumber = searchByHashtagRequestDTO.PageNumber;

            var existingHashtag = await _dbContext.Hashtags
                .Where(x => x.Text == searchByHashtagRequestDTO.Text)
                .Include(x => x.ShoeHashtags)
                .ThenInclude(x => x.Shoe)
                .ThenInclude(x => x.Images)
                .FirstOrDefaultAsync();

            if (existingHashtag is null)
            {
                _apiResponse.ErrorMessages.Add($"{searchByHashtagRequestDTO.Text} hashtag does not exist");
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                return _apiResponse;
            }

            var hashTag = _mapper.Map<HashtagGetDTO>(existingHashtag);
            var shoeGetDTO = _mapper.Map<List<ShoeGetDTO>>(existingHashtag.ShoeHashtags.Select(x => x.Shoe));


            //pagination                
            if (pageSize > 0)
            {
                if (pageSize > 100)
                {
                    pageSize = 100;
                }

                hashTag.ShoeGetDTO = shoeGetDTO.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            }

            //adding pahgination information to header
            PaginationForResponseHeader pgResponseHeader = new() { PageNumber = pageNumber, PageSize = pageSize };
            //Response is property of ControllerBase class, its type is HttpResponse, that manipulates HttpResponse for executing action
            httpContext.Response.Headers.Add("Pagination", JsonConvert.SerializeObject(pgResponseHeader));

            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Result = hashTag;
            return _apiResponse;
        }

        public async Task<ApiResponse> GetAllWithPaginationAsync(GetAllWithPaginationRequestDTO getAllWithPaginationRequestDTO, HttpContext httpContext)
        {
            var pageSize = getAllWithPaginationRequestDTO.PageSize;
            var pageNumber = getAllWithPaginationRequestDTO.PageNumber;

            var shoes = await _dbContext.Shoes.Include(x => x.Images).Include(x => x.ShoeHashtags).ThenInclude(x => x.Hashtag).ToListAsync();

            //pagination                
            if (pageSize > 0)
            {
                if (pageSize > 100)
                {
                    pageSize = 100;
                }

                shoes = shoes.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            }
            var shoeGetDTO = _mapper.Map<List<ShoeGetDTO>>(shoes);

            //adding pahgination information to header
            PaginationForResponseHeader pgResponseHeader = new() { PageNumber = pageNumber, PageSize = pageSize };
            httpContext.Response.Headers.Add("Pagination", JsonConvert.SerializeObject(pgResponseHeader));

            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Result = shoeGetDTO;
            return _apiResponse;
        }

        public async Task<ApiResponse> SearchShoeByFilterAsync(GetShoeByFilterDTO getShoeByFilterDTO)
        {
            ValidationResult getShoeByFilterDTOValidationResult = new GetShoeByFilterDTOValidator().Validate(getShoeByFilterDTO);
            if (!getShoeByFilterDTOValidationResult.IsValid)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                foreach (var error in getShoeByFilterDTOValidationResult.Errors)
                {
                    _apiResponse.ErrorMessages.Add(error.ErrorMessage);
                }
                _apiResponse.Result = getShoeByFilterDTOValidationResult;
                return _apiResponse;
            }


            // gender 
            

            // color 
            

            // rating                 
            

            // category                
            var ctgry = await _dbContext.Categories.Where(ct => ct.Name == getShoeByFilterDTO.CTName).AsNoTracking().FirstOrDefaultAsync();

            //
            Gender gender;
            Color color;
            Rating rating;
            var shoes = await _dbContext
                .Shoes
                .Where(s =>
                s.Brand.Contains(getShoeByFilterDTO.Brand) ||
                s.Model.Contains(getShoeByFilterDTO.Model) ||
                (getShoeByFilterDTO.Size!=null && s.Size == getShoeByFilterDTO.Size) ||
                (Enum.TryParse<Rating>(getShoeByFilterDTO.Rating, true, out rating) && s.Rating == rating) ||
                (getShoeByFilterDTO.Price!=null && s.Price == getShoeByFilterDTO.Price) ||
                (Enum.TryParse<Gender>(getShoeByFilterDTO.Gender,  true, out gender) && s.Gender == gender) ||
                (Enum.TryParse<Color>(getShoeByFilterDTO.Color, true, out color) && s.Color == color) ||
                (ctgry != null && s.CategoryId == ctgry.Id))
                .Include(x => x.Images)
                .Include(x => x.ShoeHashtags)
                .ThenInclude(x => x.Hashtag)
                .ToListAsync();

            // shoe                
            shoes = shoes.Take(getShoeByFilterDTO.Count).ToList();
            var shoeGetDTO = _mapper.Map<List<ShoeGetDTO>>(shoes);
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Result = shoeGetDTO;
            return _apiResponse;
        }

        public async Task<ApiResponse> GetReviewsByShoeIdAsync(int? ShoeId)
        {
            if (ShoeId == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add($"ShoeId is null");
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return _apiResponse;
            }

            // checking if shoe with provided id exist
            var existingShoe = await _dbContext.Shoes.Where(x => x.Id == ShoeId).FirstOrDefaultAsync();
            if (existingShoe == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.ErrorMessages.Add($"shoe with {ShoeId} id does not exist");
                return _apiResponse;
            }

            // getting reveiws
            var reviews = await _dbContext.Reviews.Where(x => x.ShoeId == ShoeId).ToListAsync();
            if (reviews.Count == 0)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.ErrorMessages.Add($"there is not any reviews for shoe with {ShoeId} id");
                return _apiResponse;
            }

            // response
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Result = _mapper.Map<List<ReviewGetDTO>>(reviews);
            return _apiResponse;
        }
    }
}
