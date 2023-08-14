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
               .NotNull().NotEmpty().WithMessage("shoe id cannot be empty");

            RuleFor(x => x.ImageId)
               .NotNull().NotEmpty().WithMessage("image id cannot be empty");
        }
    }
}
