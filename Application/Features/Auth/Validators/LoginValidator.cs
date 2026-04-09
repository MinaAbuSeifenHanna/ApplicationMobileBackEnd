using FluentValidation;
using StayHub.Backend.Application.Features.Auth.DTOs;

namespace StayHub.Backend.Application.Features.Auth.Validators;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
