using DigitalShoes.Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.FluentValidators
{
    public class HashtagValidator : AbstractValidator<Hashtag>
    {
        public HashtagValidator()
        {
            RuleFor(p => p.Text)
                .MaximumLength(25).WithMessage("maximum character size is 25")
                .MinimumLength(3).WithMessage("minimum character size is 3");
        }
    }
}
