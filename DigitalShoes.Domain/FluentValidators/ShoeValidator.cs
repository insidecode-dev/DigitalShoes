using DigitalShoes.Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace DigitalShoes.Domain.FluentValidators
{
    public class ShoeValidator : AbstractValidator<Shoe>
    {
        public ShoeValidator()
        {
            RuleFor(p=>p.Brand).NotNull().NotEmpty();
        }
    }
}
