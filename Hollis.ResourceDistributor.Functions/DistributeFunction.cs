using Hollis.ResourceDistributor.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace Hollis.ResourceDistributor.Functions;

public class DistributeFunction(
    ILogger<DistributeFunction> logger,
    ResourceDistributorDbContext dbContext,
    HttpClient httpClient,
    IOptionsMonitor<AppConfig> appConfig)
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

        // auth
        if (!resource.AllowAnymouse)
        {
            var userIdentify = req.Query["user"];
            var user = await dbContext.Users
                .FirstOrDefaultAsync(x => x.ClearTextKey == userIdentify, cancellationToken);
            if (user is null)
            {
                logger.LogWarning("User from {city}({ip}) with wrong key {key} request for resource {rid}, accept.", "city", "ip", userIdentify, resource.Id);
                return req.CreateResponse(HttpStatusCode.NotFound);
            }

            logger.LogInformation("User {name} from {city}({ip}) request for resource {rid}, accept.", user.IdentificationName, "city", "ip", resource.Id);
        }
        else
        {
            logger.LogInformation("User from {city}({ip}) request anymouse resource {rid}, accpet.", "city", "ip", resource.Id);
        }

        // copy request header
        var defaultUserAgent = Environment.GetEnvironmentVariable("default_user_agent");
        httpClient.DefaultRequestHeaders.Add("User-Agent", defaultUserAgent);

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

        await rescourResponse.Content.CopyToAsync(result.Body, cancellationToken); ;
        return result;
    }
}
