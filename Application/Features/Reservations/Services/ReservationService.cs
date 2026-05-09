using Microsoft.EntityFrameworkCore;
using StayHub.Backend.Application.Features.Reservations.DTOs;
using StayHub.Backend.Application.Features.Reservations.Interfaces;
using StayHub.Backend.Domain.Entities;
using StayHub.Backend.Domain.Enums;
using StayHub.Backend.Infrastructure.Data;

namespace StayHub.Backend.Application.Features.Reservations.Services;

public class ReservationService : IReservationService
{
    private readonly ApplicationDbContext _context;

    public ReservationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReservationResponseDto> CreateReservationAsync(CreateReservationDto dto, string userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Fetch Unit and re-verify availability
            var unit = await _context.Units
                .Include(u => u.UnitReservations)
                .FirstOrDefaultAsync(u => u.Id == dto.UnitId);

            if (unit == null)
                throw new Exception("Unit not found.");

            var hasOverlap = unit.UnitReservations.Any(r =>
                r.CheckInDate < dto.CheckOutDate &&
                r.CheckOutDate > dto.CheckInDate &&
                (r.State == UnitState.Reserved || r.State == UnitState.CheckIn || r.State == UnitState.Maintenance));

            if (hasOverlap)
                throw new Exception("The unit is no longer available for the selected dates.");

            // 2. Simulate Payment Processing
            bool paymentSucceeded = SimulatePayment(dto.PaymentDetails);
            if (!paymentSucceeded)
                throw new Exception("Payment processing failed.");

            // 3. Calculate Financials
            int nights = (dto.CheckOutDate.Date - dto.CheckInDate.Date).Days;
            if (nights <= 0) throw new Exception("Check-out date must be after check-in date.");

            decimal basePrice = unit.PricePerNight * nights;
            decimal serviceFee = basePrice * 0.10m; // 10% fee
            decimal taxes = basePrice * 0.05m;      // 5% tax
            decimal totalPrice = basePrice + serviceFee + taxes;

            // 4. Generate Booking Reference
            string bookingRef = $"STY-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

            // 5. Create Reservation Entity
            var reservation = new UnitReservation
            {
                Id = Guid.NewGuid(),
                UnitId = dto.UnitId,
                GuestId = userId,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                State = UnitState.Reserved,
                BookingReference = bookingRef,
                BasePrice = basePrice,
                ServiceFee = serviceFee,
                Taxes = taxes,
                TotalPrice = totalPrice,
                PaymentStatus = PaymentStatus.Completed,
                PaymentMethod = dto.PaymentMethod,
                CreatedAt = DateTime.UtcNow
            };

            _context.UnitReservations.Add(reservation);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return MapToResponse(reservation);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ReservationResponseDto?> GetReservationByIdAsync(Guid id)
    {
        var reservation = await _context.UnitReservations
            .Include(r => r.Unit)
            .FirstOrDefaultAsync(r => r.Id == id);

        return reservation != null ? MapToResponse(reservation) : null;
    }

    public async Task<List<ReservationResponseDto>> GetUserReservationsAsync(string userId)
    {
        var reservations = await _context.UnitReservations
            .Include(r => r.Unit)
            .Where(r => r.GuestId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return reservations.Select(MapToResponse).ToList();
    }

    public async Task<ReservationResponseDto?> UpdateReservationAsync(Guid id, UpdateReservationDto dto)
    {
        var reservation = await _context.UnitReservations
            .Include(r => r.Unit)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null) return null;

        if (dto.CheckInDate.HasValue) reservation.CheckInDate = dto.CheckInDate.Value;
        if (dto.CheckOutDate.HasValue) reservation.CheckOutDate = dto.CheckOutDate.Value;
        
        // Recalculate financials if dates changed
        if (dto.CheckInDate.HasValue || dto.CheckOutDate.HasValue)
        {
            int nights = (reservation.CheckOutDate.Date - reservation.CheckInDate.Date).Days;
            reservation.BasePrice = reservation.Unit.PricePerNight * nights;
            reservation.ServiceFee = reservation.BasePrice * 0.10m;
            reservation.Taxes = reservation.BasePrice * 0.05m;
            reservation.TotalPrice = reservation.BasePrice + reservation.ServiceFee + reservation.Taxes;
        }

        await _context.SaveChangesAsync();
        return MapToResponse(reservation);
    }

    public async Task<bool> CancelReservationAsync(Guid id)
    {
        var reservation = await _context.UnitReservations.FindAsync(id);
        if (reservation == null) return false;

        // Soft delete or status change
        reservation.State = UnitState.Available; // Or a new state 'Cancelled'
        // In this implementation, we'll just set the state to Available to free it up
        // but typically you'd have a 'Cancelled' state.
        
        await _context.SaveChangesAsync();
        return true;
    }

    private bool SimulatePayment(string details)
    {
        // Simple simulation: always succeeds unless details contain "FAIL"
        return !details.Contains("FAIL", StringComparison.OrdinalIgnoreCase);
    }

    private ReservationResponseDto MapToResponse(UnitReservation r)
    {
        return new ReservationResponseDto
        {
            Id = r.Id,
            BookingReference = r.BookingReference,
            UnitNumber = r.Unit?.UnitNumber ?? "N/A",
            UnitTitle = r.Unit?.Title ?? "N/A",
            CheckInDate = r.CheckInDate,
            CheckOutDate = r.CheckOutDate,
            TotalNights = (r.CheckOutDate.Date - r.CheckInDate.Date).Days,
            State = r.State,
            BasePrice = r.BasePrice,
            ServiceFee = r.ServiceFee,
            Taxes = r.Taxes,
            TotalPrice = r.TotalPrice,
            PaymentStatus = r.PaymentStatus,
            PaymentMethod = r.PaymentMethod,
            CreatedAt = r.CreatedAt
        };
    }
}
