using DigitalShoes.Domain.DTOs.CartItemDTOs;
using DigitalShoes.Domain.DTOs.UserProfileDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DigitalShoes.Domain.FluentValidators
{
    public class UpdatePasswordDTOValidator : AbstractValidator<UpdatePasswordDTO>
    {
        public UpdatePasswordDTOValidator()
        {
            RuleFor(p => p.NewPassword)
            .NotEmpty().WithMessage("New password can't be empty.")
            .Length(8, 25).WithMessage("Password character size should be between 8 and 25.")
            .Must(password =>
                password.Any(char.IsUpper) &&
                password.Any(char.IsLower) &&
                password.Any(char.IsDigit) &&
                Regex.IsMatch(password, @"[^a-zA-Z0-9\s]")
            ).WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");

            RuleFor(p => p.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password can't be empty.")
                .Equal(p => p.NewPassword).WithMessage("Passwords do not match.");
        }
    }
}
