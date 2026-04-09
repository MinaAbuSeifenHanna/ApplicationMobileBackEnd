using FluentValidation;
using StayHub.Backend.Application.Features.Auth.DTOs;
using StayHub.Backend.Domain.Enums;

namespace StayHub.Backend.Application.Features.Auth.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[\W]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match.");

        // Role-based validation
        RuleFor(x => x.FaceIdImage)
            .NotEmpty().When(x => x.RoleType == UserRoleType.Host || x.RoleType == UserRoleType.Client)
            .WithMessage("Face ID Image is required.");

        RuleFor(x => x.BackIdImage)
            .NotEmpty().When(x => x.RoleType == UserRoleType.Host || x.RoleType == UserRoleType.Client)
            .WithMessage("Back ID Image is required.");

        RuleFor(x => x.BankIban)
            .NotEmpty().When(x => x.RoleType == UserRoleType.Host)
            .WithMessage("Bank IBAN is required for Hosts.");

        RuleFor(x => x.ContractImage)
            .NotEmpty().When(x => x.RoleType == UserRoleType.Client)
            .WithMessage("Contract Image is required for Clients.");
    }
}
