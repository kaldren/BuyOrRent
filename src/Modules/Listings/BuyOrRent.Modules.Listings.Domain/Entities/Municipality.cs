namespace BuyOrRent.Modules.Listings.Domain.Entities;

public class Municipality
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public Guid RegionId { get; set; }
    public Region Region { get; set; } = null!;
    public List<City> Cities { get; set; } = [];
}
