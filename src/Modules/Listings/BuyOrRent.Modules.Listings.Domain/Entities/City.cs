namespace BuyOrRent.Modules.Listings.Domain.Entities;

public class City
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public Guid MunicipalityId { get; set; }
    public Municipality Municipality { get; set; } = null!;
}
