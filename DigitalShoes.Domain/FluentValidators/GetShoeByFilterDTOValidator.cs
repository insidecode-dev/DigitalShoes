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
                .MaximumLength(15).WithMessage("maximum character size is 15")
                .MinimumLength(2).WithMessage("minimum character size is 2");

            RuleFor(p => p.Model)                
                .MaximumLength(20).WithMessage("maximum character size is 20")
                .MinimumLength(2).WithMessage("minimum character size is 2");

            RuleFor(p => p.Count)                
                .Must(x => x.ToString().All(char.IsDigit)).WithMessage("count can only contain numeric characters");

            RuleFor(p => p.Size)                
                .Must(x => x.ToString().All(char.IsDigit)).WithMessage("size can only contain numeric characters");

            RuleFor(p => p.Price)                
                .Must(x => x.ToString().All(char.IsDigit)).WithMessage("price can only contain numeric characters");               


            RuleFor(p => p.Gender)                
                .MaximumLength(10).WithMessage("maximum character size is 10")
                .MinimumLength(2).WithMessage("minimum character size is 2");

            RuleFor(p => p.Color)                
                .MaximumLength(10).WithMessage("maximum character size is 10")
                .MinimumLength(2).WithMessage("minimum character size is 2");

            RuleFor(p => p.CTName)                
                .MaximumLength(25).WithMessage("maximum character size is 25")
                .MinimumLength(2).WithMessage("minimum character size is 2");
        }
    }

}
