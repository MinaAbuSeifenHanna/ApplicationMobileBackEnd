using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayHub.Backend.Application.Features.Amenities.DTOs;
using StayHub.Backend.Application.Features.Amenities.Interfaces;

namespace StayHub.Backend.Controllers;

[Route("api/amenities")]
[ApiController]
public class AmenitiesController : ControllerBase
{
    private readonly IAmenityService _amenityService;

    public AmenitiesController(IAmenityService amenityService)
    {
        _amenityService = amenityService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _amenityService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var amenity = await _amenityService.GetByIdAsync(id);
        if (amenity == null) return NotFound();
        return Ok(amenity);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateAmenityDto createAmenityDto)
    {
        var amenity = await _amenityService.CreateAsync(createAmenityDto);
        return CreatedAtAction(nameof(GetById), new { id = amenity.Id }, amenity);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAmenityDto updateAmenityDto)
    {
        var amenity = await _amenityService.UpdateAsync(id, updateAmenityDto);
        if (amenity == null) return NotFound();
        return Ok(amenity);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _amenityService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
