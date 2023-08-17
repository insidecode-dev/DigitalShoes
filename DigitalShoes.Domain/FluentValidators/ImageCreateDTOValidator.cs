using DigitalShoes.Domain.DTOs.ImageDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.FluentValidators
{
    public class ImageCreateDTOValidator:AbstractValidator<ImageCreateDTO>
    {
        public ImageCreateDTOValidator()
        {
            RuleFor(p => p.ShoeId)
               .NotNull().NotEmpty().WithMessage("shoe id cannot be empty")
               .Must(shoeId => shoeId.ToString().All(char.IsDigit)).WithMessage("Shoe ID can only contain numeric characters");

            RuleFor(x => x.Image)
            .Must(list => list != null && list.Count > 0)
            .WithMessage("The list must contain at least one item")
            .Must(img => img.All(x => x.ContentType.Contains("image")))
            .WithMessage("you must upload just image");
        }
    }
}
