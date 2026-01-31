namespace Hollis.ResourceDistributor.Ip2LocationClient.Models;

public class GeoLocationResponse
{
    public required string Ip { get; init; }

    public required string CountryCode { get; init; }

    public required string CountryName { get; init; }

    public required string RegionName { get; init; }

    public required string CityName { get; init; }

    public double? Latitude { get; init; }

    public double? Longitude { get; init; }

    public required string ZipCode { get; init; }

    public required string TimeZone { get; init; }

    public required string ASN { get; init; }

    public required string AS { get; init; }

    public bool IsProxy { get; init; }
}
