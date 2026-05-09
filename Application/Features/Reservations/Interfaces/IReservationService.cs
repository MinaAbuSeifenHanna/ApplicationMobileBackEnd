using StayHub.Backend.Application.Features.Reservations.DTOs;

namespace StayHub.Backend.Application.Features.Reservations.Interfaces;

public interface IReservationService
{
    Task<ReservationResponseDto> CreateReservationAsync(CreateReservationDto dto, string userId);
    Task<ReservationResponseDto?> GetReservationByIdAsync(Guid id);
    Task<List<ReservationResponseDto>> GetUserReservationsAsync(string userId);
    Task<ReservationResponseDto?> UpdateReservationAsync(Guid id, UpdateReservationDto dto);
    Task<bool> CancelReservationAsync(Guid id);
}
