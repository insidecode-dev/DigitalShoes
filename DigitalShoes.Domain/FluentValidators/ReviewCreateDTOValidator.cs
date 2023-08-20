using DigitalShoes.Domain.DTOs.CartItemDTOs;
using DigitalShoes.Domain.DTOs.ReviewDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.FluentValidators
{
    public class ReviewCreateDTOValidator : AbstractValidator<ReviewCreateDTO>
    {
        public ReviewCreateDTOValidator()
        {
            RuleFor(p => p.ShoeId)
                .NotNull().NotEmpty().WithMessage("items count should be added");

            RuleFor(p => p.ReviewText)
                .MaximumLength(300).WithMessage("maximum character size is 300")
                .MinimumLength(3).WithMessage("minimum character size is 3");

            RuleFor(p => p.Rating)
                .MaximumLength(10).WithMessage("maximum character size is 10")
                .MinimumLength(3).WithMessage("minimum character size is 3");
        }
    }
}
