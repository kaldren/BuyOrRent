namespace BuyOrRent.Modules.Listings.Domain.Entities;

public class Country
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public List<Region> Regions { get; set; } = [];
}
