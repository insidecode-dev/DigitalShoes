using DigitalShoes.Domain.DTOs.ImageDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.FluentValidators
{
    public class ImageDeleteDTOValidator : AbstractValidator<ImageDeleteDTO>
    {
        public ImageDeleteDTOValidator()
        {
            RuleFor(p => p.ShoeId)
               .NotNull().NotEmpty().WithMessage("shoe id cannot be empty")
               .Must(shoeId => shoeId.ToString().All(char.IsDigit)).WithMessage("Shoe ID can only contain numeric characters");

            RuleFor(x => x.ImageId)
               .NotNull().NotEmpty().WithMessage("image id cannot be empty")
               .Must(x => x.ToString().All(char.IsDigit)).WithMessage("Image ID can only contain numeric characters");
        }
    }
}
