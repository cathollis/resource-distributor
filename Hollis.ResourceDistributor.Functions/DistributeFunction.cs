using Hollis.ResourceDistributor.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;

namespace Hollis.ResourceDistributor.Functions;

public class DistributeFunction(
    ILogger<DistributeFunction> logger,
    ResourceDistributorDbContext dbContext,
    IOptions<AppConfig> appConfig,
    HttpClient httpClient)
{
    const string Get = "get";

    [Function(nameof(GetResource))]
    public async Task<HttpResponseData> GetResource(
        [HttpTrigger(AuthorizationLevel.Anonymous, Get, Route = $"{nameof(Resource)}/{{id}}")]
        HttpRequestData req,
        Guid id)
    {
        var resource = await dbContext.Resources
            .FirstOrDefaultAsync(x => x.Id == id);
        if (resource is null)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        // auth
        if (!resource.AllowAnymouse)
        {
            var userIdentify = req.Query["user"];
            var user = await dbContext.Users
                .FirstOrDefaultAsync(x => x.ClearTextKey == userIdentify);
            if (user is null)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }

            logger.LogInformation("User {name} from {city}({ip}) with code {code} request for config, accept.", user.IdentificationName, "city", "ip", user.ClearTextKey);
        }

        httpClient.DefaultRequestHeaders.Add("User-Agent", appConfig.Value.DefaultUserAgent);

        var response = await httpClient.GetAsync(resource.TargetUrl);
        var content = await response.Content.ReadAsStringAsync();
        if (response.StatusCode != HttpStatusCode.OK)
        {
            logger.LogError("Fetch failed, status:{code}, message: {msg}", response.StatusCode, content);
        }
        else
        {
            logger.LogInformation("Fetch config success.");
        }

        // copy response headers
        var result = req.CreateResponse(HttpStatusCode.OK);
        foreach (var headerName in resource.ResponseCopyHeaderName)
        {
            if (!response.Headers.TryGetValues(headerName, out var headerValue))
            {
                continue;
            }

            if (result.Headers.Any(x => x.Key == headerName))
            {
                result.Headers.Remove(headerName);
            }

            result.Headers.Add(headerName, headerValue);
        }

        return result;
    }
}
