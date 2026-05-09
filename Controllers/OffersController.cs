using Microsoft.AspNetCore.Mvc;
using StayHub.Backend.Application.Features.Offers.DTOs;
using StayHub.Backend.Application.Features.Offers.Interfaces;

namespace StayHub.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OffersController : ControllerBase
{
    private readonly IOfferService _offerService;

    public OffersController(IOfferService offerService)
    {
        _offerService = offerService;
    }

    [HttpPost("search")]
    public async Task<ActionResult<List<OfferResponseDto>>> Search([FromBody] SearchRequestDto request)
    {
        // 1. Validation: MoveInDate must be before CheckOutDate
        if (request.MoveInDate >= request.CheckOutDate)
        {
            return BadRequest("Move-in date must be strictly before check-out date.");
        }

        // 2. Call Application Service
        var offers = await _offerService.SearchAvailableOffersAsync(request);

        // 3. Return JSON list
        return Ok(offers);
    }
}
