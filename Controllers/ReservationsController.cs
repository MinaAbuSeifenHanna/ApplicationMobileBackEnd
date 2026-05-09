using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayHub.Backend.Application.Features.Reservations.DTOs;
using StayHub.Backend.Application.Features.Reservations.Interfaces;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace StayHub.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost]
    public async Task<ActionResult<ReservationResponseDto>> CreateReservation([FromBody] CreateReservationDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var reservation = await _reservationService.CreateReservationAsync(dto, userId);
            return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id }, reservation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReservationResponseDto>> GetReservationById(Guid id)
    {
        var reservation = await _reservationService.GetReservationByIdAsync(id);
        if (reservation == null) return NotFound();
        return Ok(reservation);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<ReservationResponseDto>>> GetUserReservations(string userId)
    {
        // For security, ensure the requested userId matches the current authenticated user (unless Admin)
        var currentUserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId != userId && !User.IsInRole("Admin"))
            return Forbid();

        var reservations = await _reservationService.GetUserReservationsAsync(userId);
        return Ok(reservations);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ReservationResponseDto>> UpdateReservation(Guid id, [FromBody] UpdateReservationDto dto)
    {
        var reservation = await _reservationService.UpdateReservationAsync(id, dto);
        if (reservation == null) return NotFound();
        return Ok(reservation);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelReservation(Guid id)
    {
        var result = await _reservationService.CancelReservationAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
