using Microsoft.EntityFrameworkCore;
using StayHub.Backend.Application.Features.Amenities.DTOs;
using StayHub.Backend.Application.Features.Amenities.Interfaces;
using StayHub.Backend.Domain.Entities;
using StayHub.Backend.Infrastructure.Data;

namespace StayHub.Backend.Infrastructure.Services;

public class AmenityService : IAmenityService
{
    private readonly ApplicationDbContext _context;

    public AmenityService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AmenityDto>> GetAllAsync()
    {
        return await _context.Amenities
            .Select(a => new AmenityDto
            {
                Id = a.Id,
                Name = a.Name,
                IconName = a.IconName
            })
            .ToListAsync();
    }

    public async Task<AmenityDto?> GetByIdAsync(int id)
    {
        var amenity = await _context.Amenities.FindAsync(id);
        if (amenity == null) return null;

        return new AmenityDto
        {
            Id = amenity.Id,
            Name = amenity.Name,
            IconName = amenity.IconName
        };
    }

    public async Task<AmenityDto> CreateAsync(CreateAmenityDto createAmenityDto)
    {
        var amenity = new Amenity
        {
            Name = createAmenityDto.Name,
            IconName = createAmenityDto.IconName
        };

        _context.Amenities.Add(amenity);
        await _context.SaveChangesAsync();

        return new AmenityDto
        {
            Id = amenity.Id,
            Name = amenity.Name,
            IconName = amenity.IconName
        };
    }

    public async Task<AmenityDto?> UpdateAsync(int id, UpdateAmenityDto updateAmenityDto)
    {
        var amenity = await _context.Amenities.FindAsync(id);
        if (amenity == null) return null;

        amenity.Name = updateAmenityDto.Name;
        amenity.IconName = updateAmenityDto.IconName;

        await _context.SaveChangesAsync();

        return new AmenityDto
        {
            Id = amenity.Id,
            Name = amenity.Name,
            IconName = amenity.IconName
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var amenity = await _context.Amenities.FindAsync(id);
        if (amenity == null) return false;

        _context.Amenities.Remove(amenity);
        await _context.SaveChangesAsync();
        return true;
    }
}
