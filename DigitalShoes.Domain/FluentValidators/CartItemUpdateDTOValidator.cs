using DigitalShoes.Domain.DTOs.CartItemDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.FluentValidators
{
    public class CartItemUpdateDTOValidator : AbstractValidator<CartItemUpdateDTO>
    {
        public CartItemUpdateDTOValidator()
        {
            RuleFor(p => p.ItemsCount)
                .NotNull().NotEmpty().WithMessage("items count should be added")
                .Must(x => x.ToString().All(char.IsDigit)).WithMessage("items count can only contain numeric characters");
        }
    }
}
