using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayHub.Backend.Application.Features.Units.DTOs;
using StayHub.Backend.Application.Features.Units.Interfaces;
using StayHub.Backend.Domain.Enums;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace StayHub.Backend.Controllers;

[Route("api/units")]
[ApiController]
public class UnitsController : ControllerBase
{
    private readonly IUnitService _unitService;

    public UnitsController(IUnitService unitService)
    {
        _unitService = unitService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? cityId, [FromQuery] UnitStatus? status)
    {
        var units = await _unitService.GetAllUnitsAsync(cityId, status);
        return Ok(units);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var unit = await _unitService.GetUnitByIdAsync(id);
        if (unit == null) return NotFound();
        return Ok(unit);
    }

    [HttpPost]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] CreateUnitDto createUnitDto)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) 
            return Unauthorized(new { message = "User ID not found in token" });

        var unit = await _unitService.CreateUnitAsync(createUnitDto, userId);
        return CreatedAtAction(nameof(GetById), new { id = unit.Id }, unit);
    }

    [HttpPut("{id}")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateUnitDto updateUnitDto)
    {
        var unit = await _unitService.UpdateUnitAsync(id, updateUnitDto);
        if (unit == null) return NotFound();
        return Ok(unit);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] bool softDelete = true)
    {
        var result = await _unitService.DeleteUnitAsync(id, softDelete);
        if (!result) return NotFound();
        return NoContent();
    }
}
