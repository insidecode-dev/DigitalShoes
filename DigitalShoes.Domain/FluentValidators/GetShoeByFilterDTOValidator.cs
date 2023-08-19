using DigitalShoes.Domain.DTOs.SearchDTOs;
using DigitalShoes.Domain.DTOs.ShoeDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.FluentValidators
{
    public class GetShoeByFilterDTOValidator : AbstractValidator<GetShoeByFilterDTO>
    {
        public GetShoeByFilterDTOValidator()
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
                .Must(x => x.ToString().All(char.IsDigit)).WithMessage("count can only contain numeric characters");

            RuleFor(p => p.Size)
                .NotNull().NotEmpty().WithMessage("size should be added")
                .Must(x => x.ToString().All(char.IsDigit)).WithMessage("size can only contain numeric characters");

            RuleFor(p => p.Price)
                .NotNull().NotEmpty().WithMessage("price should be added")
                .Must(x => x.ToString().All(char.IsDigit)).WithMessage("price can only contain numeric characters");
                


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
