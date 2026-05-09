using Microsoft.EntityFrameworkCore;
using StayHub.Backend.Application.Features.Offers.DTOs;
using StayHub.Backend.Application.Features.Offers.Interfaces;
using StayHub.Backend.Domain.Enums;
using StayHub.Backend.Infrastructure.Data;

namespace StayHub.Backend.Application.Features.Offers.Services;

public class OfferService : IOfferService
{
    private readonly ApplicationDbContext _context;

    public OfferService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<OfferResponseDto>> SearchAvailableOffersAsync(SearchRequestDto request)
    {
        var query = _context.Units
            .Include(u => u.UnitReservations)
            .AsQueryable();

        // 1. Basic Criteria Filtering
        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            query = query.Where(u => u.Address.Contains(request.Location));
        }

        if (!string.IsNullOrWhiteSpace(request.PropertyType))
        {
            // Assuming PropertyType in request matches UnitType enum names
            query = query.Where(u => u.UnitType.ToString() == request.PropertyType);
        }

        query = query.Where(u => u.Capacity >= request.GuestNumber);
        query = query.Where(u => u.PricePerNight <= request.MaxBudget);

        // 2. Overbooking Prevention Logic
        // Exclude units that have an overlapping reservation
        query = query.Where(u => !u.UnitReservations.Any(r =>
            r.CheckInDate < request.CheckOutDate &&
            r.CheckOutDate > request.MoveInDate &&
            (r.State == UnitState.Reserved || r.State == UnitState.CheckIn || r.State == UnitState.Maintenance)
        ));

        // 3. Mapping to DTO
        var results = await query
            .Select(u => new OfferResponseDto
            {
                UnitNumber = u.UnitNumber,
                Title = u.Title,
                HostName = u.HostName ?? "Unknown Host",
                Rating = u.Rating,
                ReviewsCount = u.ReviewsCount,
                PricePerNight = u.PricePerNight,
                ImageUrl = u.ImageUrl ?? string.Empty,
                Amenities = u.Amenities
            })
            .ToListAsync();

        return results;
    }
}
