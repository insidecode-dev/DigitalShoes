using DigitalShoes.Domain.DTOs.CategoryDTOs;
using DigitalShoes.Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.FluentValidators
{
    public class CategoryCreateDTOValidator : AbstractValidator<CategoryCreateDTO>
    {
        public CategoryCreateDTOValidator()
        {
            RuleFor(p => p.Name)
                .NotNull().NotEmpty().WithMessage("category cannot be empty")
                .MaximumLength(25).WithMessage("maximum character size is 25")
                .MinimumLength(2).WithMessage("minimum character size is 2");
        }
    }
}
