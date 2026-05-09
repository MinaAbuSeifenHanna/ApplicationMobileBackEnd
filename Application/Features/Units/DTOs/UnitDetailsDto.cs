using StayHub.Backend.Domain.Enums;

namespace StayHub.Backend.Application.Features.Units.DTOs;

public class UnitDetailsDto : UnitListDto
{
    public string Description { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public UnitType UnitType { get; set; }
    public UnitStatus Status { get; set; }
}
