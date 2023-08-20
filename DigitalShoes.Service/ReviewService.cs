using AutoMapper;
using DigitalShoes.Dal.Context;
using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.ReviewDTOs;
using DigitalShoes.Domain.DTOs.ShoeDTOs;
using DigitalShoes.Domain.Entities;
using DigitalShoes.Domain.FluentValidators;
using DigitalShoes.Service.Abstractions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static DigitalShoes.Domain.StaticDetails;

namespace DigitalShoes.Service
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _dbContext;
        //
        private readonly IMapper _mapper;
        protected ApiResponse _apiResponse;
        //
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewService(ApplicationDbContext dbContext, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _apiResponse = new();
            _userManager = userManager;
        }

        public async Task<ApiResponse> AddReviewAsync(ReviewCreateDTO reviewCreateDTO, HttpContext httpContext)
        {
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                ValidationResult reviewCreateDTOValidationResult = new ReviewCreateDTOValidator().Validate(reviewCreateDTO);
                if (!reviewCreateDTOValidationResult.IsValid)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    foreach (var error in reviewCreateDTOValidationResult.Errors)
                    {
                        _apiResponse.ErrorMessages.Add(error.ErrorMessage);
                    }
                    return _apiResponse;
                }

                // rating 
                if (!Enum.TryParse<Rating>(reviewCreateDTO.Rating, ignoreCase: true, out var rating))
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"rating is not valid");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                string username = httpContext
                .User
                .Identities
                .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
                .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
                .Value;

                var user = await _userManager
                    .Users
                    .Include(w => w.Reviews)
                    .FirstOrDefaultAsync(u => u.UserName == username);

                // checking if shoe with ShoeId exists
                var existingShoe = await _dbContext.Shoes.Where(x => x.Id == reviewCreateDTO.ShoeId).FirstOrDefaultAsync();
                if (existingShoe is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.ErrorMessages.Add($"shoe with {reviewCreateDTO.ShoeId} id does not exist");
                    return _apiResponse;
                }

                // creating review
                var reviewToCreate = _mapper.Map<Review>(reviewCreateDTO);
                reviewToCreate.ApplicationUserId = user.Id;
                reviewToCreate.Rating = rating;
                await _dbContext.Reviews.AddAsync(reviewToCreate);
                await _dbContext.SaveChangesAsync();

                // checking if review created
                var ifReviewCreated = await _dbContext.Reviews.Where(x => x.Id == reviewToCreate.Id).FirstOrDefaultAsync();
                if (ifReviewCreated is null)
                {
                    await _dbContextTransaction.RollbackAsync();
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"operation is not successful");
                    _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return _apiResponse;
                }

                // transaction finished
                _dbContextTransaction.Commit();

                // response
                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                _apiResponse.Result = _mapper.Map<ReviewGetDTO>(ifReviewCreated);
                return _apiResponse;
            }
            catch (Exception ex)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.ErrorMessages.Add(ex.Message.ToString());
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
        }

        public async Task<ApiResponse> GetByIdAsync(int? ReviewId, HttpContext httpContext)
        {
            try
            {
                if (ReviewId == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"ReviewId is null");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                string username = httpContext
                .User
                .Identities
                .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
                .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
                .Value;

                var user = await _userManager
                    .Users
                    .Include(u => u.Reviews)
                    .FirstOrDefaultAsync(u => u.UserName == username);

                // checking if user's review with provided ReviewId exists
                var existingReview = user.Reviews.Where(x => x.Id == ReviewId).FirstOrDefault();
                if (existingReview == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.ErrorMessages.Add($"you don't have review with {ReviewId} id");
                    return _apiResponse;
                }

                // response
                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.Result = _mapper.Map<ReviewGetDTO>(existingReview);
                return _apiResponse;
            }
            catch (Exception ex)
            {
                _apiResponse.ErrorMessages.Add(ex.Message.ToString());
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Result = ex;
                return _apiResponse;
            }
        }

        public async Task<ApiResponse> UpdateReviewAsync(ReviewUpdateDTO reviewUpdateDTO, HttpContext httpContext)
        {
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                ValidationResult reviewUpdateDTOValidationResult = new ReviewUpdateDTOValidator().Validate(reviewUpdateDTO);
                if (!reviewUpdateDTOValidationResult.IsValid)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    foreach (var error in reviewUpdateDTOValidationResult.Errors)
                    {
                        _apiResponse.ErrorMessages.Add(error.ErrorMessage);
                    }
                    return _apiResponse;
                }

                // rating 
                if (!Enum.TryParse<Rating>(reviewUpdateDTO.Rating, ignoreCase: true, out var rating))
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"rating is not valid");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                string username = httpContext
                .User
                .Identities
                .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
                .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
                .Value;

                var user = await _userManager
                    .Users
                    .Include(w => w.Reviews)
                    .FirstOrDefaultAsync(u => u.UserName == username);

                // checking if review exists
                var existingReview = user.Reviews.Where(x => x.ShoeId == reviewUpdateDTO.ShoeId && x.Id == reviewUpdateDTO.ReviewId).FirstOrDefault();
                if (existingReview == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.ErrorMessages.Add($"review with {reviewUpdateDTO.ReviewId} id for shoe with {reviewUpdateDTO.ShoeId} id does not exist");
                    return _apiResponse;
                }

                // updating
                _mapper.Map(reviewUpdateDTO, existingReview);
                existingReview.ApplicationUserId = user.Id;
                existingReview.Rating = rating;
                await _dbContext.SaveChangesAsync();

                // transaction finished
                _dbContextTransaction.Commit();

                // response
                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                return _apiResponse;
            }
            catch (Exception ex)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.ErrorMessages.Add(ex.Message.ToString());
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
        }

        public async Task<ApiResponse> RemoveReviewAsync(int? ReviewId, HttpContext httpContext)
        {
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (ReviewId == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"ReviewId is null");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                string username = httpContext
                .User
                .Identities
                .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
                .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
                .Value;

                var user = await _userManager
                    .Users
                    .Include(u => u.Reviews)
                    .FirstOrDefaultAsync(u => u.UserName == username);

                // checking if user's review with provided ReviewId exists
                var existingReview = user.Reviews.Where(x => x.Id == ReviewId).FirstOrDefault();
                if (existingReview == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.ErrorMessages.Add($"you don't have review with {ReviewId} id");
                    return _apiResponse;
                }

                // removing review
                _dbContext.Reviews.Remove(existingReview);
                await _dbContext.SaveChangesAsync();
                if (await _dbContext.Reviews.Where(x => x.Id == ReviewId).FirstOrDefaultAsync() is not null)
                {
                    await _dbContextTransaction.RollbackAsync();
                    _apiResponse.ErrorMessages.Add($"your review {ReviewId} id was not removed");
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return _apiResponse;
                }

                // transaction finished
                _dbContextTransaction.Commit();

                // response
                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                return _apiResponse;
            }
            catch (Exception ex)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.ErrorMessages.Add(ex.Message.ToString());
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
        }

        
    }
}
