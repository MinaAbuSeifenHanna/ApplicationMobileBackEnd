using System.ComponentModel.DataAnnotations;

namespace StayHub.Backend.Domain.Entities;

public class Amenity
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? IconName { get; set; }

    public virtual ICollection<UnitAmenity> UnitAmenities { get; set; } = new HashSet<UnitAmenity>();
}
