using FluentValidation;
using StayHub.Backend.Application.Features.Auth.DTOs;

namespace StayHub.Backend.Application.Features.Auth.Validators;

public class OnboardingProfileValidator : AbstractValidator<OnboardingProfileRequest>
{
    public OnboardingProfileValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+[1-9]\d{1,14}$").WithMessage("Phone number must include country code (e.g., +20123456789).");
        RuleFor(x => x.Gender).IsInEnum();
    }
}
