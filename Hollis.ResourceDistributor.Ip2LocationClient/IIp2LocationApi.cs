using Hollis.ResourceDistributor.Ip2LocationClient.Models;
using Refit;

namespace Hollis.ResourceDistributor.Ip2LocationClient;

public interface IIp2LocationApi
{
    private const string GeoLocationPath = "/";

    [Get(GeoLocationPath)]
    Task<GeoLocationResponse> FetchGeoLocationAsync([Query] string ip);
}
