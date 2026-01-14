using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
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
    private readonly string route = nameof(GetResource).Replace("Get", string.Empty, StringComparison.OrdinalIgnoreCase) + "config/{code}";

    [Function(nameof(GetResource))]
    public async Task<IActionResult> GetResource(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = route)] HttpRequest req,
        string code)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.ClearTextKey == code);
        if (user is null)
        {
            return new NotFoundResult();
        }

        logger.LogInformation("User {name} from {city}({ip}) with code {code} request for config, accept.", user.IdentificationName, "city", "ip", user.ClearTextKey);

        httpClient.DefaultRequestHeaders.Add("User-Agent", appConfig.Value.DefaultUserAgent);


        var response = await httpClient.GetAsync(appConfig.Value.TargetUrl);
        var content = await response.Content.ReadAsStringAsync();
        if (response.StatusCode != HttpStatusCode.OK)
        {
            logger.LogError("Fetch failed, status:{code}, message: {msg}", response.StatusCode, content);
        }
        else
        {
            logger.LogInformation("Fetch config success.");
        }

        return new OkObjectResult(content);
    }
}
