using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using FluentValidation.Results;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using static DigitalShoes.Domain.StaticDetails;
using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.ShoeDTOs;
using DigitalShoes.Service.Abstractions;
using DigitalShoes.Domain.FluentValidators;
using DigitalShoes.Domain.Entities;
using DigitalShoes.Dal.Context;
using DigitalShoes.Domain.DTOs.HashtagDtos;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;

namespace DigitalShoes.Service
{
    public class ShoeService : IShoeService
    {
        private readonly ApplicationDbContext _dbContext;
        //
        private readonly IMapper _mapper;
        protected ApiResponse _apiResponse;
        //
        private readonly UserManager<ApplicationUser> _userManager;
        //
        

        public ShoeService(IMapper mapper, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _userManager = userManager;
            _apiResponse = new();        
        }

        public async Task<ApiResponse> CreateAsync(ShoeCreateDTO shoeCreateDTO, HttpContext httpContext)
        {
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();
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

                string username = httpContext
                .User
                .Identities
                .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
                .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
                .Value;

                // checking if user have this product in database
                var user = await _userManager
                    .Users
                    .Include(u => u.Shoes)
                    .FirstOrDefaultAsync(u => u.UserName == username);

                // checking if user have this product in database
                var existingShoe = user.Shoes.Where(sh => sh.Brand == shoeCreateDTO.Brand && sh.Model == shoeCreateDTO.Model && sh.ApplicationUserId == user.Id).FirstOrDefault();
                if (existingShoe != null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"You have {shoeCreateDTO.Model} model of {shoeCreateDTO.Brand} brand in your products, you can increase count instead");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }


                // gender 
                if (!Enum.TryParse<Gender>(shoeCreateDTO.Gender, ignoreCase: true, out var gender))
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"gender is not valid");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                // color 
                if (!Enum.TryParse<Color>(shoeCreateDTO.Color, ignoreCase: true, out var color))
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"color is not valid");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                // category                
                var category = await _dbContext.Categories.Where(ct => ct.Name == shoeCreateDTO.CTName).AsNoTracking().FirstOrDefaultAsync();
                if (category == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"{shoeCreateDTO.CTName} category does not exist");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return _apiResponse;
                }

                // adding shoe                
                var shoe = _mapper.Map<Shoe>(shoeCreateDTO);
                shoe.Gender = gender;
                shoe.Color = color;
                shoe.ApplicationUserId = user.Id;
                shoe.CategoryId = category.Id;

                await _dbContext.Shoes.AddAsync(shoe);
                await _dbContext.SaveChangesAsync();

                // checking if shoe added to database
                var shoeInDb = user.Shoes.Where(sh => sh.Brand == shoeCreateDTO.Brand && sh.Model == shoeCreateDTO.Model).FirstOrDefault();
                if (shoeInDb == null)
                {
                    await _dbContextTransaction.RollbackAsync();
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"{shoeCreateDTO.Model} shoe was not created");
                    _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return _apiResponse;
                }

                // hashtag
                if (shoeCreateDTO.Hashtags.Count > 0)
                {
                    foreach (var item in shoeCreateDTO.Hashtags)
                    {
                        var hashTag = await _dbContext.Hashtags.Where(x => x.Text == item.Text).FirstOrDefaultAsync();
                        if (hashTag is null)
                        {
                            var hashTagCreated = _mapper.Map<Hashtag>(item);
                            ValidationResult hashtagResult = new HashtagValidator().Validate(hashTagCreated);
                            if (!hashtagResult.IsValid)
                            {
                                await _dbContextTransaction.RollbackAsync();
                                _apiResponse.IsSuccess = false;
                                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                                foreach (var error in hashtagResult.Errors)
                                {
                                    _apiResponse.ErrorMessages.Add(error.ErrorMessage);
                                }
                                return _apiResponse;
                            }

                            // adding hashtag
                            await _dbContext.Hashtags.AddAsync(hashTagCreated);
                            await _dbContext.SaveChangesAsync();

                            // checking if hashtag added
                            var hashtagInDb = await _dbContext.Hashtags.Where(x => x.Text == item.Text).FirstOrDefaultAsync();
                            if (hashtagInDb is null)
                            {
                                await _dbContextTransaction.RollbackAsync();
                                _apiResponse.IsSuccess = false;
                                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                                _apiResponse.ErrorMessages.Add($"{item.Text} hashtag could not be created");
                                return _apiResponse;
                            }

                            // adding shoehashtag
                            await _dbContext.ShoeHashtags.AddAsync(new ShoeHashtag
                            {
                                HashtagId = hashTagCreated.Id,
                                ShoeId = shoe.Id
                            });
                            await _dbContext.SaveChangesAsync();

                            // checking if shoehashtag added
                            var shoeHashtagInDb = await _dbContext.ShoeHashtags.Where(x => x.HashtagId == hashTagCreated.Id && x.ShoeId == shoe.Id).FirstOrDefaultAsync();
                            if (shoeHashtagInDb is null)
                            {
                                await _dbContextTransaction.RollbackAsync();
                                _apiResponse.IsSuccess = false;
                                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                                _apiResponse.ErrorMessages.Add($"{item.Text} hashtag for {shoe.Model} shoe could not be created in shoeHashtag");
                                return _apiResponse;
                            }
                        }
                        else
                        {
                            // adding shoehashtag
                            await _dbContext.ShoeHashtags.AddAsync(new ShoeHashtag
                            {
                                HashtagId = hashTag.Id,
                                ShoeId = shoe.Id
                            });
                            await _dbContext.SaveChangesAsync();

                            // checking if shoehashtag added
                            var shoeHashtagInDb = await _dbContext.ShoeHashtags.Where(x => x.HashtagId == hashTag.Id && x.ShoeId == shoe.Id).FirstOrDefaultAsync();
                            if (shoeHashtagInDb is null)
                            {
                                await _dbContextTransaction.RollbackAsync();
                                _apiResponse.IsSuccess = false;
                                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                                _apiResponse.ErrorMessages.Add($"{item.Text} hashtag for {shoe.Model} shoe could not be created in shoeHashtag");
                                return _apiResponse;
                            }
                        }
                    }
                }

                // transaction finished
                _dbContextTransaction.Commit();

                // response
                var shoeGetDTO = _mapper.Map<ShoeGetDTO>(shoe);
                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                _apiResponse.Result = shoeGetDTO;
                return _apiResponse;
            }
            catch (Exception ex)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add(ex.Message);
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.Result = ex;
                return _apiResponse;
            }
        }

        public async Task<ApiResponse> UpdateAsync(int? id, ShoeUpdateDTO shoeUpdateDTO, HttpContext httpContext)
        {
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (id == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"id is null");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.Result = shoeUpdateDTO;
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
                    .Include(u => u.Shoes)
                    .ThenInclude(sh => sh.ShoeHashtags)
                    .ThenInclude(h => h.Hashtag)
                    .FirstOrDefaultAsync(u => u.UserName == username);

                var existingShoe = user.Shoes.Where(sh => sh.Id == id).FirstOrDefault();
                if (existingShoe == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"You don't have any product with {id} id");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return _apiResponse;
                }

                ValidationResult shoeUpdateDTOValidationResult = new ShoeUpdateDTOValidator().Validate(shoeUpdateDTO);
                if (!shoeUpdateDTOValidationResult.IsValid)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    foreach (var error in shoeUpdateDTOValidationResult.Errors)
                    {
                        _apiResponse.ErrorMessages.Add(error.ErrorMessage);
                    }
                    return _apiResponse;
                }

                var ifSameExists = user.Shoes.Where(sh => sh.Brand == shoeUpdateDTO.Brand && sh.Model == shoeUpdateDTO.Model && sh.Id != id).FirstOrDefault();
                if (ifSameExists != null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"You have {ifSameExists.Model} model of {ifSameExists.Brand} brand in your products, you can increase count instead");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                // gender 
                if (!Enum.TryParse<Gender>(shoeUpdateDTO.Gender, ignoreCase: true, out var gender))
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"gender is not valid");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                // color 
                if (!Enum.TryParse<Color>(shoeUpdateDTO.Color, ignoreCase: true, out var color))
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"color is not valid");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                // category                
                var category = await _dbContext.Categories.Where(ct => ct.Name == shoeUpdateDTO.CTName).AsNoTracking().FirstOrDefaultAsync();
                if (category == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"{shoeUpdateDTO.CTName} category does not exist");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return _apiResponse;
                }

                // getting shoe price before update
                var price = existingShoe.Price;

                // shoe                
                _mapper.Map(shoeUpdateDTO, existingShoe);
                existingShoe.Gender = gender;
                existingShoe.Color = color;
                existingShoe.ApplicationUserId = user.Id;
                existingShoe.CategoryId = category.Id;

                _dbContext.Shoes.Update(existingShoe);
                var affectedRows = await _dbContext.SaveChangesAsync();
                if (affectedRows == 0)
                {
                    await _dbContextTransaction.RollbackAsync();
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"operation is not successful");
                    _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return _apiResponse;
                }

                // getting updated shoe
                var updatedShoe = await _dbContext.Shoes.Where(x => x.Id == existingShoe.Id).Include(x => x.CartItems).ThenInclude(x => x.Cart).FirstOrDefaultAsync();
                // checking if shoe price updated
                if (price != updatedShoe.Price)
                {
                    foreach (var item in updatedShoe.CartItems)
                    {
                        // getting cartItem price before update
                        var cartItemPrice = item.Price;
                        // updating cartItem price
                        item.Price = item.ItemsCount * updatedShoe.Price;
                        //_dbContext.CartItems.Update(item); 
                        await _dbContext.SaveChangesAsync();
                        // getting updated cartItem price
                        var updatedCartItemPrice = await _dbContext.CartItems.Where(x => x.Id == item.Id).Select(x => x.Price).FirstOrDefaultAsync();
                        // checking if cartItem price updated
                        if (cartItemPrice == updatedCartItemPrice)
                        {
                            await _dbContextTransaction.RollbackAsync();
                            _apiResponse.IsSuccess = false;
                            _apiResponse.ErrorMessages.Add($"operation is not successful");
                            _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                            return _apiResponse;
                        }

                        // getting cart of cartItem before update
                        var cart = await _dbContext.Carts.Where(x => x.Id == item.CartId).FirstOrDefaultAsync();
                        // getting cart TotalPrice of cartItem before update
                        var cartTotalPrice = cart.TotalPrice;
                        // updating cart TotalPrice of cartItem
                        cart.TotalPrice = await _dbContext.CartItems.Where(x => x.CartId == cart.Id).Select(x => x.Price).SumAsync();
                        //_dbContext.Carts.Update(cart);
                        await _dbContext.SaveChangesAsync();
                        // getting updated cart total price of cartItem
                        var updatedCartTotalPrice = await _dbContext.Carts.Where(x => x.Id == item.CartId).Select(x => x.TotalPrice).FirstOrDefaultAsync();
                        // checking if cart total price of cartItem updated
                        if (cartTotalPrice == updatedCartTotalPrice)
                        {
                            await _dbContextTransaction.RollbackAsync();
                            _apiResponse.IsSuccess = false;
                            _apiResponse.ErrorMessages.Add($"operation is not successful");
                            _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                            return _apiResponse;
                        }
                    }

                }
                // transaction finished
                _dbContextTransaction.Commit();

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                return _apiResponse;
            }
            catch (Exception ex)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add(ex.Message);
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.Result = ex;
                return _apiResponse;
            }
        }

        public async Task<ApiResponse> GetAllAsync(HttpContext httpContext)
        {
            try
            {
                string username = httpContext
                .User
                .Identities
                .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
                .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
                .Value;

                //var user = await _userManager.FindByNameAsync(username);
                var user = await _userManager
                    .Users
                    .Include(u => u.Shoes)
                       .ThenInclude(s => s.Images)
                    .Include(u => u.Shoes)
                       .ThenInclude(s => s.ShoeHashtags)
                           .ThenInclude(sh => sh.Hashtag)// Include the navigation property for Shoes
                    .FirstOrDefaultAsync(u => u.UserName == username);

                var shoeGetDTO = _mapper.Map<List<ShoeGetDTO>>(user.Shoes);


                if (shoeGetDTO.Count == 0)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add("you don't have any products");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.Result = shoeGetDTO;
                return _apiResponse;
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add(ex.Message.ToString());
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Result = ex;
                return _apiResponse;
            }
        }

        public async Task<ApiResponse> GetByIdAsync(int? id, HttpContext httpContext)
        {
            try
            {
                if (id == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"id is null");
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
                    .Include(u => u.Shoes)
                       .ThenInclude(s => s.Images)
                    .Include(u => u.Shoes)
                       .ThenInclude(s => s.ShoeHashtags)
                           .ThenInclude(sh => sh.Hashtag)// Include the navigation property for Shoes
                    .FirstOrDefaultAsync(u => u.UserName == username);


                var existingShoe = user.Shoes.Where(x => x.Id == id).FirstOrDefault();
                if (existingShoe == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"You don't have a product with {id} id");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return _apiResponse;
                }

                var shoeGetDTO = _mapper.Map<ShoeGetDTO>(existingShoe);
                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.Result = shoeGetDTO;
                return _apiResponse;
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add(ex.Message.ToString());
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Result = ex;
                return _apiResponse;
            }
        }

        public async Task<ApiResponse> DeleteProductByIdAsync(int? id, HttpContext httpContext)
        {
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                if (id == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"id is null");
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
                    .Include(u => u.Shoes)
                    .FirstOrDefaultAsync(u => u.UserName == username);


                var existingShoe = user.Shoes.Where(x => x.Id == id).FirstOrDefault();
                if (existingShoe == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"You don't have a product with {id} id");
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return _apiResponse;
                }

                var cartId = await _dbContext.CartItems.Where(x => x.ShoeId == id).Select(x=>x.CartId).ToListAsync();
                var imageLocalPath = await _dbContext.Images.Where(x => x.ShoeId == id).Select(x => x.ImageLocalPath).ToListAsync();
                _dbContext.Shoes.Remove(existingShoe);
                await _dbContext.SaveChangesAsync();


                var ifDeleted = user.Shoes.Where(x => x.Id == id).FirstOrDefault();
                if (ifDeleted != null)
                {
                    await _dbContextTransaction.RollbackAsync();
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add($"shoe with {id} id was not deleted");
                    _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return _apiResponse;
                }

                foreach (var _id in cartId)
                {
                    // getting cart of cartItem
                    var cart = await _dbContext.Carts.Where(x => x.Id == _id).Include(x => x.CartItems).FirstOrDefaultAsync();
                    // updating cart                     
                    var totalPriceBeforeUpdate = cart.TotalPrice; 
                    var countBeforeUpdate = cart.ItemsCount;                    
                    cart.TotalPrice = cart.CartItems.Select(x=>x.Price).Sum();
                    cart.ItemsCount = cart.CartItems.Select(x=>x.ItemsCount).Sum();
                    await _dbContext.SaveChangesAsync();
                    // checking if totalprice of cart updated
                    if (totalPriceBeforeUpdate==cart.TotalPrice || countBeforeUpdate==cart.ItemsCount)        
                    {
                        await _dbContextTransaction.RollbackAsync();
                        _apiResponse.IsSuccess = false;
                        _apiResponse.ErrorMessages.Add($"operation is not successful");
                        _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return _apiResponse;
                    }
                }

                // removing also images of shoe
                foreach (var item in imageLocalPath)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        FileInfo file = new FileInfo(item);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }
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
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add(ex.Message.ToString());
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Result = ex;
                return _apiResponse;
            }
        }


    }
}
