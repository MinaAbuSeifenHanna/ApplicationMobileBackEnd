using StayHub.Backend.Application.Features.Units.DTOs;
using StayHub.Backend.Domain.Enums;

namespace StayHub.Backend.Application.Features.Units.Interfaces;

public interface IUnitService
{
    Task<IEnumerable<UnitListDto>> GetAllUnitsAsync(int? cityId, UnitStatus? status);
    Task<UnitDetailsDto?> GetUnitByIdAsync(Guid id);
    Task<UnitDetailsDto> CreateUnitAsync(CreateUnitDto createUnitDto, string ownerId);
    Task<UnitDetailsDto?> UpdateUnitAsync(Guid id, UpdateUnitDto updateUnitDto);
    Task<bool> DeleteUnitAsync(Guid id, bool softDelete = true);
}
