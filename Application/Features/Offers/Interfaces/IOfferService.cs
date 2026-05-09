using StayHub.Backend.Application.Features.Offers.DTOs;

namespace StayHub.Backend.Application.Features.Offers.Interfaces;

public interface IOfferService
{
    Task<List<OfferResponseDto>> SearchAvailableOffersAsync(SearchRequestDto request);
}
