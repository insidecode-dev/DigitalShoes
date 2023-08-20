using AutoMapper;
using DigitalShoes.Dal.Context;
using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.CartItemDTOs;
using DigitalShoes.Domain.DTOs.WishlistDTOs;
using DigitalShoes.Domain.Entities;
using DigitalShoes.Domain.FluentValidators;
using DigitalShoes.Service.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Net;
using System.Security.Claims;

namespace DigitalShoes.Service
{
    public class WishListService : IWishListService
    {
        private readonly ApplicationDbContext _dbContext;
        //
        private readonly IMapper _mapper;
        protected ApiResponse _apiResponse;
        //
        private readonly UserManager<ApplicationUser> _userManager;

        public WishListService(ApplicationDbContext dbContext, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _apiResponse = new();
            _userManager = userManager;
        }

        public async Task<ApiResponse> AddToWishlistAsync(int? ShoeId, HttpContext httpContext)
        {
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (ShoeId == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"ShoeId is null");
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
                    .Include(w => w.Wishlist)
                    .ThenInclude(ws => ws.DesiredShoes)
                    .FirstOrDefaultAsync(u => u.UserName == username);

                // checking if user has wishlist
                if (user.Wishlist == null)
                {
                    var wishlist = new Wishlist { ApplicationUserId = user.Id };
                    await _dbContext.Wishlists.AddAsync(wishlist);
                    await _dbContext.SaveChangesAsync();

                    //checking if wishlist created
                    if (await _dbContext.Wishlists.Where(x => x.Id == wishlist.Id).FirstOrDefaultAsync() == null)
                    {
                        await _dbContextTransaction.RollbackAsync();
                        _apiResponse.IsSuccess = false;
                        _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                        _apiResponse.ErrorMessages.Add("operation is not successful");
                        return _apiResponse;
                    }
                }

                // checking if shoe with provided id exists
                var existingShoe = await _dbContext.Shoes.Where(x => x.Id == ShoeId).FirstOrDefaultAsync();
                if (existingShoe is null)
                {
                    await _dbContextTransaction.RollbackAsync();
                    _apiResponse.ErrorMessages.Add($"shoe with {ShoeId} id does not exist");
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return _apiResponse;
                }

                // checking if shoe has been added to shoewishlist table before
                var wishListId = await _dbContext.Wishlists.Where(x => x.ApplicationUserId == user.Id).Select(x => x.Id).FirstOrDefaultAsync();
                var existingShoeWishlist = await _dbContext.ShoeWishlists.Where(x => x.WishlistId == wishListId && x.ShoeId == existingShoe.Id).FirstOrDefaultAsync();
                if (existingShoeWishlist is null)
                {
                    var newShoeWishlist = new ShoeWishlist { WishlistId = wishListId, ShoeId = existingShoe.Id };
                    await _dbContext.ShoeWishlists.AddAsync(newShoeWishlist);
                    await _dbContext.SaveChangesAsync();

                    // checking if shoewishlist item created
                    if (await _dbContext.ShoeWishlists.Where(x => x.WishlistId == wishListId && x.ShoeId == existingShoe.Id).FirstOrDefaultAsync() is null)
                    {
                        await _dbContextTransaction.RollbackAsync();
                        _apiResponse.IsSuccess = false;
                        _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                        _apiResponse.ErrorMessages.Add("operation is not successful");
                        return _apiResponse;
                    }
                }
                else
                {
                    await _dbContextTransaction.RollbackAsync();
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.ErrorMessages.Add($"shoe with {ShoeId} id exists in your wishlist");
                    return _apiResponse;
                }

                // transaction finished
                _dbContextTransaction.Commit();

                //response
                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.Result = new ShoeWishlistResponseDTO { ShoeId = ShoeId, WishlistId = wishListId };
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

        public async Task<ApiResponse> RemoveFromWishlistAsync(int? ShoeId, HttpContext httpContext)
        {
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (ShoeId == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"ShoeId is null");
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
                    .Include(w => w.Wishlist)
                    .ThenInclude(ws => ws.DesiredShoes)
                    .FirstOrDefaultAsync(u => u.UserName == username);

                // checking if user has wishlist
                if (user.Wishlist == null)
                {
                    await _dbContextTransaction.RollbackAsync();
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.ErrorMessages.Add("you don't have any shoe in your wishlist");
                    return _apiResponse;
                }

                var existingShoeWishlist = await _dbContext.ShoeWishlists.Where(x => x.ShoeId == ShoeId && x.WishlistId == user.Wishlist.Id).FirstOrDefaultAsync();
                if (existingShoeWishlist is null)
                {
                    await _dbContextTransaction.RollbackAsync();
                    _apiResponse.ErrorMessages.Add($"shoe with {ShoeId} id does not exist in your wishlist");
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return _apiResponse;
                }
                else
                {
                    // removing shoewishlist item
                    _dbContext.ShoeWishlists.Remove(existingShoeWishlist);
                    await _dbContext.SaveChangesAsync();

                    // checking if shoewishlist item removed
                    if (await _dbContext.ShoeWishlists.Where(x => x.ShoeId == ShoeId && x.WishlistId == user.Wishlist.Id).FirstOrDefaultAsync() is not null)
                    {
                        await _dbContextTransaction.RollbackAsync();
                        _apiResponse.IsSuccess = false;
                        _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                        _apiResponse.ErrorMessages.Add("operation is not successful");
                        return _apiResponse;
                    }
                }

                // transaction finished
                _dbContextTransaction.Commit();

                //response
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
