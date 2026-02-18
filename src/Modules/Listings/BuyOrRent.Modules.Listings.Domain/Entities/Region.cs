namespace BuyOrRent.Modules.Listings.Domain.Entities;

public class Region
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public Guid CountryId { get; set; }
    public Country Country { get; set; } = null!;
    public List<Municipality> Municipalities { get; set; } = [];
}
