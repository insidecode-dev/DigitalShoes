using DigitalShoes.Domain.DTOs.ShoeDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.FluentValidators
{
    public class ShoeUpdateDTOValidator : AbstractValidator<ShoeUpdateDTO>
    {
        public ShoeUpdateDTOValidator()
        {
            RuleFor(p => p.Brand)
                .NotNull().NotEmpty().WithMessage("brand cannot be empty")
                .MaximumLength(15).WithMessage("maximum character size is 15")
                .MinimumLength(2).WithMessage("minimum character size is 2");

            RuleFor(p => p.Model)
                .NotNull().NotEmpty().WithMessage("model cannot be empty")
                .MaximumLength(20).WithMessage("maximum character size is 20")
                .MinimumLength(2).WithMessage("minimum character size is 2");

            RuleFor(p => p.Count)
                .NotNull().NotEmpty().WithMessage("count should be added");

            RuleFor(p => p.Size)
                .NotNull().NotEmpty().WithMessage("size should be added");

            RuleFor(p => p.Description)
                .NotNull().NotEmpty().WithMessage("description should not be empty")
                .MaximumLength(1000).WithMessage("maximum character size is 1000")
                .MinimumLength(2).WithMessage("minimum character size is 2");

            RuleFor(p => p.Gender)
                .NotNull().NotEmpty().WithMessage("gender should not be empty")
                .MaximumLength(10).WithMessage("maximum character size is 10")
                .MinimumLength(2).WithMessage("minimum character size is 2");

            RuleFor(p => p.Color)
                .NotNull().NotEmpty().WithMessage("color should not be empty")
                .MaximumLength(10).WithMessage("maximum character size is 10")
                .MinimumLength(2).WithMessage("minimum character size is 2");

            RuleFor(p => p.CTName)
                .NotNull().NotEmpty().WithMessage("category should not be empty")
                .MaximumLength(25).WithMessage("maximum character size is 25")
                .MinimumLength(2).WithMessage("minimum character size is 2");
        }
    }
}
