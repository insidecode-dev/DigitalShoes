using DigitalShoes.Dal.Context;
using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.DTOs.CartItemDTOs;
using DigitalShoes.Domain.DTOs.PaymentDTOs;
using DigitalShoes.Domain.Entities;
using DigitalShoes.Domain.FluentValidators;
using DigitalShoes.Service.Abstractions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Net;
using System.Security.Claims;

namespace DigitalShoes.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ApiResponse _apiResponse;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _apiResponse = new();
            _userManager = userManager;
        }

        public async Task<ApiResponse> ApproveCartAsync(HttpContext httpContext)
        {
            // beginning transaction
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

            string username = httpContext
            .User
            .Identities
            .FirstOrDefault(identity => identity.Claims.Any(claim => claim.Type == ClaimTypes.Name))?
            .Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?
            .Value;

            var user = await _userManager
                .Users
                .Include(x => x.Cart)
                .ThenInclude(x => x.CartItems)
                .FirstOrDefaultAsync(u => u.UserName == username);

            var cartItems = user.Cart.CartItems;

            foreach (var item in cartItems)
            {
                // checking if there is still enough shoe for request
                var currentCount = await _dbContext.Shoes.Where(x => x.Id == item.ShoeId).Select(x => x.Count).FirstOrDefaultAsync();
                if (currentCount < item.ItemsCount || currentCount == 0)
                {
                    _apiResponse.ErrorMessages.Add($"there is not enough shoe with {item.ShoeId} id for this request currently");
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return _apiResponse;
                }
            }


            // checking if user have enough balance for requested shoe 
            if (user.Balance < user.Cart.TotalPrice)
            {
                _apiResponse.ErrorMessages.Add($"you don't have enough balance for completing this request");
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return _apiResponse;
            }


            // main operations

            // creating payment session
            var payment = new Payment { ApplicationUserId = user.Id };
            await _dbContext.Payments.AddAsync(payment);
            await _dbContext.SaveChangesAsync();

            // checking if payment session created
            if (await _dbContext.Payments.Where(x => x.Id == payment.Id).FirstOrDefaultAsync() is null)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMessages.Add("transaction is not successful");
                return _apiResponse;
            }

            // creating paymentObjects for session 
            foreach (var item in user.Cart.CartItems)
            {
                var paymentObject = new PaymentObject
                {
                    ItemsCount = item.ItemsCount,
                    ShoeId = item.ShoeId,
                    PaymentId = payment.Id,
                    Price = item.Price
                };
                await _dbContext.PaymentObjects.AddAsync(paymentObject);
                await _dbContext.SaveChangesAsync();


                // checking if paymentObject is created for session                 
                if (await _dbContext.PaymentObjects.Where(x => x.Id == paymentObject.Id).FirstOrDefaultAsync() is null)
                {
                    await _dbContextTransaction.RollbackAsync();
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                    _apiResponse.ErrorMessages.Add("transaction is not successful");
                    return _apiResponse;
                }
            }


            // updating count of requested shoe in Shoe table
            foreach (var item in user.Cart.CartItems)
            {
                var shoeDecreased = await _dbContext.Shoes.Where(x => x.Id == item.ShoeId).FirstOrDefaultAsync();
                int cnt = shoeDecreased.Count - item.ItemsCount;
                shoeDecreased.Count = cnt;
                _dbContext.Shoes.Update(shoeDecreased);
                await _dbContext.SaveChangesAsync();

                // checking if count updated
                if (await _dbContext.Shoes.Where(x => x.Id == item.ShoeId).Select(x => x.Count).FirstOrDefaultAsync() != cnt)
                {
                    await _dbContextTransaction.RollbackAsync();
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                    _apiResponse.ErrorMessages.Add("transaction is not successful");
                    return _apiResponse;
                }
            }


            // updating total price for payment session                
            payment.TotalPrice = user.Cart.TotalPrice;
            _dbContext.Payments.Update(payment);
            await _dbContext.SaveChangesAsync();

            // checking if total price for payment session updated
            if (await _dbContext.Payments.Where(x => x.Id == payment.Id).Select(x => x.TotalPrice).FirstOrDefaultAsync() != user.Cart.TotalPrice)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMessages.Add("transaction is not successful");
                return _apiResponse;
            }



            // decreasing customer's balance
            user.Balance -= user.Cart.TotalPrice;
            decimal currentCustomerBalance = user.Balance;
            await _userManager.UpdateAsync(user);
            await _dbContext.SaveChangesAsync();

            // checking if customer's balance decreased
            if (await _userManager
                .Users
                .Where(u => u.UserName == username)
                .Select(x => x.Balance)
                .FirstOrDefaultAsync() != currentCustomerBalance)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMessages.Add("transaction is not successful");
                return _apiResponse;
            }


            //increasing seller's balance
            foreach (var item in user.Cart.CartItems)
            {
                var seller = await _dbContext
                .Shoes
                .Include(x => x.ApplicationUser)
                .Where(x => x.Id == item.ShoeId)
                .Select(x => x.ApplicationUser)
                .FirstOrDefaultAsync();

                seller.Balance += item.Price;
                var currentSellerBalance = seller.Balance;
                await _userManager.UpdateAsync(user);
                await _dbContext.SaveChangesAsync();

                // checking if seller's balance increased
                if (await _userManager
                    .Users
                    .Where(u => u.UserName == seller.UserName)
                    .Select(x => x.Balance)
                    .FirstOrDefaultAsync() != currentSellerBalance)
                {
                    await _dbContextTransaction.RollbackAsync();
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                    _apiResponse.ErrorMessages.Add("transaction is not successful");
                    return _apiResponse;
                }
            }

            // empting customer's cart
            _dbContext.Carts.Remove(user.Cart);
            await _dbContext.SaveChangesAsync();

            // checking if seller's balance increased
            var customerCart = await _userManager
                .Users
                .Where(x => x.UserName == username)
                .Include(x => x.Cart)
                .Select(x => x.Cart)
                .FirstOrDefaultAsync();
            if (customerCart != null)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMessages.Add("transaction is not successful");
                return _apiResponse;
            }

            // transaction finished
            _dbContextTransaction.Commit();

            // response
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            return _apiResponse;
        }
        /*
         yoxlanilir ki bele bir ayaqqabi varmi (done)
         yoxlanilir ki kifayet qeder ayaqqabi varmi (done)
         yoxlanilir ki musterinin kifayet qeder balansi varmi (done)
         payment yaradilir user id ile ve daha sonra elave olunur (done)
         payment yaradilmasi yoxlanilir (done)
         payment object yaradilir elave olunur (done)
         payment object yaradilmasi yoxlanilir (done) 
         saticinin hemin mehsulunun miqdarindan requestdeki miqdar cixilir (done) 
         miqdar cixilimasi yoxlanilir (done) 
         payment ucun Totalprice update olunur (done)
         payment ucun Totalprice update olunmasi yoxlanilir (done)
         musterinin balansinnan balans cixilir (done)
         balans cixilmasi yoxlanilir (done)
         saticinin balansina musteriden cixilan qeder elave olunur (done)
         balans artimi yoxlanilir (done)
        */
        public async Task<ApiResponse> BuyProductByIdAsync(TransactionDTO transactionDTO, HttpContext httpContext)
        {
            // beginning transaction
            IDbContextTransaction _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

            // validation
            ValidationResult transactionDTOValidationResult = new TransactionDTOValidator().Validate(transactionDTO);
            if (!transactionDTOValidationResult.IsValid)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                foreach (var error in transactionDTOValidationResult.Errors)
                {
                    _apiResponse.ErrorMessages.Add(error.ErrorMessage);
                }
                _apiResponse.Result = transactionDTOValidationResult;
                return _apiResponse;
            }

            // if shoe exists
            var existingShoe = await _dbContext.Shoes
                .Where(x => x.Id == transactionDTO.ShoeId)
                .FirstOrDefaultAsync();
            if (existingShoe == null)
            {
                _apiResponse.ErrorMessages.Add($"there is not shoe with {transactionDTO.ShoeId} id");
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                return _apiResponse;
            }


            // checking if there is enough shoe for request
            if (existingShoe.Count < transactionDTO.ItemsCount || existingShoe.Count == 0)
            {
                _apiResponse.ErrorMessages.Add($"there is not enough shoe with {existingShoe.Id} id for this request");
                _apiResponse.IsSuccess = false;
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
                .FirstOrDefaultAsync(u => u.UserName == username);

            // checking if user have enough balance for requested shoe 
            if (user.Balance < (int)(transactionDTO.ItemsCount * existingShoe.Price))
            {
                _apiResponse.ErrorMessages.Add($"you don't have enough balance for completing this request");
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return _apiResponse;
            }

            // main operations

            // creating payment session
            var payment = new Payment { ApplicationUserId = user.Id };
            await _dbContext.Payments.AddAsync(payment);
            await _dbContext.SaveChangesAsync();

            // checking if payment session created
            if (await _dbContext.Payments.Where(x => x.Id == payment.Id).FirstOrDefaultAsync() is null)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMessages.Add("transaction is not successful");
                return _apiResponse;
            }

            // creating paymentObject for session 
            var paymentObject = new PaymentObject
            {
                ItemsCount = transactionDTO.ItemsCount,
                ShoeId = transactionDTO.ShoeId,
                PaymentId = payment.Id,
                Price = existingShoe.Price * transactionDTO.ItemsCount
            };
            await _dbContext.PaymentObjects.AddAsync(paymentObject);
            await _dbContext.SaveChangesAsync();


            // checking if paymentObject is created for session                 
            if (await _dbContext.PaymentObjects.Where(x => x.Id == paymentObject.Id).FirstOrDefaultAsync() is null)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMessages.Add("transaction is not successful");
                return _apiResponse;
            }



            // updating count of requested shoe in Shoe table
            var shoeDecreased = await _dbContext.Shoes.Where(x => x.Id == transactionDTO.ShoeId).FirstOrDefaultAsync();
            int cnt = existingShoe.Count - transactionDTO.ItemsCount;
            shoeDecreased.Count = cnt;
            _dbContext.Shoes.Update(shoeDecreased);
            await _dbContext.SaveChangesAsync();

            // checking if count updated
            if (await _dbContext.Shoes.Where(x => x.Id == transactionDTO.ShoeId).Select(x => x.Count).FirstOrDefaultAsync() != cnt)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMessages.Add("transaction is not successful");
                return _apiResponse;
            }

            // updating total price for payment session
            var totalPaymentPrice = await _dbContext.Payments.Where(x => x.Id == payment.Id).Include(x => x.PaymentObjects)
                .Select(x => x.PaymentObjects.Sum(x => x.Price)).FirstOrDefaultAsync();
            payment.TotalPrice = totalPaymentPrice;
            _dbContext.Payments.Update(payment);
            await _dbContext.SaveChangesAsync();

            // checking if total price for payment session updated
            if (await _dbContext.Payments.Where(x => x.Id == payment.Id).Select(x => x.TotalPrice).FirstOrDefaultAsync() != totalPaymentPrice)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMessages.Add("transaction is not successful");
                return _apiResponse;
            }



            // decreasing customer's balance
            user.Balance -= totalPaymentPrice;
            decimal currentCustomerBalance = user.Balance;
            await _userManager.UpdateAsync(user);
            await _dbContext.SaveChangesAsync();

            // checking if customer's balance decreased
            if (await _userManager
                .Users
                .Where(u => u.UserName == username)
                .Select(x => x.Balance)
                .FirstOrDefaultAsync() != currentCustomerBalance)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMessages.Add("transaction is not successful");
                return _apiResponse;
            }


            //increasing seller's balance
            var seller = await _dbContext
                .Shoes
                .Include(x => x.ApplicationUser)
                .Where(x => x.Id == transactionDTO.ShoeId)
                .Select(x => x.ApplicationUser)
                .FirstOrDefaultAsync();

            seller.Balance += totalPaymentPrice;
            var currentSellerBalance = seller.Balance;
            await _userManager.UpdateAsync(user);
            await _dbContext.SaveChangesAsync();

            // checking if seller's balance increased
            if (await _userManager
                .Users
                .Where(u => u.UserName == seller.UserName)
                .Select(x => x.Balance)
                .FirstOrDefaultAsync() != currentSellerBalance)
            {
                await _dbContextTransaction.RollbackAsync();
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.ErrorMessages.Add("transaction is not successful");
                return _apiResponse;
            }

            // transaction finished
            _dbContextTransaction.Commit();

            // response
            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            return _apiResponse;
        }
    }
}

