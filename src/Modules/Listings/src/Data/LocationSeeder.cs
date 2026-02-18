using System.Text.Json;
using Locations.Location.Abstractions;
using Locations.Location.Entities;

namespace Listings.Data;

public class LocationSeeder(ILocationRepository locationRepository)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await locationRepository.HasLocationsAsync(cancellationToken))
            return;

        var assembly = typeof(LocationSeeder).Assembly;
        var resourceNames = assembly.GetManifestResourceNames()
            .Where(n => n.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                     && n.Contains("SeedData", StringComparison.OrdinalIgnoreCase));

        foreach (var resourceName in resourceNames)
        {
            await using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is null) continue;

            var data = await JsonSerializer.DeserializeAsync<CountrySeedData>(stream, JsonOptions, cancellationToken);
            if (data is null) continue;

            var country = new Country
            {
                Id = Guid.NewGuid(),
                Name = data.Name,
                Code = data.Code
            };

            foreach (var regionData in data.Regions)
            {
                var region = new Region
                {
                    Id = Guid.NewGuid(),
                    Name = regionData.Name,
                    CountryId = country.Id
                };

                foreach (var municipalityData in regionData.Municipalities)
                {
                    var municipality = new Municipality
                    {
                        Id = Guid.NewGuid(),
                        Name = municipalityData.Name,
                        RegionId = region.Id
                    };

                    foreach (var cityName in municipalityData.Cities)
                    {
                        municipality.Cities.Add(new City
                        {
                            Id = Guid.NewGuid(),
                            Name = cityName,
                            MunicipalityId = municipality.Id
                        });
                    }

                    region.Municipalities.Add(municipality);
                }

                country.Regions.Add(region);
            }

            await locationRepository.SeedAsync(country, cancellationToken);
        }
    }

    private record CountrySeedData(string Name, string Code, List<RegionSeedData> Regions);
    private record RegionSeedData(string Name, List<MunicipalitySeedData> Municipalities);
    private record MunicipalitySeedData(string Name, List<string> Cities);
}
