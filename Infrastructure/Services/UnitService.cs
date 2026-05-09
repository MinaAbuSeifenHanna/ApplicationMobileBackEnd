using Microsoft.EntityFrameworkCore;
using StayHub.Backend.Application.Features.Auth.Interfaces;
using StayHub.Backend.Application.Features.Units.DTOs;
using StayHub.Backend.Application.Features.Units.Interfaces;
using StayHub.Backend.Domain.Entities;
using StayHub.Backend.Domain.Enums;
using StayHub.Backend.Infrastructure.Data;

using StayHub.Backend.Application.Common.Interfaces;

namespace StayHub.Backend.Infrastructure.Services;

public class UnitService : IUnitService
{
    private readonly ApplicationDbContext _context;
    private readonly IPhotoService _photoService;

    public UnitService(ApplicationDbContext context, IPhotoService photoService)
    {
        _context = context;
        _photoService = photoService;
    }

    public async Task<IEnumerable<UnitListDto>> GetAllUnitsAsync(int? cityId, UnitStatus? status)
    {
        var query = _context.Units.AsQueryable();

        if (cityId.HasValue)
            query = query.Where(u => u.CityId == cityId.Value);

        if (status.HasValue)
            query = query.Where(u => u.Status == status.Value);

        return await query
            .Select(u => new UnitListDto
            {
                Id = u.Id,
                Title = u.Title,
                Address = u.Address,
                PricePerNight = u.PricePerNight,
                Bedrooms = u.Bedrooms,
                Bathrooms = u.Bathrooms,
                HostName = u.OwnerUser.FirstName + " " + u.OwnerUser.LastName,
                HostRating = u.Reservations.SelectMany(r => r.Reviews).Any() 
                    ? u.Reservations.SelectMany(r => r.Reviews).Average(rev => rev.Rating) 
                    : 0,
                ReviewsCount = u.Reservations.SelectMany(r => r.Reviews).Count(),
                ImagesUrl = u.Images.Select(i => i.ImageUrl).ToList(),
                Amenities = u.UnitAmenities.Select(ua => new AmenityDto
                {
                    Id = ua.Amenity.Id,
                    Name = ua.Amenity.Name,
                    IconName = ua.Amenity.IconName
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<UnitDetailsDto?> GetUnitByIdAsync(Guid id)
    {
        return await _context.Units
            .Where(u => u.Id == id)
            .Select(u => new UnitDetailsDto
            {
                Id = u.Id,
                Title = u.Title,
                Address = u.Address,
                PricePerNight = u.PricePerNight,
                Bedrooms = u.Bedrooms,
                Bathrooms = u.Bathrooms,
                Description = u.Description,
                Capacity = u.Capacity,
                UnitType = u.UnitType,
                Status = u.Status,
                HostName = u.OwnerUser.FirstName + " " + u.OwnerUser.LastName,
                HostRating = u.Reservations.SelectMany(r => r.Reviews).Any() 
                    ? u.Reservations.SelectMany(r => r.Reviews).Average(rev => rev.Rating) 
                    : 0,
                ReviewsCount = u.Reservations.SelectMany(r => r.Reviews).Count(),
                ImagesUrl = u.Images.Select(i => i.ImageUrl).ToList(),
                Amenities = u.UnitAmenities.Select(ua => new AmenityDto
                {
                    Id = ua.Amenity.Id,
                    Name = ua.Amenity.Name,
                    IconName = ua.Amenity.IconName
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<UnitDetailsDto> CreateUnitAsync(CreateUnitDto createUnitDto, string ownerId)
    {
        var unit = new Unit
        {
            UnitNumber = createUnitDto.UnitNumber,
            Title = createUnitDto.Title,
            Description = createUnitDto.Description,
            PricePerNight = createUnitDto.PricePerNight,
            Address = createUnitDto.Address,
            Capacity = createUnitDto.Capacity,
            Bedrooms = createUnitDto.Bedrooms,
            Bathrooms = createUnitDto.Bathrooms,
            CityId = createUnitDto.CityId,
            UnitType = createUnitDto.UnitType,
            OwnerId = ownerId,
            Status = UnitStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        if (createUnitDto.AmenityIds != null)
        {
            foreach (var amenityId in createUnitDto.AmenityIds)
            {
                unit.UnitAmenities.Add(new UnitAmenity { AmenityId = amenityId });
            }
        }

        if (createUnitDto.Images != null)
        {
            foreach (var image in createUnitDto.Images)
            {
                var imageUrl = await _photoService.UploadImageAsync(image);
                if (imageUrl != null)
                {
                    unit.Images.Add(new UnitImage { ImageUrl = imageUrl });
                }
            }
        }

        _context.Units.Add(unit);
        await _context.SaveChangesAsync();

        return (await GetUnitByIdAsync(unit.Id))!;
    }

    public async Task<UnitDetailsDto?> UpdateUnitAsync(Guid id, UpdateUnitDto updateUnitDto)
    {
        var unit = await _context.Units
            .Include(u => u.UnitAmenities)
            .Include(u => u.Images)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (unit == null) return null;

        if (updateUnitDto.Title != null) unit.Title = updateUnitDto.Title;
        if (updateUnitDto.Description != null) unit.Description = updateUnitDto.Description;
        if (updateUnitDto.PricePerNight.HasValue) unit.PricePerNight = updateUnitDto.PricePerNight.Value;
        if (updateUnitDto.Address != null) unit.Address = updateUnitDto.Address;
        if (updateUnitDto.Capacity.HasValue) unit.Capacity = updateUnitDto.Capacity.Value;
        if (updateUnitDto.Bedrooms.HasValue) unit.Bedrooms = updateUnitDto.Bedrooms.Value;
        if (updateUnitDto.Bathrooms.HasValue) unit.Bathrooms = updateUnitDto.Bathrooms.Value;
        if (updateUnitDto.CityId.HasValue) unit.CityId = updateUnitDto.CityId.Value;
        if (updateUnitDto.UnitType.HasValue) unit.UnitType = updateUnitDto.UnitType.Value;
        if (updateUnitDto.Status.HasValue) unit.Status = updateUnitDto.Status.Value;

        if (updateUnitDto.AmenityIds != null)
        {
            unit.UnitAmenities.Clear();
            foreach (var amenityId in updateUnitDto.AmenityIds)
            {
                unit.UnitAmenities.Add(new UnitAmenity { AmenityId = amenityId });
            }
        }

        if (updateUnitDto.NewImages != null)
        {
            foreach (var image in updateUnitDto.NewImages)
            {
                var imageUrl = await _photoService.UploadImageAsync(image);
                if (imageUrl != null)
                {
                    unit.Images.Add(new UnitImage { ImageUrl = imageUrl });
                }
            }
        }

        await _context.SaveChangesAsync();

        return await GetUnitByIdAsync(unit.Id);
    }

    public async Task<bool> DeleteUnitAsync(Guid id, bool softDelete = true)
    {
        var unit = await _context.Units.FindAsync(id);
        if (unit == null) return false;

        if (softDelete)
        {
            unit.Status = UnitStatus.Inactive;
        }
        else
        {
            _context.Units.Remove(unit);
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
