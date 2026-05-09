using FluentValidation;
using StayHub.Backend.Application.Features.Units.DTOs;

namespace StayHub.Backend.Application.Features.Units.Validators;

public class UpdateUnitDtoValidator : AbstractValidator<UpdateUnitDto>
{
    public UpdateUnitDtoValidator()
    {
        RuleFor(x => x.Title).MaximumLength(120).When(x => x.Title != null);
        RuleFor(x => x.Description).MaximumLength(2000).When(x => x.Description != null);
        RuleFor(x => x.PricePerNight).GreaterThan(0).When(x => x.PricePerNight.HasValue);
        RuleFor(x => x.Address).MaximumLength(300).When(x => x.Address != null);
        RuleFor(x => x.Capacity).InclusiveBetween(1, 50).When(x => x.Capacity.HasValue);
        RuleFor(x => x.Bedrooms).GreaterThanOrEqualTo(0).When(x => x.Bedrooms.HasValue);
        RuleFor(x => x.Bathrooms).GreaterThanOrEqualTo(0).When(x => x.Bathrooms.HasValue);
    }
}
