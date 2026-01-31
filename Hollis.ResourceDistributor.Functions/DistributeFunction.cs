using Hollis.ResourceDistributor.Functions.Configs;
using Hollis.ResourceDistributor.Functions.Entities;
using Hollis.ResourceDistributor.Ip2LocationClient;
using Hollis.ResourceDistributor.Ip2LocationClient.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace Hollis.ResourceDistributor.Functions;

public class DistributeFunction(
    IOptionsMonitor<AppConfig> appConfig,
    ILogger<DistributeFunction> logger,
    ResourceDistributorDbContext dbContext,
    IIp2LocationApi locationApi,
    HttpClient httpClient)
{
    const string Get = "get";

    [Function(nameof(GetResource))]
    public async Task<HttpResponseData> GetResource(
        [HttpTrigger(AuthorizationLevel.Anonymous, Get, Route = $"{nameof(Resource)}/{{id}}")]
        HttpRequestData req,
        Guid id,
        CancellationToken cancellationToken)
    {
        var resource = await dbContext.Resources
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (resource is null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        foreach (var h in req.Headers)
        {
            logger.LogInformation("Req hdr {h}: `{j}`", h.Key, JsonSerializer.Serialize(h.Value));
        }

        // ip audit log
        string? userIp = null;
        GeoLocationResponse? userGeoInfo = null;

        // get ip
        req.Headers.TryGetValues("CLIENT-IP", out var userIpList);
        if (userIpList?.Any() ?? false)
        {
            if (IPEndPoint.TryParse(userIpList.First(), out var ip))
            {
                userIp = ip.Address.ToString();
            }
        }

        // get geo location
        if (!string.IsNullOrWhiteSpace(userIp))
        {
            userGeoInfo = await locationApi.FetchGeoLocationAsync(new() { Ip = userIp });
        }

        // auth
        if (!resource.AllowAnymouse)
        {
            var userIdentify = req.Query["user"];
            var user = await dbContext.Users
                .FirstOrDefaultAsync(x => x.ClearTextKey == userIdentify, cancellationToken);
            if (user is null)
            {
                logger.LogWarning("User from {city}({ip}) with wrong key {key} request for resource {rid}, reject.", userGeoInfo?.CityName, userGeoInfo?.Ip, userIdentify, resource.Id);
                return req.CreateResponse(HttpStatusCode.NotFound);
            }

            logger.LogInformation("User {name} from {city}({ip}) request for resource {rid}, accept.", user.IdentificationName, userGeoInfo?.CityName, userGeoInfo?.Ip, resource.Id);
        }
        else
        {
            logger.LogInformation("User from {city}({ip}) request anymouse resource {rid}, accpet.", userGeoInfo?.CityName, userGeoInfo?.Ip, resource.Id);
        }

        // copy request header
        httpClient.DefaultRequestHeaders.Add("User-Agent", appConfig.CurrentValue.DefaultUserAgent);

        var rescourResponse = await httpClient.GetAsync(resource.TargetUrl, cancellationToken);
        if (rescourResponse.StatusCode != HttpStatusCode.OK)
        {
            var content = await rescourResponse.Content.ReadAsStringAsync(cancellationToken);
            logger.LogError("Fetch failed, status:{code}, message: {msg}", rescourResponse.StatusCode, content);
        }
        else
        {
            logger.LogInformation("Fetch config success.");
        }

        var result = req.CreateResponse(HttpStatusCode.OK);

        // copy response headers
        foreach (var headerName in resource.ResponseCopyHeaderName)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!rescourResponse.Headers.TryGetValues(headerName, out var headerValue))
            {
                continue;
            }

            if (result.Headers.Any(x => x.Key == headerName))
            {
                result.Headers.Remove(headerName);
            }

            result.Headers.Add(headerName, headerValue);
        }

        await rescourResponse.Content.CopyToAsync(result.Body, cancellationToken);
        return result;
    }
}
