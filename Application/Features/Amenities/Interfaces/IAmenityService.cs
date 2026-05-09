using StayHub.Backend.Application.Features.Amenities.DTOs;

namespace StayHub.Backend.Application.Features.Amenities.Interfaces;

public interface IAmenityService
{
    Task<IEnumerable<AmenityDto>> GetAllAsync();
    Task<AmenityDto?> GetByIdAsync(int id);
    Task<AmenityDto> CreateAsync(CreateAmenityDto createAmenityDto);
    Task<AmenityDto?> UpdateAsync(int id, UpdateAmenityDto updateAmenityDto);
    Task<bool> DeleteAsync(int id);
}
