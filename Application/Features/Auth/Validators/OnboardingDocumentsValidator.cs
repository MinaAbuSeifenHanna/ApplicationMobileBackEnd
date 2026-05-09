using FluentValidation;
using StayHub.Backend.Application.Features.Auth.DTOs;

namespace StayHub.Backend.Application.Features.Auth.Validators;

public class OnboardingDocumentsValidator : AbstractValidator<OnboardingDocumentsRequest>
{
    public OnboardingDocumentsValidator()
    {
        RuleFor(x => x.Nationality).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BankAccountIban)
            .NotEmpty()
            .Matches(@"^[A-Z]{2}[0-9]{2}[A-Z0-9]{4,30}$").WithMessage("Invalid IBAN format.");
    }
}
