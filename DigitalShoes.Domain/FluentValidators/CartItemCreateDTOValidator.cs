﻿using DigitalShoes.Domain.DTOs.CartItemDTOs;
using DigitalShoes.Domain.DTOs.CategoryDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.FluentValidators
{
    public class CartItemCreateDTOValidator : AbstractValidator<CartItemCreateDTO>
    {
        public CartItemCreateDTOValidator()
        {
            RuleFor(p => p.ItemsCount)
                .NotNull().NotEmpty().WithMessage("items count should be added")
                .Must(x => x.ToString().All(char.IsDigit)).WithMessage("items count can only contain numeric characters");

            RuleFor(p => p.ShoeId)
                .NotNull().NotEmpty().WithMessage("shoeid should be added")
                .Must(x => x.ToString().All(char.IsDigit)).WithMessage("shoeid can only contain numeric characters");
        }
    }
}
