namespace StayHub.Backend.Domain.Entities;

public class UnitAmenity
{
    public Guid UnitId { get; set; }
    public int AmenityId { get; set; }

    public virtual Unit Unit { get; set; } = null!;
    public virtual Amenity Amenity { get; set; } = null!;
}
